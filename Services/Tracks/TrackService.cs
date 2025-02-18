using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Slugify;
using Sounds_New.Db;
using Sounds_New.DTO;
using Sounds_New.Models;
using Sounds_New.Utils;

namespace Sounds_New.Services.Tracks
{
    public class TrackService(SoundsContext context) : ITrackService
    {
        private readonly SoundsContext _context = context;

        public async Task<Track?> GetTrackBySlug(string slug)
        {
            var track = await _context.Tracks.Include(t => t.User).FirstAsync(t => t.Slug == slug);
            return track;
        }

        public async Task<CreateTrackResultDTO> CreateTrack(CreateTrackDTO dto, string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                var result = new CreateTrackResultDTO
                {
                    Status = 401,
                    Message = "Authentication failed"
                };
                return result;
            }

            var slug = new SlugHelper().GenerateSlug($"{dto.Title}-by-{user.Username}");

            var duplicate = await _context.Tracks.AnyAsync(t => t.Slug == slug);
            if (duplicate)
            {
                var result = new CreateTrackResultDTO
                {
                    Status = 400,
                    Message = "You already have a track with this title"
                };
                return result;
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var artworkFileName = $"{Guid.NewGuid()}_{dto.ArtworkFile.FileName}";
            var artworkFilePath = Path.Combine(uploadsFolder, artworkFileName);

            using (var stream = new FileStream(artworkFilePath, FileMode.Create))
            {
                await dto.ArtworkFile.CopyToAsync(stream);
            }

            var audioFileName = $"{Guid.NewGuid()}_{dto.AudioFile.FileName}";
            var audioFilePath = Path.Combine(uploadsFolder, audioFileName);

            using (var stream = new FileStream(audioFilePath, FileMode.Create))
            {
                await dto.AudioFile.CopyToAsync(stream);
            }

            var track = new Track
            {
                Title = dto.Title,
                Slug = slug,
                Genres = dto.Genres,
                ImageFilePath = artworkFileName,
                AudioFilePath = audioFileName,
                UserId = user.Id,
                User = user
            };

            var created = await _context.Tracks.AddAsync(track);
            await _context.SaveChangesAsync();

            try
            {
                Utilites.SendTrackToAnalysisService(track);
            }
            catch (HttpRequestException)
            {
                // TODO: Handle this
                Console.WriteLine("Failed to send track to analysis service");
            }

            return new CreateTrackResultDTO
            {
                Status = 201,
                Message = "Track created successfully",
                Track = created.Entity
            };
        }

        public async Task<List<Track>> GetHotTracks()
        {
            var tracks = await _context.Tracks.OrderByDescending(t => t.Likes).Take(10).ToListAsync();
            return tracks;
        }

        public async Task<List<Track>> GetTracksByUser(string userSlug, string? ctxUsername)
        {
            var isAuthor = false;

            if (ctxUsername != null)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == ctxUsername);
                if (user != null)
                {
                    isAuthor = user.Slug == userSlug;
                }
            }

            if (isAuthor)
            {
                return await _context.Tracks.Where(t => t.User.Slug == userSlug).ToListAsync();
            }

            return await _context.Tracks.Where(t => t.User.Slug == userSlug && t.IsPublic).ToListAsync();
        }

        public async Task<UpdateTrackDataStatus> UpdateTrackData(UpdateTrackDataDTO dto, int id)
        {
            var track = await _context.Tracks.FindAsync(id);
            if (track is null)
            {
                return UpdateTrackDataStatus.TrackNotFound;
            }

            track.Peaks = dto.Peaks;
            await _context.SaveChangesAsync();
            return UpdateTrackDataStatus.Success;
        }

        public async Task<DefaultMethodResponseDTO> DeleteTrack(int trackId, string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return new DefaultMethodResponseDTO
                {
                    IsOk = false,
                    Message = "Пользователь не существует",
                    StatusCode = 401,
                };
            }

            var track = await _context.Tracks.FirstOrDefaultAsync(t => t.Id == trackId);
            if (track == null)
            {
                return new DefaultMethodResponseDTO
                {
                    IsOk = false,
                    Message = "Трек не существует",
                    StatusCode = 404,
                };
            }

            if (track.UserId != user.Id)
            {
                return new DefaultMethodResponseDTO
                {
                    IsOk = false,
                    Message = "Вы не можете удалить этот трек",
                    StatusCode = 403,
                };
            }

            _context.Tracks.Remove(track);
            await _context.SaveChangesAsync();
            return new DefaultMethodResponseDTO
            {
                IsOk = true,
                Message = "Трек успешно удален",
                StatusCode = 200,
            };
        }
    }
}
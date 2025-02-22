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

        /// <summary>
        /// Метод для поиска трека по slug
        /// </summary>
        /// <param name="slug">Slug трека</param>
        /// <returns>Трек с автором. Если трек не найден, возвращает null</returns>
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

            var slug = GenerateTrackSlug(dto.Title, user.Username);

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
                Description = dto.Description,
                Slug = slug,
                Genres = dto.Genres,
                ImageFilePath = artworkFileName,
                AudioFilePath = audioFileName,
                UserId = user.Id,
                User = user,
                CreatedAt = DateTime.Now
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

        public async Task<DefaultMethodResponseDTO> UpdateTrackPrimaryData(UpdateTrackPrimaryDataDTO dto, string trackSlug, string ctxUsername)
        {
            var track = await _context.Tracks.Include(t => t.User).FirstOrDefaultAsync(t => t.Slug == trackSlug);
            if (track == null)
            {
                return DefaultReturnFabric.NotFound("Трек не найден");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == ctxUsername);
            if (user == null)
            {
                return DefaultReturnFabric.Unauthorized("Ошибка авторизации");
            }

            if (track.User.Id != user.Id)
            {
                return DefaultReturnFabric.Forbidden("Вы не можете редактировать этот трек");
            }

            var slug = GenerateTrackSlug(dto.Title, user.Username);

            track.Title = dto.Title;
            track.Slug = slug;
            track.Description = dto.Description;
            track.Genres = dto.Genres;
            track.IsDownloadsEnabled = dto.IsDownloadsEnabled;
            track.IsPublic = dto.IsPublic;

            await _context.SaveChangesAsync();

            return DefaultReturnFabric.Ok(slug);
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

        public async Task<DefaultMethodResponseDTO> ChangeTrackVisibility(string slug, string username, bool newIsPublic)
        {
            var track = await _context.Tracks.Include(t => t.User).FirstOrDefaultAsync(t => t.Slug == slug);
            if (track == null)
            {
                return new DefaultMethodResponseDTO
                {
                    IsOk = false,
                    StatusCode = 404,
                    Message = "Трек не найден"
                };
            }

            if (track.User.Username != username)
            {
                return new DefaultMethodResponseDTO
                {
                    IsOk = false,
                    StatusCode = 403,
                    Message = "Действие запрещено"
                };
            }

            track.IsPublic = newIsPublic;
            await _context.SaveChangesAsync();

            return new DefaultMethodResponseDTO
            {
                IsOk = true,
                StatusCode = 204,
                Message = "Ok"
            };
        }

        public async Task<DefaultMethodResponseDTO> IncreaseTrackListens(string slug)
        {
            var track = await _context.Tracks.FirstOrDefaultAsync(t => t.Slug == slug);
            if (track == null)
            {
                return new DefaultMethodResponseDTO
                {
                    IsOk = false,
                    Message = "Трек не найден",
                    StatusCode = 404
                };
            }

            track.Listens += 1;
            await _context.SaveChangesAsync();

            return new DefaultMethodResponseDTO
            {
                IsOk = true,
                Message = "Ok",
                StatusCode = 204
            };
        }

        ///

        private string GenerateTrackSlug(string title, string username)
        {
            var slug = new SlugHelper().GenerateSlug($"{title}-by-{username}");
            return slug;
        }

        /// <summary>
        /// Toggles a like on the specified track for the given user. If like already exists, it is deleted.
        /// </summary>
        /// <param name="trackSlug">The slug of the track to be liked or unliked.</param>
        /// <param name="ctxUsername">The username of the user performing the action.</param>
        /// <returns>A response indicating the success or failure of the operation. Returns a not found response if the track or user is not found.</returns>

        public async Task<DefaultMethodResponseDTO> ChangeTrackLikes(string trackSlug, string ctxUsername)
        {
            var trackTask = _context.Tracks.FirstOrDefaultAsync(t => t.Slug == trackSlug);
            var userTask = _context.Users.FirstOrDefaultAsync(u => u.Username == ctxUsername);

            await Task.WhenAll(trackTask, userTask);

            var track = trackTask.Result;
            var user = userTask.Result;

            if (user is null || track is null)
            {
                return DefaultReturnFabric.NotFound("Трек и/или пользователь не найдены");
            }

            var like = await _context.Likes.FirstOrDefaultAsync(l => l.TrackId == track.Id && l.UserId == user.Id);
            if (like != null)
            {
                _context.Likes.Remove(like);
                track.LikesCount -= 1;

                await _context.SaveChangesAsync();

                return DefaultReturnFabric.Ok("Ok");
            }

            var newLike = new Like
            {
                TrackId = track.Id,
                UserId = user.Id
            };

            _context.Likes.Add(newLike);
            track.LikesCount += 1;
            await _context.SaveChangesAsync();

            return DefaultReturnFabric.Ok("Ok");
        }
    }
}
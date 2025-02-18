using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sounds_New.Db;
using Sounds_New.DTO;
using Sounds_New.Models;

namespace Sounds_New.Services.Comments
{
    public class CommentsService(SoundsContext context) : ICommentsService
    {
        private readonly SoundsContext _context = context;

        public async Task<Comment?> GetCommentById(int id)
        {
            var comment = await _context.Comments.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            return comment;
        }

        public async Task<DefaultMethodResponseDTO> CreateComment(CreateCommentDTO dto, string username)
        {
            var track = await _context.Tracks.Include(t => t.User).FirstOrDefaultAsync(t => t.Id == dto.TrackId);
            if (track == null)
            {
                return new DefaultMethodResponseDTO
                {
                    IsOk = false,
                    StatusCode = 404,
                    Message = "Трек не найден"
                };
            }

            var user = await _context.Users.FirstOrDefaultAsync(t => t.Username == username);
            if (user == null)
            {
                return new DefaultMethodResponseDTO
                {
                    IsOk = false,
                    StatusCode = 401,
                    Message = "Ошибка авторизации"
                };
            }

            if (!track.IsPublic && track.User.Username != username)
            {
                return new DefaultMethodResponseDTO
                {
                    IsOk = false,
                    StatusCode = 403,
                    Message = "Доступ к треку закрыт"
                };
            }

            var comment = new Comment
            {
                Text = dto.Text,
                User = user,
                Track = track
            };

            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();

            return new DefaultMethodResponseDTO
            {
                IsOk = true,
                StatusCode = 201,
                Message = "Комментарий создан"
            };
        }

        public async Task<List<Comment>?> GetCommentsByTrackSlug(string trackSlug, string? username)
        {
            var track = await _context.Tracks.Include(t => t.User).FirstOrDefaultAsync(t => t.Slug == trackSlug);
            if (track == null)
            {
                return null;
            }

            if (!track.IsPublic && track.User.Username != username)
            {
                return null;
            }

            if (!track.IsPublic && username == null)
            {
                return null;
            }

            var comments = await _context.Comments
                .Include(c => c.User)
                .Where(c => c.Track.Id == track.Id)
                .ToListAsync();

            return comments;
        }
    }
}
using Sounds_New.DTO;
using Sounds_New.Models;

namespace Sounds_New.Services.Comments
{
    public interface ICommentsService
    {
        public Task<Comment?> GetCommentById(int id);
        public Task<List<Comment>?> GetCommentsByTrackSlug(string trackSlug, string? username);
        public Task<DefaultMethodResponseDTO> CreateComment(CreateCommentDTO dto, string username);
    }
}
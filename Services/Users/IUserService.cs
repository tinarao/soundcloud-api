using Sounds_New.DTO;
using Sounds_New.Models;

namespace Sounds_New.Services.Users
{
    public interface IUserService
    {
        Task<User?> GetUserBySlug(string slug);
        Task<UserStatisticDTO> GetUserStatistics(string username);
        Task<UserPrimaryDataDTO?> GetUserPrimaryDataBySlug(string slug);
        Task<DefaultMethodResponseDTO> ChangeUserAvatar(IFormFile newAvatar, string username);
        Task<DefaultMethodResponseDTO> SetUserLinks(SetUserLinksDTO dto, string username);
    }
}
using Sounds_New.DTO;
using Sounds_New.Models;

namespace Sounds_New.Services.Users
{
    public interface IUserService
    {
        Task<User?> GetUserBySlug(string slug);
        Task<UserStatisticDTO> GetUserStatistics(string username);
        Task<UserPrimaryDataDTO?> GetUserPrimaryDataById(int userId);
    }
}
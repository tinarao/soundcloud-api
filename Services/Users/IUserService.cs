using Sounds_New.Models;

namespace Sounds_New.Services.Users
{
    public interface IUserService
    {
        Task<User?> GetUserBySlug(string slug);
    }
}
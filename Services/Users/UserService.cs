using Microsoft.EntityFrameworkCore;
using Sounds_New.Db;
using Sounds_New.Models;

namespace Sounds_New.Services.Users
{
    public class UserService(SoundsContext context) : IUserService
    {
        private readonly SoundsContext _context = context;

        public async Task<User?> GetUserBySlug(string slug)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Slug == slug);
            return user;
        }
    }
}
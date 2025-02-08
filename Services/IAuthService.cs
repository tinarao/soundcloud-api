using Microsoft.AspNetCore.Mvc;
using Sounds_New.DTO;
using Sounds_New.Models;

namespace Sounds_New.Services
{
    public interface IAuthService
    {
        Task<TokensDTO?> Login(LoginDTO dto);
        Task<User?> Register(RegisterDTO dto);
        Task<List<User>> GetAllUsers();
        Task<User?> GetMe(string username);
        Task<TokensDTO?> RefreshTokens(int userId, string refreshToken);
    }
}
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Slugify;
using Sounds_New.Db;
using Sounds_New.DTO;
using Sounds_New.Models;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace Sounds_New.Services
{
    public class AuthService(SoundsContext context, IConfiguration configuration) : IAuthService
    {
        private readonly SoundsContext _context = context;
        private readonly IConfiguration _configuration = configuration;

        public async Task<List<User>> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            return users;
        }

        public async Task<User?> Register(RegisterDTO dto)
        {
            var user = new User();
            var slug = new SlugHelper().GenerateSlug(dto.Username);

            var dup = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Username == dto.Username ||
                    u.Email == dto.Email ||
                    u.Slug == slug
                );

            if (dup != null)
            {
                return null;
            }

            var hashedPassword = new PasswordHasher<User>()
                .HashPassword(user, dto.Password);

            user.Username = dto.Username;
            user.Password = hashedPassword;
            user.Email = dto.Email;
            user.Slug = slug;
            user.CreatedAt = DateTime.Now;

            var created = await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<TokensDTO?> Login(LoginDTO dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
            {
                return null;
            }

            var hasher = new PasswordHasher<User>();
            if (hasher.VerifyHashedPassword(user, user.Password, dto.Password) == PasswordVerificationResult.Failed)
            {
                return null;
            }

            var response = new TokensDTO
            {
                AccessToken = GenerateAccessToken(user),
                RefreshToken = await GenerateAndSaveRefreshToken(user)
            };

            return response;
        }

        public async Task<TokensDTO?> RefreshTokens(int userId, string refreshToken)
        {
            var user = await ValidateRefreshToken(userId, refreshToken);
            if (user == null)
            {
                return null;
            }

            var response = new TokensDTO
            {
                AccessToken = GenerateAccessToken(user),
                RefreshToken = await GenerateAndSaveRefreshToken(user)
            };

            return response;
        }

        public async Task<User?> GetMe(string username)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            return user;
        }

        private string GenerateAccessToken(User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration.GetValue<string>("AppSettings:Token")!)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
            var tokenDescriptor = new JwtSecurityToken(
                issuer: _configuration.GetValue<string>("AppSettings:Issuer"),
                audience: _configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private async Task<string> GenerateAndSaveRefreshToken(User user)
        {
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.Now.AddDays(14);

            await _context.SaveChangesAsync();
            return refreshToken;
        }

        private async Task<User?> ValidateRefreshToken(int userId, string refreshToken)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiry < DateTime.Now)
            {
                return null;
            }

            return user;
        }
    }
}
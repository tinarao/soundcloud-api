using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sounds_New.DTO;
using Sounds_New.Models;
using Sounds_New.Services;

namespace Sounds_New.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpGet]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            var users = await _authService.GetAllUsers();
            return Ok(users);
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(RegisterDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity("Некорректный запрос");
            }

            var user = await _authService.Register(dto);
            if (user == null)
            {
                return BadRequest("Found user with same credentials.");
            }

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tokens = await _authService.Login(dto);
            if (tokens == null)
            {
                return Unauthorized("Provided credentials don't match our records");
            }

            return Ok(tokens);
        }

        [HttpPost("refresh-tokens")]
        public async Task<ActionResult<TokensDTO>> RefreshTokens(RefreshTokensDTO dto)
        {
            var tokens = await _authService.RefreshTokens(dto.UserId, dto.RefreshToken);
            if (tokens == null || tokens.AccessToken == null || tokens.RefreshToken == null)
            {
                return Unauthorized("Invalid refresh token");
            }

            return Ok(tokens);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            if (User.Identity == null || User.Identity.Name == null)
            {
                return Unauthorized();
            }

            var user = await _authService.GetMe(User.Identity.Name);
            return Ok(user);
        }

        [Authorize]
        [HttpGet("auth-only")]
        public IActionResult AuthOnlyEndpoint()
        {
            var user = User.Identity.Name;
            return Ok(user);
        }

        [CustomAuthorize(UserRoles.Admin)]
        [HttpGet("admin-only")]
        public IActionResult AdminOnlyEndpoint()
        {
            return Ok("You are an admin!");
        }
    }
}
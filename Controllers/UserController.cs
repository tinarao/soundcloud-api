using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sounds_New.DTO;
using Sounds_New.Services.Users;
using Sounds_New.Utils;

namespace Sounds_New.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpGet("{slug}")]
        public async Task<IActionResult> GetUserBySlug(string slug)
        {
            var user = await _userService.GetUserBySlug(slug);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [Authorize]
        [HttpGet("stats")]
        public async Task<ActionResult> GetUserStatistics()
        {
            if (User.Identity == null || User.Identity.Name == null)
            {
                return Unauthorized();
            }

            var stats = await _userService.GetUserStatistics(User.Identity.Name);
            return Ok(stats);
        }

        [HttpGet("primary/{slug}")]
        public async Task<ActionResult> GetUserPrimaryDataBySlug(string slug)
        {
            var user = await _userService.GetUserPrimaryDataBySlug(slug);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [Authorize]
        [HttpPatch("links")]
        public async Task<ActionResult> SetUserLinks(SetUserLinksDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var ctxUsername = Utilites.GetIdentityUserName(HttpContext);
            if (ctxUsername == null)
            {
                return Unauthorized();
            }

            var result = await _userService.SetUserLinks(dto, ctxUsername);

            return result.StatusCode switch
            {
                404 => NotFound(result.Message),
                200 => Ok(),
                _ => StatusCode(500, "An error occurred while changing the links")
            };
        }

        [Authorize]
        [HttpPatch("avatar")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> ChangeUserAvatar([FromForm] ChangeAvatarDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var ctxUsername = Utilites.GetIdentityUserName(HttpContext);
            if (ctxUsername == null)
            {
                return Unauthorized();
            }

            if (dto.NewAvatar.Length / 1024 / 1024 > 2)
            {
                return BadRequest();
            }

            var result = await _userService.ChangeUserAvatar(dto.NewAvatar, ctxUsername);

            return result.StatusCode switch
            {
                404 => NotFound(result.Message),
                200 => Ok(),
                _ => StatusCode(500, "An error occurred while changing the avatar")
            };
        }
    }
}
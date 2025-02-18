using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sounds_New.Services.Users;

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

        [HttpGet("primary/{id}")]
        public async Task<ActionResult> GetUserPrimaryDataById(int id)
        {
            var user = await _userService.GetUserPrimaryDataById(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
    }
}
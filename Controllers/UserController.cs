using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    }
}
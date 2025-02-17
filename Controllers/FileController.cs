using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sounds_New.Db;

namespace Sounds_New.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController(SoundsContext context) : ControllerBase
    {
        private readonly SoundsContext _context = context;

        [HttpGet("{slug}")]
        public async Task<IActionResult> GetAudioFile(string slug)
        {
            var track = await _context.Tracks.Include(t => t.User).FirstOrDefaultAsync(t => t.Slug == slug);
            if (track == null)
            {
                return NotFound("Track does not exist");
            }

            // var userContext = HttpContext.User.Identity;
            // if (!track.IsPublic)
            // {
            //     if (userContext == null || userContext.Name == null)
            //     {
            //         return Forbid();
            //     }

            //     if (track.User.Username != userContext.Name)
            //     {
            //         return Forbid();
            //     }
            // }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            string filePath = Path.Combine(uploadsFolder, track.AudioFilePath);


            FileStream fs = new(filePath, FileMode.Open, FileAccess.Read);
            return new FileStreamResult(fs, "audio/mpeg");
        }
    }
}
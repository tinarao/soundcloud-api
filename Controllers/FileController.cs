using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sounds_New.Db;
using Sounds_New.Services.SignedUrl;
using Sounds_New.Utils;

namespace Sounds_New.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController(SoundsContext context, ISignedUrlService signedUrlService) : ControllerBase
    {
        private readonly SoundsContext _context = context;
        private readonly ISignedUrlService _signedUrlService = signedUrlService;

        [HttpGet("track/{slug}")]
        public async Task<IActionResult> GetAudioFile(string slug)
        {
            var track = await _context.Tracks.Include(t => t.User).FirstOrDefaultAsync(t => t.Slug == slug);
            if (track == null)
            {
                return NotFound("Track does not exist");
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            string filePath = Path.Combine(uploadsFolder, track.AudioFilePath);

            if (track.IsPublic)
            {
                var signedUrl = await _signedUrlService.CreateSignedUrl(filePath);
                if (signedUrl == null)
                {
                    return StatusCode(500);
                }

                return Ok(signedUrl);
            }

            var userCtx = Utilites.GetIdentityUserName(HttpContext);
            if (!track.IsPublic && userCtx == null)
            {
                return Forbid();
            }

            if (!track.IsPublic && track.User.Username != userCtx)
            {
                return Forbid();
            }

            var aSignedUrl = await _signedUrlService.CreateSignedUrl(filePath);
            return Ok(aSignedUrl);
        }

        [HttpGet("image/{slug}")]
        public async Task<IActionResult> GetImageFile(string slug)
        {
            var track = await _context.Tracks.Include(t => t.User).FirstOrDefaultAsync(t => t.Slug == slug);
            if (track == null)
            {
                return NotFound("Not exist");
            }

            var userContext = User.Identity;
            if (!track.IsPublic)
            {
                if (userContext == null || userContext.Name == null)
                {
                    return Forbid();
                }

                if (track.User.Username != userContext.Name)
                {
                    return Forbid();
                }
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            string filePath = Path.Combine(uploadsFolder, track.ImageFilePath);

            FileStream fs = new(filePath, FileMode.Open, FileAccess.Read);
            return new FileStreamResult(fs, "image/jpeg");
        }
    }
}
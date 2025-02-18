using Microsoft.AspNetCore.Mvc;
using Sounds_New.Services.SignedUrl;

namespace Sounds_New.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SignedUrlController(ISignedUrlService signedUrlService) : ControllerBase
    {
        private readonly ISignedUrlService _signedUrlService = signedUrlService;

        [HttpGet("{signedUrl}")]
        public async Task<IActionResult> GetFile(string signedUrl)
        {
            var filepath = await _signedUrlService.GetFilePathByUrl(signedUrl);
            if (filepath == null)
            {
                return NotFound("Пользователь и/или треки не найдены");
            }

            if (filepath.EndsWith(".mp3"))
            {
                FileStream fs = new(filepath, FileMode.Open, FileAccess.Read);
                return new FileStreamResult(fs, "audio/mpeg");  // TODO: Multiple MIME types
            }
            else
            {
                FileStream fs = new(filepath, FileMode.Open, FileAccess.Read);
                return File(fs, "image/jpg");
            }
        }
    }
}
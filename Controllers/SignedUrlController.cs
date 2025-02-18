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

            var mime = filepath.EndsWith(".mp3") ? "audio/mpeg" : "image/jpg";

            FileStream fs = new(filepath, FileMode.Open, FileAccess.Read);
            return new FileStreamResult(fs, mime);  // TODO: Multiple MIME types
        }
    }
}
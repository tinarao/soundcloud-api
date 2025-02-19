using Microsoft.AspNetCore.Mvc;
using Sounds_New.Services.Files;
using static Sounds_New.Utils.Utilites;

namespace Sounds_New.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController(IFileService fileService) : ControllerBase
    {
        private readonly IFileService _fileService = fileService;

        [HttpGet("{kind}/{slug}")]
        public async Task<IActionResult> GetImageSignedUrl(string slug, FileKind kind)
        {
            var userCtx = GetIdentityUserName(HttpContext);
            var signedUrl = await _fileService.GetImageSignedUrl(slug, kind, userCtx);
            if (signedUrl == null)
            {
                return NotFound();
            }

            return Ok(signedUrl);
        }
    }
}
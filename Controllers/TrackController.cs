using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sounds_New.DTO;
using Sounds_New.Models;
using Sounds_New.Services.Tracks;

namespace Sounds_New.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrackController(ITrackService trackService) : ControllerBase
    {
        private readonly ITrackService _trackService = trackService;

        [HttpGet("{slug}")]
        public async Task<ActionResult<List<Track>>> GetTrackBySlug(string slug)
        {
            var track = await _trackService.GetTrackBySlug(slug);
            if (track == null)
            {
                return NotFound();
            }

            return Ok(track);
        }

        [Authorize]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateTrack([FromForm] CreateTrackDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }
            if (User.Identity == null || User.Identity.Name == null)
            {
                return Unauthorized();
            }

            var result = await _trackService.CreateTrack(dto, User.Identity.Name);
            return result.Status switch
            {
                400 => BadRequest(result.Message),
                401 => Unauthorized(result.Message),
                201 => CreatedAtAction(nameof(GetTrackBySlug), new { id = result.Track.Id }, new { result.Track.Id, result.Track.Title, result.Track.Slug, result.Track.AudioFilePath }),
                _ => StatusCode(500, "An error occurred while creating the track")
            };
        }

        [HttpGet("hot")]
        public async Task<ActionResult<List<Track>>> GetHotTracks()
        {
            var tracks = await _trackService.GetHotTracks();
            return Ok(tracks);
        }

        [HttpPost("update-track-data/{id}")]
        public async Task<ActionResult> UpdateTrackData(UpdateTrackDataDTO dto, int id)
        {
            // Specific endpoint for updating track data made specifically for the analysis service
            // and shall not be used by clients (some protection?)

            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            var status = await _trackService.UpdateTrackData(dto, id);
            return status switch
            {
                UpdateTrackDataStatus.Success => Ok(),
                UpdateTrackDataStatus.TrackNotFound => NotFound()
            };
        }
    }
}

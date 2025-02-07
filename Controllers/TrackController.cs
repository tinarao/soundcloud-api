using Microsoft.AspNetCore.Mvc;
using Sounds_New.Models;

namespace Sounds_New.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrackController : ControllerBase
    {

        static private List<Track> tracks = [];

        [HttpGet]
        public ActionResult<List<Track>> GetTracks()
        {
            return Ok(tracks);
        }

        [HttpGet("{id}")]
        public ActionResult<Track> GetTrack(int id)
        {
            var track = tracks.FirstOrDefault(x => x.Id == id);
            if (track is null)
            {
                return NotFound();
            }

            return Ok(track);
        }

        [HttpPost]
        public ActionResult<Track> CreateTrack(Track track)
        {
            if (track is null)
            {
                return BadRequest();
            }

            track.Id = tracks.Max(t => t.Id) + 1;
            tracks.Add(track);

            return CreatedAtAction(nameof(GetTrack), new { id = track.Id }, track);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateTrack(int id, Track newTrackData)
        {
            var track = tracks.FirstOrDefault(t => t.Id == id);
            if (track is null)
            {
                return NotFound();
            }

            track.Title = newTrackData.Title;
            track.Slug = newTrackData.Slug;
            track.Likes = newTrackData.Likes;
            track.Listens = newTrackData.Listens;
            track.Downloads = newTrackData.Downloads;

            return NoContent();
        }
    
        [HttpDelete("{id}")]
        public IActionResult DeleteTrack(int id)
        {
            var track = tracks.FirstOrDefault(t => t.Id == id);
            if (track is null)
            {
                return NotFound();
            }

            tracks.Remove(track);
            return NoContent();
        }
    }
}

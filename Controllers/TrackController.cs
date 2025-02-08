using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sounds_New.Db;
using Sounds_New.DTO;
using Sounds_New.Models;

namespace Sounds_New.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrackController(SoundsContext context) : ControllerBase
    {
        private readonly SoundsContext _context = context;

        [HttpGet]
        public async Task<ActionResult<List<Track>>> GetTracks()
        {
            var tracks = await _context.Tracks.ToListAsync();
            return Ok(tracks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Track>> GetTrack(int id)
        {
            var track = await _context.Tracks.FindAsync(id);
            if (track is null)
            {
                return NotFound();
            }

            return Ok(track);
        }

        // [HttpPost]
        // public async Task<ActionResult<Track>> CreateTrack([FromForm] CreateTrackDTO dto)
        // {
        //     if (dto is null)
        //     {
        //         return BadRequest();
        //     }

        //     if (!ModelState.IsValid)
        //     {
        //         return UnprocessableEntity();
        //     }

        //     var track = new Track
        //     {
        //         Title = dto.Title,
        //         Slug = dto.Slug,
        //     };

        //     var created = await _context.Tracks.AddAsync(track);
        //     await _context.SaveChangesAsync();

            // return CreatedAtAction(nameof(GetTrack), new { id = track.Id }, track);
        // }

        // [HttpPut("{id}")]
        // public IActionResult UpdateTrack(int id, Track newTrackData)
        // {
        //     var track = tracks.FirstOrDefault(t => t.Id == id);
        //     if (track is null)
        //     {
        //         return NotFound();
        //     }

        //     track.Title = newTrackData.Title;
        //     track.Slug = newTrackData.Slug;
        //     track.Likes = newTrackData.Likes;
        //     track.Listens = newTrackData.Listens;
        //     track.Downloads = newTrackData.Downloads;

        //     return NoContent();
        // }
    
        // [HttpDelete("{id}")]
        // public IActionResult DeleteTrack(int id)
        // {
        //     var track = tracks.FirstOrDefault(t => t.Id == id);
        //     if (track is null)
        //     {
        //         return NotFound();
        //     }

        //     tracks.Remove(track);
        //     return NoContent();
        // }
    }
}

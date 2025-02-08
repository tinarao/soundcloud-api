using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Slugify;
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

        [Authorize]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<Track>> CreateTrack([FromForm] CreateTrackDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == User.Identity.Name);
            if (user == null)
            {
                return Unauthorized();
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var artworkFileName = $"{Guid.NewGuid()}_{dto.ArtworkFile.FileName}";
            var artworkFilePath = Path.Combine(uploadsFolder, artworkFileName);

            using (var stream = new FileStream(artworkFilePath, FileMode.Create))
            {
                await dto.ArtworkFile.CopyToAsync(stream);
            }

            var audioFileName = $"{Guid.NewGuid()}_{dto.AudioFile.FileName}";
            var audioFilePath = Path.Combine(uploadsFolder, audioFileName);

            using (var stream = new FileStream(audioFilePath, FileMode.Create))
            {
                await dto.AudioFile.CopyToAsync(stream);
            }

            var slug = new SlugHelper().GenerateSlug($"{dto.Title}-by-{user.Username}");

            var track = new Track
            {
                Title = dto.Title,
                Slug = slug,
                Genres = dto.Genres,
                ImageFilePath = artworkFilePath,
                AudioFilePath = audioFilePath,
                UserId = user.Id,
                User = user
            };

            var created = await _context.Tracks.AddAsync(track);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTrack), new { id = track.Id }, new { track.Id, track.Title, track.Slug, track.AudioFilePath });
        }

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

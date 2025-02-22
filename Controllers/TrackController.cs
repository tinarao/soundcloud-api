﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sounds_New.DTO;
using Sounds_New.Models;
using Sounds_New.Services.Tracks;
using Sounds_New.Utils;

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
            var ctxUserName = Utilites.GetIdentityUserName(HttpContext);
            var track = await _trackService.GetTrackBySlug(slug);
            if (track == null)
            {
                return NotFound();
            }

            if (!track.IsPublic && track.User.Username != ctxUserName)
            {
                return Forbid();
            }

            return Ok(track);
        }

        [HttpGet("hot")]
        public async Task<ActionResult<List<Track>>> GetHotTracks()
        {
            var tracks = await _trackService.GetHotTracks();
            return Ok(tracks);
        }

        [HttpGet("by-user/{userSlug}")]
        public async Task<IActionResult> GetTracksByUser(string userSlug)
        {
            var userCtx = Utilites.GetIdentityUserName(HttpContext);

            var tracks = await _trackService.GetTracksByUser(userSlug, userCtx);
            if (tracks == null)
            {
                return NotFound("Пользователь и/или треки не найдены");
            }

            return Ok(tracks);
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
                201 => CreatedAtAction(nameof(GetTrackBySlug), new { slug = result.Track.Slug }, new { result.Track.Title, result.Track.Slug }),
                _ => StatusCode(500, "An error occurred while creating the track")
            };
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
                UpdateTrackDataStatus.TrackNotFound => NotFound(),
                _ => StatusCode(500, "An error occurred while updating the track data")
            };
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTrack(int id)
        {
            if (User.Identity == null || User.Identity.Name == null)
            {
                return Forbid();
            }

            var ctxUser = User.Identity.Name;

            var result = await _trackService.DeleteTrack(id, ctxUser);
            if (result.IsOk)
            {
                return NoContent();
            }

            return result.StatusCode switch
            {
                401 => Unauthorized(result.Message),
                404 => NotFound(result.Message),
                403 => Forbid(result.Message),
                _ => StatusCode(500, "An error occurred while deleting the track")
            };
        }

        [Authorize]
        [HttpPatch("{slug}/primary")]
        public async Task<ActionResult> UpdateTrackPrimaryData(UpdateTrackPrimaryDataDTO dto, string slug)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            if (dto.Genres.Length > 5 && dto.Genres.Length < 1)
            {
                return BadRequest("Количество жанров должно быть от 1 до 5");
            }

            var ctxUsername = Utilites.GetIdentityUserName(HttpContext);
            if (ctxUsername == null)
            {
                return Forbid();
            }

            var result = await _trackService.UpdateTrackPrimaryData(dto, slug, ctxUsername);
            return result.StatusCode switch
            {
                200 => Ok(result.Message),
                401 => Unauthorized(result.Message),
                404 => NotFound(result.Message),
                403 => Forbid(result.Message),
                _ => StatusCode(500, "An error occurred while updating the track primary data")
            };
        }

        [Authorize]
        [HttpPatch("like/{slug}")]
        public async Task<ActionResult> ChangeTrackLikes(string slug)
        {
            var ctxUsername = Utilites.GetIdentityUserName(HttpContext);
            if (ctxUsername == null)
            {
                return Forbid();
            }

            var result = await _trackService.ChangeTrackLikes(slug, ctxUsername);

            return result.StatusCode switch
            {
                200 => Ok(),
                404 => NotFound(result.Message),
                _ => StatusCode(500, "An error occurred while changing the track likes")
            };
        }

        [Authorize]
        [HttpPatch("{slug}/visibility/{newIsPublic}")]
        public async Task<ActionResult> ChangeTrackVisibility(string slug, bool newIsPublic)
        {
            var ctxUserName = Utilites.GetIdentityUserName(HttpContext);
            if (ctxUserName == null)
            {
                return Forbid();
            }

            var result = await _trackService.ChangeTrackVisibility(slug, ctxUserName, newIsPublic);

            return result.StatusCode switch
            {
                204 => NoContent(),
                404 => NotFound(result.Message),
                403 => Forbid(result.Message),
                _ => StatusCode(500, "An error occurred while changing the track visibility")
            };
        }

        [HttpPatch("{slug}/listens")]
        public async Task<ActionResult> IncreaseTrackListens(string slug)
        {
            var result = await _trackService.IncreaseTrackListens(slug);
            return result.StatusCode switch
            {
                204 => NoContent(),
                404 => NotFound(result.Message),
                _ => StatusCode(500, "An error occurred while increasing the track listens")
            };
        }
    }
}

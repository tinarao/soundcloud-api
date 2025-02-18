using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sounds_New.DTO;
using Sounds_New.Services.Comments;
using Sounds_New.Utils;

namespace Sounds_New.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController(ICommentsService commentsService) : ControllerBase
    {

        private readonly ICommentsService _commentsService = commentsService;

        [HttpGet("by-id/{id}")]
        public async Task<ActionResult> GetCommentById(int id)
        {
            var comment = await _commentsService.GetCommentById(id);
            if (comment == null)
            {
                return NotFound();
            }

            return Ok(comment);
        }

        [HttpGet("by-track/{trackSlug}")]
        public async Task<ActionResult> GetCommentsByTrackId(string trackSlug)
        {
            var ctxUsername = Utilites.GetIdentityUserName(HttpContext);
            var comments = await _commentsService.GetCommentsByTrackSlug(trackSlug, ctxUsername);
            if (comments == null)
            {
                return NotFound();
            }

            return Ok(comments);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> CreateComment(CreateCommentDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var ctxUsername = Utilites.GetIdentityUserName(HttpContext);
            if (ctxUsername == null)
            {
                return Unauthorized();
            }

            var result = await _commentsService.CreateComment(dto, ctxUsername);

            return result.StatusCode switch
            {
                400 => BadRequest(result.Message),
                401 => Unauthorized(result.Message),
                201 => Created(),
                _ => StatusCode(500, "An error occurred while creating the comment")
            };
        }

    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sounds_New.Services.Subscriptions;

namespace Sounds_New.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class SubscriptionController(ISubscriptionsService subscriptionsService) : ControllerBase
    {
        private readonly ISubscriptionsService _subscriptionsService = subscriptionsService;

        [Authorize]
        [HttpPost("{id}")]
        public async Task<ActionResult> SubscribeTo(int id)
        {
            // Needs to be tested
            // https://learn.microsoft.com/en-us/aspnet/core/web-api/http-repl/?view=aspnetcore-9.0&tabs=windows

            if (User.Identity == null || User.Identity.Name == null)
            {
                return Unauthorized();
            }

            var result = await _subscriptionsService.SubscribeTo(id, User.Identity.Name);
            return result.StatusCode switch
            {
                404 => NotFound(result.Message),
                401 => Unauthorized(result.Message),
                400 => BadRequest(result.Message),
                204 => NoContent(),
                _ => StatusCode(500)
            };
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetSubscribersById(int id)
        {
            var subscribers = await _subscriptionsService.GetSubscribersByUserId(id);
            if (subscribers == null)
            {
                return NotFound();
            }

            return Ok(subscribers);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> UnsubscribeFrom(int id)
        {
            if (User.Identity == null || User.Identity.Name == null)
            {
                return Unauthorized();
            }

            var result = await _subscriptionsService.UnsubscribeFrom(id, User.Identity.Name);
            return result.StatusCode switch
            {
                404 => NotFound(),
                401 => Unauthorized(),
                400 => BadRequest(),
                204 => NoContent(),
                _ => StatusCode(500)
            };
        }
    }
}
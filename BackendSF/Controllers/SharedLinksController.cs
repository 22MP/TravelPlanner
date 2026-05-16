using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using TravelPlanner.Shared.DTOs.SharedLink;
using TravelPlanner.Shared.Interfaces;

namespace BackendSF.Controllers
{
    [ApiController]
    [Route("api/trips/{tripId}/shared-links")]
    [Authorize]
    public class SharedLinksController : ControllerBase
    {
        private readonly ISharedLinkServices _sharedLinkServices;

        public SharedLinksController()
        {
            _sharedLinkServices = ServiceProxy.Create<ISharedLinkServices>(
                new Uri("fabric:/TravelPlanner/TravelService"));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int tripId)
        {
            var links = await _sharedLinkServices.GetAllByTripAsync(tripId);
            return Ok(links);
        }

        [HttpPost]
        public async Task<IActionResult> Create(int tripId, [FromBody] CreateSharedLinkDto dto)
        {
            dto.TripId = tripId;
            var link = await _sharedLinkServices.CreateAsync(dto);
            if (link.Id == 0)
                return BadRequest(new { message = "Failed to create shared link." });
            return Ok(link);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int tripId, int id)
        {
            var result = await _sharedLinkServices.DeleteAsync(id);
            if (!result)
                return NotFound(new { message = "Shared link not found." });
            return NoContent();
        }

        [HttpGet("/api/shared/{token}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByToken(string token)
        {
            var link = await _sharedLinkServices.GetByTokenAsync(token);
            if (link.Id == 0)
                return NotFound(new { message = "Invalid or expired link." });
            return Ok(link);
        }

        [HttpGet("/api/shared/{token}/validate")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateToken(string token, [FromQuery] string accessType)
        {
            var isValid = await _sharedLinkServices.ValidateTokenAsync(token, accessType);
            return Ok(new { isValid });
        }
    }
}
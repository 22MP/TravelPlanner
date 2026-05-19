using BackendSF.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using TravelPlanner.Shared.DTOs.Activity;
using TravelPlanner.Shared.Interfaces;

namespace BackendSF.Controllers
{
    [ApiController]
    [Route("api/trips/{tripId}/activities")]
    [Authorize]
    [ServiceFilter(typeof(SharedLinkAccessFilter))]
    public class ActivitiesController : ControllerBase
    {
        private readonly IActivityServices _activityServices;

        public ActivitiesController()
        {
            _activityServices = ServiceProxy.Create<IActivityServices>(
                new Uri("fabric:/TravelPlanner/TravelService"));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int tripId)
        {
            var activities = await _activityServices.GetAllByTripAsync(tripId);
            return Ok(activities);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int tripId, int id)
        {
            var activity = await _activityServices.GetByIdAsync(id);
            if (activity.Id == 0)
                return NotFound(new { message = "Activity not found." });
            return Ok(activity);
        }

        [HttpPost]
        public async Task<IActionResult> Create(int tripId, [FromBody] CreateActivityDto dto)
        {
            dto.TripId = tripId;
            var activity = await _activityServices.CreateAsync(dto);
            if (activity.Id == 0)
                return BadRequest(new { message = "Invalid activity data." });
            return CreatedAtAction(nameof(GetById), new { tripId, id = activity.Id }, activity);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int tripId, int id, [FromBody] UpdateActivityDto dto)
        {
            var activity = await _activityServices.UpdateAsync(id, dto);
            if (activity.Id == 0)
                return BadRequest(new { message = "Invalid activity data or not found." });
            return Ok(activity);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int tripId, int id)
        {
            var result = await _activityServices.DeleteAsync(id);
            if (!result)
                return NotFound(new { message = "Activity not found." });
            return NoContent();
        }
    }
}
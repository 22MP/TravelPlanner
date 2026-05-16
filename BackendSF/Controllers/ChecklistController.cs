using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using TravelPlanner.Shared.DTOs.Checklist;
using TravelPlanner.Shared.Interfaces;

namespace BackendSF.Controllers
{
    [ApiController]
    [Route("api/trips/{tripId}/checklist")]
    [Authorize]
    public class ChecklistController : ControllerBase
    {
        private readonly IChecklistServices _checklistServices;

        public ChecklistController()
        {
            _checklistServices = ServiceProxy.Create<IChecklistServices>(
                new Uri("fabric:/TravelPlanner/TravelService"));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int tripId)
        {
            var items = await _checklistServices.GetAllByTripAsync(tripId);
            return Ok(items);
        }

        [HttpPost]
        public async Task<IActionResult> Create(int tripId, [FromBody] CreateChecklistItemDto dto)
        {
            dto.TripId = tripId;
            var item = await _checklistServices.CreateAsync(dto);
            if (item.Id == 0)
                return BadRequest(new { message = "Invalid checklist item data." });
            return Ok(item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int tripId, int id, [FromBody] UpdateChecklistItemDto dto)
        {
            var item = await _checklistServices.UpdateAsync(id, dto);
            if (item.Id == 0)
                return BadRequest(new { message = "Checklist item not found." });
            return Ok(item);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int tripId, int id)
        {
            var result = await _checklistServices.DeleteAsync(id);
            if (!result)
                return NotFound(new { message = "Checklist item not found." });
            return NoContent();
        }
    }
}
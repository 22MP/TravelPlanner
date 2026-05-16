using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System.Security.Claims;
using TravelPlanner.Shared.DTOs.Trip;
using TravelPlanner.Shared.Interfaces;

namespace BackendSF.Controllers
{
    [ApiController]
    [Route("api/trips")]
    [Authorize]
    public class TripsController : ControllerBase
    {
        private readonly ITripServices _tripServices;

        public TripsController()
        {
            _tripServices = ServiceProxy.Create<ITripServices>(
                new Uri("fabric:/TravelPlanner/TravelService"));
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var trips = await _tripServices.GetAllByUserAsync(GetUserId());
            return Ok(trips);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var trip = await _tripServices.GetByIdAsync(id, GetUserId());
            if (trip.Id == 0)
                return NotFound(new { message = "Trip not found." });
            return Ok(trip);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTripDto dto)
        {
            var trip = await _tripServices.CreateAsync(dto, GetUserId());
            if (trip.Id == 0)
                return BadRequest(new { message = "Invalid trip data." });
            return CreatedAtAction(nameof(GetById), new { id = trip.Id }, trip);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTripDto dto)
        {
            var trip = await _tripServices.UpdateAsync(id, dto, GetUserId());
            if (trip.Id == 0)
                return BadRequest(new { message = "Invalid trip data or trip not found." });
            return Ok(trip);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _tripServices.DeleteAsync(id, GetUserId());
            if (!result)
                return NotFound(new { message = "Trip not found." });
            return NoContent();
        }
    }
}
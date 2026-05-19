using BackendSF.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using TravelPlanner.Shared.DTOs.Destination;
using TravelPlanner.Shared.Interfaces;
using TravelPlanner.Shared.Models;

namespace BackendSF.Controllers
{
    [ApiController]
    [Route("api/trips/{tripId}/destinations")]
    [Authorize]
    [ServiceFilter(typeof(SharedLinkAccessFilter))]
    public class DestinationsController : ControllerBase
    {
        private readonly IDestinationServices _destinationServices;

        public DestinationsController()
        {
            _destinationServices = ServiceProxy.Create<IDestinationServices>(
                new Uri("fabric:/TravelPlanner/TravelService"));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int tripId)
        {
            var destinations = await _destinationServices.GetAllByTripAsync(tripId);
            return Ok(destinations);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int tripId, int id)
        {
            var destination = await _destinationServices.GetByIdAsync(id);
            if (destination.Id == 0)
                return NotFound(new { message = "Destination not found." });
            return Ok(destination);
        }

        [HttpPost]
        public async Task<IActionResult> Create(int tripId, [FromBody] CreateDestinationDto dto)
        {
            dto.TripId = tripId;
            var destination = await _destinationServices.CreateAsync(dto);
            if (destination.Id == 0)
                return BadRequest(new { message = "Invalid destination data." });
            return CreatedAtAction(nameof(GetById), new { tripId, id = destination.Id }, destination);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int tripId, int id, [FromBody] UpdateDestinationDto dto)
        {
            var destination = await _destinationServices.UpdateAsync(tripId,id, dto);
         
            if (destination.Id == 0)
                return BadRequest(new { message = "Invalid destination data or not found." });
            return Ok(destination);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int tripId, int id)
        {
            var result = await _destinationServices.DeleteAsync(tripId, id);
            if (!result)
                return NotFound(new { message = "Destination not found." });
            return NoContent();
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System.Security.Claims;
using TravelPlanner.Shared.DTOs.Trip;
using TravelPlanner.Shared.Interfaces;
using TravelPlanner.Shared.Models;

namespace BackendSF.Controllers
{
    [ApiController]
    [Route("api/trips")]
    [Authorize] 
    public class TripsController : ControllerBase
    {
        private readonly ITripServices _tripServices;
        private readonly IExpenseServices _expenseServices;

        public TripsController()
        {
            // ServiceProxy kreira proxy za komunikaciju sa TravelService
            
            _tripServices = ServiceProxy.Create<ITripServices>(
                new Uri("fabric:/TravelPlanner/TravelService"));
            _expenseServices = ServiceProxy.Create<IExpenseServices>(
                new Uri("fabric:/TravelPlanner/FinanceService"));
        }

        // Pomocna metoda — cita userId iz JWT tokena
        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        /// <summary>
        /// Dohvat svih putovanja ulogovanog korisnika
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var trips = await _tripServices.GetAllByUserAsync(GetUserId());
            return Ok(trips);
        }

        /// <summary>
        /// Dohvat jednog putovanja po ID-u
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var trip = await _tripServices.GetByIdAsync(id, GetUserId());
            if (trip.Id == 0)
                return NotFound(new { message = "Putovanje nije pronadjeno." });

            try
            {
                var summary = await _expenseServices.GetBudgetSummaryAsync(id, trip.Budget);
                trip.TotalExpenses = summary.TotalExpenses;
                trip.RemainingBudget = summary.RemainingBudget;
            }
            catch
            {
                // FinanceService unavailable — return trip without expense summary
                trip.TotalExpenses = 0;
                trip.RemainingBudget = trip.Budget;
            }

            return Ok(trip);
        }

        /// <summary>
        /// Kreiranje novog putovanja
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTripDto dto)
        {
            // ModelState provjera validira DataAnnotations iz CreateTripDto
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var trip = await _tripServices.CreateAsync(dto, GetUserId());

            // Servis vraca prazan TripDto (Id == 0) ako validacija nije prosla
            if (trip.Id == 0)
                return BadRequest(new { message = "Krajnji datum ne moze biti prije pocetnog, ili je budzet negativan." });

            // 201 Created sa Location headerom koji pokazuje na novi resurs
            return CreatedAtAction(nameof(GetById), new { id = trip.Id }, trip);
        }

        /// <summary>
        /// Izmjena postojeceg putovanja
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTripDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var trip = await _tripServices.UpdateAsync(id, dto, GetUserId());

            if (trip.Id == 0)
                return BadRequest(new { message = "Putovanje nije pronadjeno ili podaci nisu ispravni." });

            return Ok(trip);
        }

        /// <summary>
        /// Brisanje putovanja 
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _tripServices.DeleteAsync(id, GetUserId());

            if (!result)
                return NotFound(new { message = "Putovanje nije pronadjeno." });

            // 204 No Content — uspjesno obrisano, nema tijela odgovora
            return NoContent();
        }
    }
}
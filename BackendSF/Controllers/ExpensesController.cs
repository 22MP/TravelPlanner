using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using TravelPlanner.Shared.DTOs.Expense;
using TravelPlanner.Shared.Interfaces;

namespace BackendSF.Controllers
{
    [ApiController]
    [Route("api/trips/{tripId}/expenses")]
    [Authorize]
    public class ExpensesController : ControllerBase
    {
        private readonly IExpenseServices _expenseServices;
        private readonly ITripServices _tripProxy;

        public ExpensesController()
        {
            _expenseServices = ServiceProxy.Create<IExpenseServices>(
                new Uri("fabric:/TravelPlanner/FinanceService"));
            _tripProxy = ServiceProxy.Create<ITripServices>(
                new Uri("fabric:/TravelPlanner/TravelService"));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int tripId)
        {
            var expenses = await _expenseServices.GetAllByTripAsync(tripId);
            return Ok(expenses);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int tripId, int id)
        {
            var expense = await _expenseServices.GetByIdAsync(id);
            if (expense.Id == 0)
                return NotFound(new { message = "Expense not found." });
            return Ok(expense);
        }

        [HttpPost]
        public async Task<IActionResult> Create(int tripId, [FromBody] CreateExpenseDto dto)
        {
            dto.TripId = tripId;
            var expense = await _expenseServices.CreateAsync(dto);
            if (expense.Id == 0)
                return BadRequest(new { message = "Invalid expense data." });
            return CreatedAtAction(nameof(GetById), new { tripId, id = expense.Id }, expense);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int tripId, int id, [FromBody] UpdateExpenseDto dto)
        {
            var expense = await _expenseServices.UpdateAsync(id, dto);
            if (expense.Id == 0)
                return BadRequest(new { message = "Invalid expense data or not found." });
            return Ok(expense);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int tripId, int id)
        {
            var result = await _expenseServices.DeleteAsync(id);
            if (!result)
                return NotFound(new { message = "Expense not found." });
            return NoContent();
        }

        [HttpGet("total")]
        public async Task<IActionResult> GetTotal(int tripId)
        {
            var total = await _expenseServices.GetTotalExpensesAsync(tripId);
            return Ok(new { totalExpenses = total });
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetBudgetSummary(int tripId,int userId)
        {
            
            var trip = await _tripProxy.GetByIdAsync(tripId,userId);
            var summary = await _expenseServices.GetBudgetSummaryAsync(tripId, trip.Budget);

            return Ok(summary);
        }
    }
}
using Microsoft.ServiceFabric.Services.Remoting;
using TravelPlanner.Shared.DTOs.Expense;

namespace TravelPlanner.Shared.Interfaces
{
    public interface IExpenseServices : IService
    {
        Task<IEnumerable<ExpenseDto>> GetAllByTripAsync(int tripId);
        Task<ExpenseDto> GetByIdAsync(int id);
        Task<ExpenseDto> CreateAsync(CreateExpenseDto dto);
        Task<ExpenseDto> UpdateAsync(int id, UpdateExpenseDto dto);
        Task<bool> DeleteAsync(int id);
        Task<decimal> GetTotalExpensesAsync(int tripId);
        Task<decimal> GetRemainingBudgetAsync(int tripId, decimal budget);
    }
}
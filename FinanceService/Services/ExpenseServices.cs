using Microsoft.EntityFrameworkCore;
using TravelPlanner.Shared.DTOs.Expense;
using TravelPlanner.Shared.Interfaces;
using TravelPlanner.Shared.Models;
using FinanceService.Data;
using AutoMapper;

namespace FinanceService.Services
{
    public class ExpenseServices : IExpenseServices
    {
        private readonly FinanceDbContext _context;
        private readonly IMapper _mapper;

        public ExpenseServices(FinanceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ExpenseDto>> GetAllByTripAsync(int tripId)
        {
            var expenses = await _context.Expenses
                .Where(e => e.TripId == tripId)
                .ToListAsync();
            return _mapper.Map<IEnumerable<ExpenseDto>>(expenses);
        }

        public async Task<ExpenseDto> GetByIdAsync(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null)
                return new ExpenseDto();
            return _mapper.Map<ExpenseDto>(expense);
        }

        public async Task<ExpenseDto> CreateAsync(CreateExpenseDto dto)
        {
            if (dto.Amount < 0)
                return new ExpenseDto();

            var expense = _mapper.Map<Expense>(dto);
            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();
            return _mapper.Map<ExpenseDto>(expense);
        }

        public async Task<ExpenseDto> UpdateAsync(int id, UpdateExpenseDto dto)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null)
                return new ExpenseDto();

            if (dto.Amount < 0)
                return new ExpenseDto();

            _mapper.Map(dto, expense);
            await _context.SaveChangesAsync();
            return _mapper.Map<ExpenseDto>(expense);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null)
                return false;

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<decimal> GetTotalExpensesAsync(int tripId)
        {
            return await _context.Expenses
                .Where(e => e.TripId == tripId)
                .SumAsync(e => e.Amount);
        }

        public async Task<decimal> GetRemainingBudgetAsync(int tripId, decimal budget)
        {
            var total = await GetTotalExpensesAsync(tripId);
            return budget - total;
        }
    }
}
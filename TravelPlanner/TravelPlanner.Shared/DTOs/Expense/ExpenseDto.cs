using TravelPlanner.Shared.Models;

namespace TravelPlanner.Shared.DTOs.Expense
{
    public class ExpenseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ExpenseCategory Category { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;
        public int TripId { get; set; }
    }
}
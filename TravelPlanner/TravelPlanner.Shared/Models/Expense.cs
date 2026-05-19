namespace TravelPlanner.Shared.Models
{
    public enum ExpenseCategory
    {
        Transport,
        Accommodation,
        Food,
        Tickets,
        Shopping,
        Other
    }

    public class Expense
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
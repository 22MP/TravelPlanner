namespace TravelPlanner.Shared.Models
{
    public class ChecklistItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsCompleted { get; set; } = false;

        public int TripId { get; set; }
        public Trip Trip { get; set; } = null!;
    }
}
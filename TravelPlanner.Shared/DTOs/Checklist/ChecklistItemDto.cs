namespace TravelPlanner.Shared.DTOs.Checklist
{
    public class ChecklistItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public int TripId { get; set; }
    }
}
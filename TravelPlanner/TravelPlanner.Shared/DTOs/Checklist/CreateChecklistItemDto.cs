namespace TravelPlanner.Shared.DTOs.Checklist
{
    public class CreateChecklistItemDto
    {
        public string Name { get; set; } = string.Empty;
        public int TripId { get; set; }
    }
}
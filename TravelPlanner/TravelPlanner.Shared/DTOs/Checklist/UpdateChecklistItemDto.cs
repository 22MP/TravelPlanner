namespace TravelPlanner.Shared.DTOs.Checklist
{
    public class UpdateChecklistItemDto
    {
        public string Name { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
    }
}
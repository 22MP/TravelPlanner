namespace TravelPlanner.Shared.Models
{
    public enum ActivityStatus
    {
        Planned,
        Reserved,
        Completed,
        Cancelled
    }

    public class Activity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal EstimatedCost { get; set; }
        public ActivityStatus Status { get; set; } = ActivityStatus.Planned;

        public int TripId { get; set; }
        public Trip Trip { get; set; } = null!;
    }
}
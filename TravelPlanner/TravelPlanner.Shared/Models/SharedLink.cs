namespace TravelPlanner.Shared.Models
{
    public enum AccessType
    {
        View,
        Edit
    }

    public class SharedLink
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public AccessType AccessType { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int TripId { get; set; }
        public Trip Trip { get; set; } = null!;
    }
}
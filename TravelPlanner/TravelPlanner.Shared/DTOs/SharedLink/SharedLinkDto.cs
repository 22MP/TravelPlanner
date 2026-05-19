using TravelPlanner.Shared.Models;

namespace TravelPlanner.Shared.DTOs.SharedLink
{
    public class SharedLinkDto
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public AccessType AccessType { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TripId { get; set; }
    }
}
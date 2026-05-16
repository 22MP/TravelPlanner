using TravelPlanner.Shared.Models;

namespace TravelPlanner.Shared.DTOs.SharedLink
{
    public class CreateSharedLinkDto
    {
        public AccessType AccessType { get; set; }
        public int ExpiresInDays { get; set; } = 7;
        public int TripId { get; set; }
    }
}
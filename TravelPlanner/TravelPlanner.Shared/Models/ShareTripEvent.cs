using System.Runtime.Serialization;

namespace TravelPlanner.Shared.Models
{
    [DataContract]
    public class ShareTripEvent
    {
        [DataMember]
        public int TripId { get; set; }

        [DataMember]
        public string Token { get; set; } = string.Empty;

        [DataMember]
        public string AccessType { get; set; } = string.Empty;

        [DataMember]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
namespace TravelPlanner.Shared.Models
{
    public class Destination
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime ArrivalDate { get; set; }
        public DateTime DepartureDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;

        public int TripId { get; set; }
        public Trip Trip { get; set; } = null!;
    }
}
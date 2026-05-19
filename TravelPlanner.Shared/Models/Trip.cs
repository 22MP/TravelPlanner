namespace TravelPlanner.Shared.Models
{
    public class Trip
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Budget ne moze biti negativan — validacija je u TripServices
        public decimal Budget { get; set; }

        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Strani kljuc — kojim korisniku pripada putovanje
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // Navigaciona svojstva — brisanjem Trip-a brisu se i sve zavisne kolekcije
        // (kaskada se podesava u TravelDbContext)
        public ICollection<Destination> Destinations { get; set; } = new List<Destination>();
        public ICollection<Activity> Activities { get; set; } = new List<Activity>();
        public ICollection<ChecklistItem> ChecklistItems { get; set; } = new List<ChecklistItem>();
        public ICollection<SharedLink> SharedLinks { get; set; } = new List<SharedLink>();

        // Expenses su uklonjene iz Trip modela
        // Nalaze se u FinanceService i pristupa im se preko TripId
    }
}
namespace TravelPlanner.Shared.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "user";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
    }
}
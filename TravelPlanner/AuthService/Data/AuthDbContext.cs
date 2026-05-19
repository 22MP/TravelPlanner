using Microsoft.EntityFrameworkCore;
using TravelPlanner.Shared.Models;

namespace AuthService.Data
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Ignoriši entitete koji ne pripadaju ovoj bazi
            modelBuilder.Ignore<Trip>();
            modelBuilder.Ignore<Destination>();
            modelBuilder.Ignore<Activity>();
            modelBuilder.Ignore<Expense>();
            modelBuilder.Ignore<ChecklistItem>();
            modelBuilder.Ignore<SharedLink>();

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Email).IsRequired().HasMaxLength(255);
                entity.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(u => u.LastName).IsRequired().HasMaxLength(100);
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.Role).HasDefaultValue("user");
            });
        }
    }
}
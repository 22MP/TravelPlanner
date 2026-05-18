using Microsoft.EntityFrameworkCore;
using TravelPlanner.Shared.Models;

namespace TravelService.Data
{
    public class TravelDbContext : DbContext
    {
        public TravelDbContext(DbContextOptions<TravelDbContext> options) : base(options) { }

        public DbSet<Trip> Trips { get; set; }
        public DbSet<Destination> Destinations { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<ChecklistItem> ChecklistItems { get; set; }
        public DbSet<SharedLink> SharedLinks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Ignorisi entitete koji ne pripadaju ovoj bazi
            modelBuilder.Ignore<User>();

            modelBuilder.Entity<Trip>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Name).IsRequired().HasMaxLength(200);
                entity.Property(t => t.Budget).HasColumnType("decimal(18,2)");
                entity.HasMany(t => t.Destinations)
                      .WithOne(d => d.Trip)
                      .HasForeignKey(d => d.TripId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(t => t.Activities)
                      .WithOne(a => a.Trip)
                      .HasForeignKey(a => a.TripId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(t => t.ChecklistItems)
                      .WithOne(c => c.Trip)
                      .HasForeignKey(c => c.TripId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(t => t.SharedLinks)
                      .WithOne(s => s.Trip)
                      .HasForeignKey(s => s.TripId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(t => t.Expenses)
                      .WithOne()
                      .HasForeignKey(e => e.TripId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Activity>(entity =>
            {
                entity.Property(a => a.EstimatedCost).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Expense>(entity =>
            {
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Destination>(entity =>
            {
                entity.Property(d => d.Name).IsRequired().HasMaxLength(200);
            });
        }
    }
}
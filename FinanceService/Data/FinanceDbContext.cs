using Microsoft.EntityFrameworkCore;
using TravelPlanner.Shared.Models;

namespace FinanceService.Data
{
    public class FinanceDbContext : DbContext
    {
        public FinanceDbContext(DbContextOptions<FinanceDbContext> options) : base(options) { }

        public DbSet<Expense> Expenses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<User>();
            modelBuilder.Ignore<Destination>();
            modelBuilder.Ignore<Activity>();
            modelBuilder.Ignore<ChecklistItem>();
            modelBuilder.Ignore<SharedLink>();
            modelBuilder.Ignore<Trip>();

            modelBuilder.Entity<Expense>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            });
        }
    }
}
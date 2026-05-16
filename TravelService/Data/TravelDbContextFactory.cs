using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TravelService.Data
{
    public class TravelDbContextFactory : IDesignTimeDbContextFactory<TravelDbContext>
    {
        public TravelDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TravelDbContext>();
            optionsBuilder.UseSqlServer(
                "Server=DESKTOP-4CEGCEE\\SQLEXPRESS;Database=TravelPlannerTravel;Trusted_Connection=True;TrustServerCertificate=True;");
            return new TravelDbContext(optionsBuilder.Options);
        }
    }
}
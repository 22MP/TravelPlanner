using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FinanceService.Data
{
    public class FinanceDbContextFactory : IDesignTimeDbContextFactory<FinanceDbContext>
    {
        public FinanceDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<FinanceDbContext>();
            optionsBuilder.UseSqlServer("Server=DESKTOP-4CEGCEE\\SQLEXPRESS;Database=TravelPlannerFinance;Trusted_Connection=True;TrustServerCertificate=True;");

            return new FinanceDbContext(optionsBuilder.Options);
        }
    }
}
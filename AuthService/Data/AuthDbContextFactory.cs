using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AuthService.Data
{
    public class AuthDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
    {
        public AuthDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();
            optionsBuilder.UseSqlServer("Server=DESKTOP-4CEGCEE\\SQLEXPRESS;Database=TravelPlannerAuth;Trusted_Connection=True;TrustServerCertificate=True;");

            return new AuthDbContext(optionsBuilder.Options);
        }
    }
}
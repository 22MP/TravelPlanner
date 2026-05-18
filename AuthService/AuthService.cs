using System.Fabric;
using AuthService.Data;
using AuthService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using TravelPlanner.Shared.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService
{
    internal sealed class AuthService : StatelessService, IAuthService
    {
        private readonly IAuthService _authServiceImpl;

        public AuthService(StatelessServiceContext context)
            : base(context)
        {
            // Citanje konfiguracije iz Settings.xml 
            var configPackage = context.CodePackageActivationContext
                .GetConfigurationPackageObject("Config");

            var connectionString = configPackage.Settings
                .Sections["ConnectionStrings"]
                .Parameters["DefaultConnection"].Value;

            var jwtSecret = configPackage.Settings
                .Sections["JwtSettings"]
                .Parameters["Secret"].Value;

            var jwtIssuer = configPackage.Settings
                .Sections["JwtSettings"]
                .Parameters["Issuer"].Value;

            var jwtAudience = configPackage.Settings
                .Sections["JwtSettings"]
                .Parameters["Audience"].Value;

            var jwtConfig = new JwtConfig
            {
                Secret = jwtSecret,
                Issuer = jwtIssuer,
                Audience = jwtAudience
            };

            var services = new ServiceCollection();

            services.AddDbContext<AuthDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddAutoMapper(cfg => cfg.AddProfile<Mappings.AuthMappingProfile>());

            // JwtConfig se registruje kao singleton da bi AuthServices mogao da ga koristi
            services.AddSingleton(jwtConfig);

            services.AddScoped<IAuthService, AuthServices>();

            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });

            var provider = services.BuildServiceProvider();
            _authServiceImpl = provider.GetRequiredService<IAuthService>();
        }

        public Task<TravelPlanner.Shared.DTOs.Auth.AuthResponseDto> RegisterAsync(
            TravelPlanner.Shared.DTOs.Auth.RegisterRequestDto request)
            => _authServiceImpl.RegisterAsync(request);

        public Task<TravelPlanner.Shared.DTOs.Auth.AuthResponseDto> LoginAsync(
            TravelPlanner.Shared.DTOs.Auth.LoginRequestDto request)
            => _authServiceImpl.LoginAsync(request);

        public Task<IEnumerable<TravelPlanner.Shared.DTOs.Auth.UserDto>> GetAllUsersAsync()
            => _authServiceImpl.GetAllUsersAsync();

        public Task<bool> DeleteUserAsync(int userId)
            => _authServiceImpl.DeleteUserAsync(userId);

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
            => this.CreateServiceRemotingInstanceListeners();
    }
}
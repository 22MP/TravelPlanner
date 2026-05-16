using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using TravelService.Data;
using TravelService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using TravelPlanner.Shared.Interfaces;
using TravelPlanner.Shared.DTOs.Trip;
using TravelPlanner.Shared.DTOs.Destination;
using TravelPlanner.Shared.DTOs.Activity;
using TravelPlanner.Shared.DTOs.Checklist;
using TravelPlanner.Shared.DTOs.SharedLink;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace TravelService
{
    internal sealed class TravelService : StatelessService,
        ITripServices, IDestinationServices, IActivityServices,
        IChecklistServices, ISharedLinkServices
    {
        private readonly ITripServices _tripServices;
        private readonly IDestinationServices _destinationServices;
        private readonly IActivityServices _activityServices;
        private readonly IChecklistServices _checklistServices;
        private readonly ISharedLinkServices _sharedLinkServices;

        public TravelService(StatelessServiceContext context)
            : base(context)
        {
            var services = new ServiceCollection();
            services.AddDbContext<TravelDbContext>(options =>
                options.UseSqlServer(
                    "Server=DESKTOP-4CEGCEE\\SQLEXPRESS;Database=TravelPlannerTravel;Trusted_Connection=True;TrustServerCertificate=True;"));
            services.AddAutoMapper(cfg => cfg.AddProfile<Mappings.TravelMappingProfile>());
            services.AddScoped<ITripServices, TripServices>();
            services.AddScoped<IDestinationServices, DestinationServices>();
            services.AddScoped<IActivityServices, ActivityServices>();
            services.AddScoped<IChecklistServices, ChecklistServices>();
            services.AddScoped<ISharedLinkServices, SharedLinkServices>();
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });

            var provider = services.BuildServiceProvider();
            _tripServices = provider.GetRequiredService<ITripServices>();
            _destinationServices = provider.GetRequiredService<IDestinationServices>();
            _activityServices = provider.GetRequiredService<IActivityServices>();
            _checklistServices = provider.GetRequiredService<IChecklistServices>();
            _sharedLinkServices = provider.GetRequiredService<ISharedLinkServices>();
        }


        // ITripServices
        Task<IEnumerable<TripDto>> ITripServices.GetAllByUserAsync(int userId)
            => _tripServices.GetAllByUserAsync(userId);
        Task<TripDto> ITripServices.GetByIdAsync(int tripId, int userId)
            => _tripServices.GetByIdAsync(tripId, userId);
        Task<TripDto> ITripServices.CreateAsync(CreateTripDto dto, int userId)
            => _tripServices.CreateAsync(dto, userId);
        Task<TripDto> ITripServices.UpdateAsync(int tripId, UpdateTripDto dto, int userId)
            => _tripServices.UpdateAsync(tripId, dto, userId);
        Task<bool> ITripServices.DeleteAsync(int tripId, int userId)
            => _tripServices.DeleteAsync(tripId, userId);

        // IDestinationServices
        Task<IEnumerable<DestinationDto>> IDestinationServices.GetAllByTripAsync(int tripId)
            => _destinationServices.GetAllByTripAsync(tripId);
        Task<DestinationDto> IDestinationServices.GetByIdAsync(int id)
            => _destinationServices.GetByIdAsync(id);
        Task<DestinationDto> IDestinationServices.CreateAsync(CreateDestinationDto dto)
            => _destinationServices.CreateAsync(dto);
        Task<DestinationDto> IDestinationServices.UpdateAsync(int id, UpdateDestinationDto dto)
            => _destinationServices.UpdateAsync(id, dto);
        Task<bool> IDestinationServices.DeleteAsync(int id)
            => _destinationServices.DeleteAsync(id);

        // IActivityServices
        Task<IEnumerable<ActivityDto>> IActivityServices.GetAllByTripAsync(int tripId)
            => _activityServices.GetAllByTripAsync(tripId);
        Task<ActivityDto> IActivityServices.GetByIdAsync(int id)
            => _activityServices.GetByIdAsync(id);
        Task<ActivityDto> IActivityServices.CreateAsync(CreateActivityDto dto)
            => _activityServices.CreateAsync(dto);
        Task<ActivityDto> IActivityServices.UpdateAsync(int id, UpdateActivityDto dto)
            => _activityServices.UpdateAsync(id, dto);
        Task<bool> IActivityServices.DeleteAsync(int id)
            => _activityServices.DeleteAsync(id);

        // IChecklistServices
        Task<IEnumerable<ChecklistItemDto>> IChecklistServices.GetAllByTripAsync(int tripId)
            => _checklistServices.GetAllByTripAsync(tripId);
        Task<ChecklistItemDto> IChecklistServices.CreateAsync(CreateChecklistItemDto dto)
            => _checklistServices.CreateAsync(dto);
        Task<ChecklistItemDto> IChecklistServices.UpdateAsync(int id, UpdateChecklistItemDto dto)
            => _checklistServices.UpdateAsync(id, dto);
        Task<bool> IChecklistServices.DeleteAsync(int id)
            => _checklistServices.DeleteAsync(id);

        // ISharedLinkServices
        Task<SharedLinkDto> ISharedLinkServices.CreateAsync(CreateSharedLinkDto dto)
            => _sharedLinkServices.CreateAsync(dto);
        Task<SharedLinkDto> ISharedLinkServices.GetByTokenAsync(string token)
            => _sharedLinkServices.GetByTokenAsync(token);
        Task<IEnumerable<SharedLinkDto>> ISharedLinkServices.GetAllByTripAsync(int tripId)
            => _sharedLinkServices.GetAllByTripAsync(tripId);
        Task<bool> ISharedLinkServices.DeleteAsync(int id)
            => _sharedLinkServices.DeleteAsync(id);
        Task<bool> ISharedLinkServices.ValidateTokenAsync(string token, string requiredAccessType)
            => _sharedLinkServices.ValidateTokenAsync(token, requiredAccessType);
 

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
            => this.CreateServiceRemotingInstanceListeners();
    }
}
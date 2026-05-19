using AutoMapper;
using TravelPlanner.Shared.DTOs.Activity;
using TravelPlanner.Shared.DTOs.Checklist;
using TravelPlanner.Shared.DTOs.Destination;
using TravelPlanner.Shared.DTOs.SharedLink;
using TravelPlanner.Shared.DTOs.Trip;
using TravelPlanner.Shared.Models;

namespace TravelService.Mappings
{
    public class TravelMappingProfile : Profile
    {
        public TravelMappingProfile()
        {
            // Trip
            CreateMap<Trip, TripDto>();
            CreateMap<CreateTripDto, Trip>();
            CreateMap<UpdateTripDto, Trip>();

            // Destination
            CreateMap<Destination, DestinationDto>();
            CreateMap<CreateDestinationDto, Destination>();
            CreateMap<UpdateDestinationDto, Destination>();

            // Activity
            CreateMap<Activity, ActivityDto>();
            CreateMap<CreateActivityDto, Activity>();
            CreateMap<UpdateActivityDto, Activity>();

            // Checklist
            CreateMap<ChecklistItem, ChecklistItemDto>();
            CreateMap<CreateChecklistItemDto, ChecklistItem>();
            CreateMap<UpdateChecklistItemDto, ChecklistItem>();

            // SharedLink
            CreateMap<SharedLink, SharedLinkDto>();
            CreateMap<CreateSharedLinkDto, SharedLink>()
                .ForMember(dest => dest.Token, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ExpiresAt, opt => opt.Ignore());
        }
    }
}
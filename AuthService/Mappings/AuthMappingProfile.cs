using AutoMapper;
using TravelPlanner.Shared.DTOs.Auth;
using TravelPlanner.Shared.Models;

namespace AuthService.Mappings
{
    public class AuthMappingProfile : Profile
    {
        public AuthMappingProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<RegisterRequestDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
        }
    }
}
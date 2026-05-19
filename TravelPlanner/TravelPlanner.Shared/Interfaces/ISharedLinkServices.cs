using Microsoft.ServiceFabric.Services.Remoting;
using TravelPlanner.Shared.DTOs.SharedLink;

namespace TravelPlanner.Shared.Interfaces
{
    public interface ISharedLinkServices : IService
    {
        Task<SharedLinkDto> CreateAsync(CreateSharedLinkDto dto);
        Task<SharedLinkDto> GetByTokenAsync(string token);
        Task<IEnumerable<SharedLinkDto>> GetAllByTripAsync(int tripId);
        Task<bool> DeleteAsync(int id);
        Task<bool> ValidateTokenAsync(string token, string requiredAccessType);
    }
}
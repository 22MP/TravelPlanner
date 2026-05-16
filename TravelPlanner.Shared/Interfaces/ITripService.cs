using Microsoft.ServiceFabric.Services.Remoting;
using TravelPlanner.Shared.DTOs.Trip;

namespace TravelPlanner.Shared.Interfaces
{
    public interface ITripServices : IService
    {
        Task<IEnumerable<TripDto>> GetAllByUserAsync(int userId);
        Task<TripDto> GetByIdAsync(int tripId, int userId);
        Task<TripDto> CreateAsync(CreateTripDto dto, int userId);
        Task<TripDto> UpdateAsync(int tripId, UpdateTripDto dto, int userId);
        Task<bool> DeleteAsync(int tripId, int userId);
    }
}
using Microsoft.ServiceFabric.Services.Remoting;
using TravelPlanner.Shared.DTOs.Destination;

namespace TravelPlanner.Shared.Interfaces
{
    public interface IDestinationServices : IService
    {
        Task<IEnumerable<DestinationDto>> GetAllByTripAsync(int tripId);
        Task<DestinationDto> GetByIdAsync(int id);
        Task<DestinationDto> CreateAsync(CreateDestinationDto dto);
        Task<DestinationDto> UpdateAsync(int id, UpdateDestinationDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
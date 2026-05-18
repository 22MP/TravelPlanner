using Microsoft.ServiceFabric.Services.Remoting;
using TravelPlanner.Shared.DTOs.Destination;

namespace TravelPlanner.Shared.Interfaces
{
    public interface IDestinationServices : IService
    {
        Task<IEnumerable<DestinationDto>> GetAllByTripAsync(int tripId);
        Task<DestinationDto> GetByIdAsync(int id);
        Task<DestinationDto> CreateAsync(CreateDestinationDto dto);
        Task<DestinationDto> UpdateAsync(int tripId,int id, UpdateDestinationDto dto);
        Task<bool> DeleteAsync(int tripId,int id);
    }
}
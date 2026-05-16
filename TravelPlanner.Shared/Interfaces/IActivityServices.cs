using Microsoft.ServiceFabric.Services.Remoting;
using TravelPlanner.Shared.DTOs.Activity;

namespace TravelPlanner.Shared.Interfaces
{
    public interface IActivityServices : IService
    {
        Task<IEnumerable<ActivityDto>> GetAllByTripAsync(int tripId);
        Task<ActivityDto> GetByIdAsync(int id);
        Task<ActivityDto> CreateAsync(CreateActivityDto dto);
        Task<ActivityDto> UpdateAsync(int id, UpdateActivityDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
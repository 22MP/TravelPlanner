using Microsoft.ServiceFabric.Services.Remoting;
using TravelPlanner.Shared.DTOs.Checklist;

namespace TravelPlanner.Shared.Interfaces
{
    public interface IChecklistServices : IService
    {
        Task<IEnumerable<ChecklistItemDto>> GetAllByTripAsync(int tripId);
        Task<ChecklistItemDto> CreateAsync(CreateChecklistItemDto dto);
        Task<ChecklistItemDto> UpdateAsync(int id, UpdateChecklistItemDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
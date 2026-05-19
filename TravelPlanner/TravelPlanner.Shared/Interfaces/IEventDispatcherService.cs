using Microsoft.ServiceFabric.Services.Remoting;
using TravelPlanner.Shared.Models;

namespace TravelPlanner.Shared.Interfaces
{
    public interface IEventDispatcherService : IService
    {
        Task EnqueueShareTripEventAsync(ShareTripEvent tripEvent);
    }
}
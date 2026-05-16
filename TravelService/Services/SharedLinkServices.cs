using Microsoft.EntityFrameworkCore;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using TravelPlanner.Shared.DTOs.SharedLink;
using TravelPlanner.Shared.Interfaces;
using TravelPlanner.Shared.Models;
using TravelService.Data;

namespace TravelService.Services
{
    public class SharedLinkServices : ISharedLinkServices
    {
        private readonly TravelDbContext _context;

        public SharedLinkServices(TravelDbContext context)
        {
            _context = context;
        }

        public async Task<SharedLinkDto> CreateAsync(CreateSharedLinkDto dto)
        {
            var token = Guid.NewGuid().ToString("N");

            var sharedLink = new SharedLink
            {
                Token = token,
                AccessType = dto.AccessType,
                ExpiresAt = DateTime.UtcNow.AddDays(dto.ExpiresInDays),
                TripId = dto.TripId
            };

            _context.SharedLinks.Add(sharedLink);
            await _context.SaveChangesAsync();

            // Fire & Forget — ne cekamo rezultat
            var proxy = ServiceProxy.Create<IEventDispatcherService>(
                new Uri("fabric:/TravelPlanner/EventDispatcher"));

            _ = proxy.EnqueueShareTripEventAsync(new ShareTripEvent
            {
                TripId = dto.TripId,
                Token = token,
                AccessType = dto.AccessType.ToString(),
                CreatedAt = DateTime.UtcNow
            });

            // Odmah vracamo rezultat korisniku
            return new SharedLinkDto
            {
                Id = sharedLink.Id,
                Token = sharedLink.Token,
                AccessType = sharedLink.AccessType,
                ExpiresAt = sharedLink.ExpiresAt,
                CreatedAt = sharedLink.CreatedAt,
                TripId = sharedLink.TripId
            };
        }
        public async Task<SharedLinkDto> GetByTokenAsync(string token)
        {
            var link = await _context.SharedLinks
                .FirstOrDefaultAsync(s => s.Token == token);

            if (link == null)
                return new SharedLinkDto();

            return new SharedLinkDto
            {
                Id = link.Id,
                Token = link.Token,
                AccessType = link.AccessType,
                ExpiresAt = link.ExpiresAt,
                CreatedAt = link.CreatedAt,
                TripId = link.TripId
            };
        }

        public async Task<IEnumerable<SharedLinkDto>> GetAllByTripAsync(int tripId)
        {
            return await _context.SharedLinks
                .Where(s => s.TripId == tripId)
                .Select(s => new SharedLinkDto
                {
                    Id = s.Id,
                    Token = s.Token,
                    AccessType = s.AccessType,
                    ExpiresAt = s.ExpiresAt,
                    CreatedAt = s.CreatedAt,
                    TripId = s.TripId
                })
                .ToListAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var link = await _context.SharedLinks.FindAsync(id);
            if (link == null)
                return false;

            _context.SharedLinks.Remove(link);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ValidateTokenAsync(string token, string requiredAccessType)
        {
            var link = await _context.SharedLinks
                .FirstOrDefaultAsync(s => s.Token == token);

            if (link == null)
                return false;

            if (link.ExpiresAt < DateTime.UtcNow)
                return false;

            if (requiredAccessType == "EDIT" && link.AccessType == AccessType.View)
                return false;

            return true;
        }
    }
}
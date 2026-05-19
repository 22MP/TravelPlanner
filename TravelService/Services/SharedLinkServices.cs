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
        private readonly IDbContextFactory<TravelDbContext> _contextFactory;

        public SharedLinkServices(IDbContextFactory<TravelDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
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

            using var context = await _contextFactory.CreateDbContextAsync();
            context.SharedLinks.Add(sharedLink);
            await context.SaveChangesAsync();

            // Fire & Forget — ne cekamo rezultat
            _ = Task.Run(async () =>
            {
                try
                {
                    var proxy = ServiceProxy.Create<IEventDispatcherService>(
                        new Uri("fabric:/TravelPlanner/EventDispatcher"));

                    await proxy.EnqueueShareTripEventAsync(new ShareTripEvent
                    {
                        TripId = dto.TripId,
                        Token = token,
                        AccessType = dto.AccessType.ToString(),
                        CreatedAt = DateTime.UtcNow
                    });
                }
                catch { /* EventDispatcher nije dostupan, ignorisi */ }
            });

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
            using var context = await _contextFactory.CreateDbContextAsync();
            var link = await context.SharedLinks
                .FirstOrDefaultAsync(s => s.Token == token);

            if (link == null || link.ExpiresAt < DateTime.UtcNow)
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
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.SharedLinks
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
            using var context = await _contextFactory.CreateDbContextAsync();
            var link = await context.SharedLinks.FindAsync(id);
            if (link == null)
                return false;

            context.SharedLinks.Remove(link);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ValidateTokenAsync(string token, string requiredAccessType)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var link = await context.SharedLinks
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
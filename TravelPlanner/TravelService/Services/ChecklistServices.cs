using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TravelPlanner.Shared.DTOs.Checklist;
using TravelPlanner.Shared.Interfaces;
using TravelPlanner.Shared.Models;
using TravelService.Data;

namespace TravelService.Services
{
    public class ChecklistServices : IChecklistServices
    {
        private readonly IDbContextFactory<TravelDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public ChecklistServices(IDbContextFactory<TravelDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ChecklistItemDto>> GetAllByTripAsync(int tripId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var items = await context.ChecklistItems
                .Where(c => c.TripId == tripId)
                .ToListAsync();
            return _mapper.Map<IEnumerable<ChecklistItemDto>>(items);
        }

        public async Task<ChecklistItemDto> CreateAsync(CreateChecklistItemDto dto)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var tripExists = await context.Trips.AnyAsync(t => t.Id == dto.TripId);
            if (!tripExists)
                return new ChecklistItemDto();

            var item = _mapper.Map<ChecklistItem>(dto);
            context.ChecklistItems.Add(item);
            await context.SaveChangesAsync();
            return _mapper.Map<ChecklistItemDto>(item);
        }

        public async Task<ChecklistItemDto> UpdateAsync(int id, UpdateChecklistItemDto dto)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var item = await context.ChecklistItems.FindAsync(id);
            if (item == null)
                return new ChecklistItemDto();

            _mapper.Map(dto, item);
            await context.SaveChangesAsync();
            return _mapper.Map<ChecklistItemDto>(item);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var item = await context.ChecklistItems.FindAsync(id);
            if (item == null)
                return false;

            context.ChecklistItems.Remove(item);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
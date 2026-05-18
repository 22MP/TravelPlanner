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
        private readonly TravelDbContext _context;
        private readonly IMapper _mapper;

        public ChecklistServices(TravelDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ChecklistItemDto>> GetAllByTripAsync(int tripId)
        {
            var items = await _context.ChecklistItems
                .Where(c => c.TripId == tripId)
                .ToListAsync();
            return _mapper.Map<IEnumerable<ChecklistItemDto>>(items);
        }

        public async Task<ChecklistItemDto> CreateAsync(CreateChecklistItemDto dto)
        {
            var tripExists = await _context.Trips.AnyAsync(t => t.Id == dto.TripId);
            if (!tripExists)
                return new ChecklistItemDto();
            var item = _mapper.Map<ChecklistItem>(dto);
            _context.ChecklistItems.Add(item);
            await _context.SaveChangesAsync();
            return _mapper.Map<ChecklistItemDto>(item);
        }

        public async Task<ChecklistItemDto> UpdateAsync(int id, UpdateChecklistItemDto dto)
        {
            var item = await _context.ChecklistItems.FindAsync(id);
            if (item == null)
                return new ChecklistItemDto();

            _mapper.Map(dto, item);
            await _context.SaveChangesAsync();
            return _mapper.Map<ChecklistItemDto>(item);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var item = await _context.ChecklistItems.FindAsync(id);
            if (item == null)
                return false;

            _context.ChecklistItems.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
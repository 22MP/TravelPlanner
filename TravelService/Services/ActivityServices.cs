using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TravelPlanner.Shared.DTOs.Activity;
using TravelPlanner.Shared.Interfaces;
using TravelPlanner.Shared.Models;
using TravelService.Data;

namespace TravelService.Services
{
    public class ActivityServices : IActivityServices
    {
        private readonly TravelDbContext _context;
        private readonly IMapper _mapper;

        public ActivityServices(TravelDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ActivityDto>> GetAllByTripAsync(int tripId)
        {
            var activities = await _context.Activities
                .Where(a => a.TripId == tripId)
                .ToListAsync();
            return _mapper.Map<IEnumerable<ActivityDto>>(activities);
        }

        public async Task<ActivityDto> GetByIdAsync(int id)
        {
            var activity = await _context.Activities.FindAsync(id);
            if (activity == null)
                return new ActivityDto();
            return _mapper.Map<ActivityDto>(activity);
        }

        public async Task<ActivityDto> CreateAsync(CreateActivityDto dto)
        {
            if (dto.EstimatedCost < 0)
                return new ActivityDto();

            var activity = _mapper.Map<Activity>(dto);
            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();
            return _mapper.Map<ActivityDto>(activity);
        }

        public async Task<ActivityDto> UpdateAsync(int id, UpdateActivityDto dto)
        {
            var activity = await _context.Activities.FindAsync(id);
            if (activity == null)
                return new ActivityDto();

            if (dto.EstimatedCost < 0)
                return new ActivityDto();

            _mapper.Map(dto, activity);
            await _context.SaveChangesAsync();
            return _mapper.Map<ActivityDto>(activity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var activity = await _context.Activities.FindAsync(id);
            if (activity == null)
                return false;

            _context.Activities.Remove(activity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
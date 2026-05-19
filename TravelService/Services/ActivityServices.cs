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
        private readonly IDbContextFactory<TravelDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public ActivityServices(IDbContextFactory<TravelDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ActivityDto>> GetAllByTripAsync(int tripId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var activities = await context.Activities
                .Where(a => a.TripId == tripId)
                .ToListAsync();
            return _mapper.Map<IEnumerable<ActivityDto>>(activities);
        }

        public async Task<ActivityDto> GetByIdAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var activity = await context.Activities.FindAsync(id);
            if (activity == null)
                return new ActivityDto();
            return _mapper.Map<ActivityDto>(activity);
        }

        public async Task<ActivityDto> CreateAsync(CreateActivityDto dto)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var tripExists = await context.Trips.AnyAsync(t => t.Id == dto.TripId);
            if (!tripExists)
                return new ActivityDto();

            if (dto.EstimatedCost < 0)
                return new ActivityDto();

            var activity = _mapper.Map<Activity>(dto);
            context.Activities.Add(activity);
            await context.SaveChangesAsync();
            return _mapper.Map<ActivityDto>(activity);
        }

        public async Task<ActivityDto> UpdateAsync(int id, UpdateActivityDto dto)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var activity = await context.Activities.FindAsync(id);
            if (activity == null)
                return new ActivityDto();

            if (dto.EstimatedCost < 0)
                return new ActivityDto();

            _mapper.Map(dto, activity);
            await context.SaveChangesAsync();
            return _mapper.Map<ActivityDto>(activity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var activity = await context.Activities.FindAsync(id);
            if (activity == null)
                return false;

            context.Activities.Remove(activity);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
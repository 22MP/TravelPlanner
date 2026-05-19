using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TravelPlanner.Shared.DTOs.Trip;
using TravelPlanner.Shared.Interfaces;
using TravelPlanner.Shared.Models;
using TravelService.Data;

namespace TravelService.Services
{
    public class TripServices : ITripServices
    {
        private readonly IDbContextFactory<TravelDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public TripServices(IDbContextFactory<TravelDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TripDto>> GetAllByUserAsync(int userId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var trips = await context.Trips
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<TripDto>>(trips);
        }

        public async Task<TripDto> GetByIdAsync(int tripId, int userId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var trip = await context.Trips
                .FirstOrDefaultAsync(t => t.Id == tripId && t.UserId == userId);

            if (trip == null)
                return new TripDto();

            return _mapper.Map<TripDto>(trip);
        }

        public async Task<TripDto> CreateAsync(CreateTripDto dto, int userId)
        {
            if (dto.EndDate < dto.StartDate)
                return new TripDto();

            if (dto.Budget < 0)
                return new TripDto();

            using var context = await _contextFactory.CreateDbContextAsync();
            var trip = _mapper.Map<Trip>(dto);
            trip.UserId = userId;
            trip.CreatedAt = DateTime.UtcNow;

            context.Trips.Add(trip);
            await context.SaveChangesAsync();

            return _mapper.Map<TripDto>(trip);
        }

        public async Task<TripDto> UpdateAsync(int tripId, UpdateTripDto dto, int userId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var trip = await context.Trips
                .FirstOrDefaultAsync(t => t.Id == tripId && t.UserId == userId);

            if (trip == null)
                return new TripDto();

            if (dto.EndDate < dto.StartDate)
                return new TripDto();

            if (dto.Budget < 0)
                return new TripDto();

            _mapper.Map(dto, trip);
            await context.SaveChangesAsync();

            return _mapper.Map<TripDto>(trip);
        }

        public async Task<bool> DeleteAsync(int tripId, int userId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var trip = await context.Trips
                .FirstOrDefaultAsync(t => t.Id == tripId && t.UserId == userId);

            if (trip == null)
                return false;

            context.Trips.Remove(trip);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
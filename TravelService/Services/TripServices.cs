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
        private readonly TravelDbContext _context;
        private readonly IMapper _mapper;

        public TripServices(TravelDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TripDto>> GetAllByUserAsync(int userId)
        {
            var trips = await _context.Trips
                .Where(t => t.UserId == userId)
                .ToListAsync();
            return _mapper.Map<IEnumerable<TripDto>>(trips);
        }

        public async Task<TripDto> GetByIdAsync(int tripId, int userId)
        {
            var trip = await _context.Trips
                .FirstOrDefaultAsync(t => t.Id == tripId && t.UserId == userId);

            if (trip == null)
                return new TripDto();

            return _mapper.Map<TripDto>(trip);
        }

        public async Task<TripDto> CreateAsync(CreateTripDto dto, int userId)
        {
            if (dto.EndDate < dto.StartDate || dto.Budget < 0)
                return new TripDto();

            var trip = _mapper.Map<Trip>(dto);
            trip.UserId = userId;

            _context.Trips.Add(trip);
            await _context.SaveChangesAsync();

            return _mapper.Map<TripDto>(trip);
        }

        public async Task<TripDto> UpdateAsync(int tripId, UpdateTripDto dto, int userId)
        {
            var trip = await _context.Trips
                .FirstOrDefaultAsync(t => t.Id == tripId && t.UserId == userId);

            if (trip == null)
                return new TripDto();

            if (dto.EndDate < dto.StartDate || dto.Budget < 0)
                return new TripDto();

            _mapper.Map(dto, trip);
            await _context.SaveChangesAsync();

            return _mapper.Map<TripDto>(trip);
        }

        public async Task<bool> DeleteAsync(int tripId, int userId)
        {
            var trip = await _context.Trips
                .FirstOrDefaultAsync(t => t.Id == tripId && t.UserId == userId);

            if (trip == null)
                return false;

            _context.Trips.Remove(trip);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

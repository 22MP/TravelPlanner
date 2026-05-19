using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TravelPlanner.Shared.DTOs.Destination;
using TravelPlanner.Shared.Interfaces;
using TravelPlanner.Shared.Models;
using TravelService.Data;

namespace TravelService.Services
{
    public class DestinationServices : IDestinationServices
    {
        private readonly IDbContextFactory<TravelDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public DestinationServices(IDbContextFactory<TravelDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DestinationDto>> GetAllByTripAsync(int tripId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var destinations = await context.Destinations
                .Where(d => d.TripId == tripId)
                .ToListAsync();
            return _mapper.Map<IEnumerable<DestinationDto>>(destinations);
        }

        public async Task<DestinationDto> GetByIdAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var destination = await context.Destinations.FindAsync(id);
            if (destination == null)
                return new DestinationDto();
            return _mapper.Map<DestinationDto>(destination);
        }

        public async Task<DestinationDto> CreateAsync(CreateDestinationDto dto)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var tripExists = await context.Trips.AnyAsync(t => t.Id == dto.TripId);
            if (!tripExists)
                return new DestinationDto();

            if (dto.DepartureDate <= dto.ArrivalDate)
                return new DestinationDto();

            var destination = _mapper.Map<Destination>(dto);
            context.Destinations.Add(destination);
            await context.SaveChangesAsync();
            return _mapper.Map<DestinationDto>(destination);
        }

        public async Task<DestinationDto> UpdateAsync(int tripId, int id, UpdateDestinationDto dto)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var destination = await context.Destinations.FindAsync(id);
            if (destination == null)
                return new DestinationDto();

            if (destination.TripId != tripId)
                return new DestinationDto();

            if (dto.DepartureDate < dto.ArrivalDate)
                return new DestinationDto();

            _mapper.Map(dto, destination);
            await context.SaveChangesAsync();
            return _mapper.Map<DestinationDto>(destination);
        }

        public async Task<bool> DeleteAsync(int tripId, int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var destination = await context.Destinations.FindAsync(id);
            if (destination == null)
                return false;

            if (destination.TripId != tripId)
                return false;

            context.Destinations.Remove(destination);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
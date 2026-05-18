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
        private readonly TravelDbContext _context;
        private readonly IMapper _mapper;

        public DestinationServices(TravelDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DestinationDto>> GetAllByTripAsync(int tripId)
        {
            var destinations = await _context.Destinations
                .Where(d => d.TripId == tripId)
                .ToListAsync();
            return _mapper.Map<IEnumerable<DestinationDto>>(destinations);
        }

        public async Task<DestinationDto> GetByIdAsync(int id)
        {
            var destination = await _context.Destinations.FindAsync(id);
            if (destination == null)
                return new DestinationDto();
            return _mapper.Map<DestinationDto>(destination);
        }

        public async Task<DestinationDto> CreateAsync(CreateDestinationDto dto)
        {
            var tripExists = await _context.Trips.AnyAsync(t => t.Id == dto.TripId);
            if (!tripExists)
                return new DestinationDto();

            if (dto.DepartureDate <= dto.ArrivalDate)
                return new DestinationDto();

            var destination = _mapper.Map<Destination>(dto);
            _context.Destinations.Add(destination);
            await _context.SaveChangesAsync();
            return _mapper.Map<DestinationDto>(destination);
        }

        public async Task<DestinationDto> UpdateAsync(int tripId, int id, UpdateDestinationDto dto)
        {
            var destination = await _context.Destinations.FindAsync(id);
            if (destination == null)
                return new DestinationDto();
            if (destination.TripId != tripId)
            {
                return new DestinationDto();
            }
            if (dto.DepartureDate < dto.ArrivalDate)
                return new DestinationDto();

            _mapper.Map(dto, destination);
            await _context.SaveChangesAsync();
            return _mapper.Map<DestinationDto>(destination);
        }

        public async Task<bool> DeleteAsync(int tripId, int id)
        {
            var destination = await _context.Destinations.FindAsync(id);
            if (destination == null)
                return false;
            if(destination.TripId != tripId)
            {
                return false;
            }
            _context.Destinations.Remove(destination);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
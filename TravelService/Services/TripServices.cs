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

        // Dohvat svih putovanja za odredjenog korisnika
        public async Task<IEnumerable<TripDto>> GetAllByUserAsync(int userId)
        {
            var trips = await _context.Trips
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<TripDto>>(trips);
        }

        // Dohvat jednog putovanja — provjera da pripada tom korisniku
        public async Task<TripDto> GetByIdAsync(int tripId, int userId)
        {
            var trip = await _context.Trips
                .FirstOrDefaultAsync(t => t.Id == tripId && t.UserId == userId);

            if (trip == null)
                return new TripDto();

            return _mapper.Map<TripDto>(trip);
        }

        // Kreiranje novog putovanja
        public async Task<TripDto> CreateAsync(CreateTripDto dto, int userId)
        {
            // Validacija: krajnji datum ne moze biti prije pocetnog
            if (dto.EndDate < dto.StartDate)
                return new TripDto();

            // Validacija: budzet ne moze biti negativan
            if (dto.Budget < 0)
                return new TripDto();

            var trip = _mapper.Map<Trip>(dto);
            trip.UserId = userId;
            trip.CreatedAt = DateTime.UtcNow;

            _context.Trips.Add(trip);
            await _context.SaveChangesAsync();

            return _mapper.Map<TripDto>(trip);
        }

        // Izmjena postojeceg putovanja
        public async Task<TripDto> UpdateAsync(int tripId, UpdateTripDto dto, int userId)
        {
            // Provjera da putovanje postoji i da pripada korisniku
            var trip = await _context.Trips
                .FirstOrDefaultAsync(t => t.Id == tripId && t.UserId == userId);

            if (trip == null)
                return new TripDto();

            // Validacija datuma i budzeta
            if (dto.EndDate < dto.StartDate)
                return new TripDto();

            if (dto.Budget < 0)
                return new TripDto();

            // AutoMapper prepisuje vrijednosti iz dto u postojeci objekat
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
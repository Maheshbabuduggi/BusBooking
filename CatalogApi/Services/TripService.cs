// TripService.cs
using Microsoft.EntityFrameworkCore;
using CatalogApi.Data;
using CatalogApi.Data.Entities;
using CatalogApi.DTOs;

namespace CatalogApi.Services;

public class TripService : ITripService
{
    private readonly CatalogDbContext _db;

    public TripService(CatalogDbContext db) => _db = db;

    public async Task<TripResponseDto> CreateTripAsync(CreateTripDto dto, CancellationToken ct = default)
    {
        var bus = await _db.Buses.FindAsync(new object[] { dto.BusId }, ct)
            ?? throw new InvalidOperationException($"Bus {dto.BusId} not found.");
        var route = await _db.Routes.FindAsync(new object[] { dto.RouteId }, ct)
            ?? throw new InvalidOperationException($"Route {dto.RouteId} not found.");

        var trip = new Trip
        {
            Id = Guid.NewGuid(),
            RouteId = dto.RouteId,
            BusId = dto.BusId,
            DepartureTime = dto.DepartureTime,
            ArrivalTime = dto.ArrivalTime,
            FarePerSeat = dto.FarePerSeat,
            Status = "Scheduled",
            CreatedAt = DateTime.UtcNow
        };
        _db.Trips.Add(trip);

        for (int i = 1; i <= bus.TotalSeats; i++)
        {
            _db.Seats.Add(new Seat
            {
                Id = Guid.NewGuid(),
                TripId = trip.Id,
                SeatNumber = $"S{i}",
                Status = "Available"
            });
        }

        await _db.SaveChangesAsync(ct);
        return await GetTripByIdAsync(trip.Id, ct) ?? throw new InvalidOperationException("Trip creation failed unexpectedly.");
    }

    public async Task<TripResponseDto?> GetTripByIdAsync(Guid id, CancellationToken ct = default)
    {
        var trip = await _db.Trips.AsNoTracking()
            .Include(t => t.Route).Include(t => t.Bus).Include(t => t.Seats)
            .FirstOrDefaultAsync(t => t.Id == id, ct);

        return trip is null ? null : ToDto(trip);
    }

    public async Task<IEnumerable<TripResponseDto>> SearchTripsAsync(string? origin, string? destination, DateTime? date, CancellationToken ct = default)
    {
        var query = _db.Trips.AsNoTracking()
            .Include(t => t.Route).Include(t => t.Bus).Include(t => t.Seats)
            .Where(t => t.Status == "Scheduled");

        if (!string.IsNullOrWhiteSpace(origin))
            query = query.Where(t => t.Route!.Origin == origin);
        if (!string.IsNullOrWhiteSpace(destination))
            query = query.Where(t => t.Route!.Destination == destination);
        if (date.HasValue)
            query = query.Where(t => t.DepartureTime.Date == date.Value.Date);

        var trips = await query.OrderBy(t => t.DepartureTime).ToListAsync(ct);
        return trips.Select(ToDto);
    }

    private static TripResponseDto ToDto(Trip t) => new()
    {
        Id = t.Id,
        RouteId = t.RouteId,
        BusId = t.BusId,
        Origin = t.Route?.Origin ?? string.Empty,
        Destination = t.Route?.Destination ?? string.Empty,
        DepartureTime = t.DepartureTime,
        ArrivalTime = t.ArrivalTime,
        FarePerSeat = t.FarePerSeat,
        Status = t.Status,
        TotalSeats = t.Seats.Count,
        AvailableSeats = t.Seats.Count(s => s.Status == "Available")
    };
}

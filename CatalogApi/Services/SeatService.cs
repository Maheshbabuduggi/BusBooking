// SeatService.cs
using Microsoft.EntityFrameworkCore;
using CatalogApi.Data;
using CatalogApi.DTOs;

namespace CatalogApi.Services;

public class SeatService : ISeatService
{
    private readonly CatalogDbContext _db;
    private readonly ILogger<SeatService> _logger;

    public SeatService(CatalogDbContext db, ILogger<SeatService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IEnumerable<SeatResponseDto>> GetSeatsForTripAsync(Guid tripId, CancellationToken ct = default)
    {
        var seats = await _db.Seats.AsNoTracking()
            .Where(s => s.TripId == tripId)
            .OrderBy(s => s.SeatNumber)
            .ToListAsync(ct);

        return seats.Select(s => new SeatResponseDto { Id = s.Id, SeatNumber = s.SeatNumber, Status = s.Status });
    }

    public async Task<SeatActionResultDto> HoldSeatsAsync(Guid tripId, List<string> seatNumbers, CancellationToken ct = default)
    {
        var seats = await _db.Seats.Where(s => s.TripId == tripId && seatNumbers.Contains(s.SeatNumber)).ToListAsync(ct);

        if (seats.Count != seatNumbers.Count)
            return new SeatActionResultDto { Success = false, Message = "One or more seat numbers do not exist on this trip." };

        if (seats.Any(s => s.Status != "Available"))
            return new SeatActionResultDto { Success = false, Message = "One or more requested seats are no longer available." };

        foreach (var seat in seats)
        {
            seat.Status = "Held";
            seat.HeldAt = DateTime.UtcNow;
        }
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Held {Count} seats on trip {TripId}", seats.Count, tripId);
        return new SeatActionResultDto { Success = true, Message = "Seats held." };
    }

    public async Task<SeatActionResultDto> ConfirmSeatsAsync(Guid tripId, List<string> seatNumbers, CancellationToken ct = default)
    {
        var seats = await _db.Seats.Where(s => s.TripId == tripId && seatNumbers.Contains(s.SeatNumber)).ToListAsync(ct);

        foreach (var seat in seats)
        {
            seat.Status = "Booked";
        }
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Confirmed {Count} seats as Booked on trip {TripId}", seats.Count, tripId);
        return new SeatActionResultDto { Success = true, Message = "Seats confirmed as booked." };
    }

    public async Task<SeatActionResultDto> ReleaseSeatsAsync(Guid tripId, List<string> seatNumbers, CancellationToken ct = default)
    {
        var seats = await _db.Seats.Where(s => s.TripId == tripId && seatNumbers.Contains(s.SeatNumber)).ToListAsync(ct);

        foreach (var seat in seats)
        {
            seat.Status = "Available";
            seat.HeldAt = null;
        }
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Released {Count} seats back to Available on trip {TripId}", seats.Count, tripId);
        return new SeatActionResultDto { Success = true, Message = "Seats released." };
    }
}

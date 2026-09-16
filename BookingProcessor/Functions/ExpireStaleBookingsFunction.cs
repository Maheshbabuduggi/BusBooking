using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BookingProcessor.Data;
using BookingProcessor.Services;

namespace BookingProcessor.Functions;

public class ExpireStaleBookingsFunction
{
    private const int ExpiryThresholdMinutes = 15;

    private readonly BookingDbContext _dbContext;
    private readonly ICatalogApiClient _catalogClient;
    private readonly ILogger<ExpireStaleBookingsFunction> _logger;

    public ExpireStaleBookingsFunction(BookingDbContext dbContext, ICatalogApiClient catalogClient, ILogger<ExpireStaleBookingsFunction> logger)
    {
        _dbContext = dbContext;
        _catalogClient = catalogClient;
        _logger = logger;
    }

    [Function("ExpireStaleBookingsFunction")]
    public async Task RunAsync([TimerTrigger("0 */5 * * * *")] TimerInfo timer)
    {
        var cutoff = DateTime.UtcNow.AddMinutes(-ExpiryThresholdMinutes);

        var stalePendingBookings = await _dbContext.Bookings
            .Where(b => b.Status == "Pending" && b.CreatedAt < cutoff)
            .ToListAsync();

        if (stalePendingBookings.Count == 0)
        {
            _logger.LogInformation("No stale pending bookings found.");
            return;
        }

        foreach (var booking in stalePendingBookings)
        {
            var seatNumbers = await _dbContext.BookingSeats
                .Where(bs => bs.BookingId == booking.Id)
                .Select(bs => bs.SeatNumber)
                .ToListAsync();

            booking.Status = "Expired";
            await _catalogClient.ReleaseSeatsAsync(booking.TripId, seatNumbers);

            _logger.LogInformation("Booking {BookingId} expired after {Minutes} min, seats released on trip {TripId}",
                booking.Id, ExpiryThresholdMinutes, booking.TripId);
        }

        await _dbContext.SaveChangesAsync();
    }
}

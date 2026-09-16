using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BookingProcessor.Data;
using BookingProcessor.Messaging;
using BookingProcessor.Services;

namespace BookingProcessor.Functions;

public class BookingConfirmedFunction
{
    private readonly BookingDbContext _dbContext;
    private readonly ICatalogApiClient _catalogClient;
    private readonly ILogger<BookingConfirmedFunction> _logger;
    private static readonly Random _random = new();

    public BookingConfirmedFunction(BookingDbContext dbContext, ICatalogApiClient catalogClient, ILogger<BookingConfirmedFunction> logger)
    {
        _dbContext = dbContext;
        _catalogClient = catalogClient;
        _logger = logger;
    }

    [Function("BookingConfirmedFunction")]
    public async Task RunAsync(
        [EventHubTrigger("bookings", Connection = "EventHubConnection", ConsumerGroup = "$Default")]
        string[] events)
    {
        foreach (var raw in events)
        {
            BookingCreatedEvent? bookingEvent;
            try { bookingEvent = JsonSerializer.Deserialize<BookingCreatedEvent>(raw); }
            catch (JsonException ex) { _logger.LogError(ex, "Failed to deserialize BookingCreated payload."); continue; }

            if (bookingEvent is null) continue;

            var booking = await _dbContext.Bookings.FirstOrDefaultAsync(b => b.Id == bookingEvent.BookingId);
            if (booking is null)
            {
                _logger.LogWarning("Booking {BookingId} not found — skipping.", bookingEvent.BookingId);
                continue;
            }

            if (booking.Status != "Pending")
            {
                _logger.LogInformation("Booking {BookingId} already {Status} — skipping (likely reprocessed message).", booking.Id, booking.Status);
                continue;
            }

            var payment = await _dbContext.Payments.FirstOrDefaultAsync(p => p.BookingId == booking.Id);

            // Simulated payment gateway call — replace with a real integration later.
            await Task.Delay(TimeSpan.FromMilliseconds(500));
            var paymentSucceeded = _random.Next(1, 101) <= 90;

            if (paymentSucceeded)
            {
                booking.Status = "Confirmed";
                booking.ConfirmedAt = DateTime.UtcNow;
                if (payment is not null)
                {
                    payment.Status = "Success";
                    payment.TransactionRef = Guid.NewGuid().ToString("N");
                    payment.ProcessedAt = DateTime.UtcNow;
                }

                await _catalogClient.ConfirmSeatsAsync(bookingEvent.TripId, bookingEvent.SeatNumbers);
                _logger.LogInformation("Booking {BookingId} Confirmed, seats booked on trip {TripId}", booking.Id, bookingEvent.TripId);
            }
            else
            {
                booking.Status = "PaymentFailed";
                if (payment is not null)
                {
                    payment.Status = "Failed";
                    payment.ProcessedAt = DateTime.UtcNow;
                }

                await _catalogClient.ReleaseSeatsAsync(bookingEvent.TripId, bookingEvent.SeatNumbers);
                _logger.LogInformation("Booking {BookingId} PaymentFailed, seats released on trip {TripId}", booking.Id, bookingEvent.TripId);
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}

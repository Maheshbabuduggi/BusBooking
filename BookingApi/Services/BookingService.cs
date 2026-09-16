using Microsoft.EntityFrameworkCore;
using BookingApi.Data;
using BookingApi.Data.Entities;
using BookingApi.DTOs;
using BookingApi.Messaging;

namespace BookingApi.Services;

public class BookingService : IBookingService
{
    private readonly BookingDbContext _db;
    private readonly ICatalogApiClient _catalogClient;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<BookingService> _logger;

    public BookingService(BookingDbContext db, ICatalogApiClient catalogClient, IEventPublisher eventPublisher, ILogger<BookingService> logger)
    {
        _db = db;
        _catalogClient = catalogClient;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task<(bool Success, string? Error, BookingResponseDto? Booking)> CreateBookingAsync(CreateBookingDto dto, CancellationToken ct = default)
    {
        var trip = await _catalogClient.GetTripAsync(dto.TripId, ct);
        if (trip is null) return (false, "Trip not found.", null);
        if (trip.Status != "Scheduled") return (false, $"Trip is not open for booking (status: {trip.Status}).", null);

        var seats = await _catalogClient.GetSeatsAsync(dto.TripId, ct);
        var requestedSeats = seats.Where(s => dto.SeatNumbers.Contains(s.SeatNumber)).ToList();

        if (requestedSeats.Count != dto.SeatNumbers.Count)
            return (false, "One or more seat numbers do not exist on this trip.", null);
        if (requestedSeats.Any(s => s.Status != "Available"))
            return (false, "One or more requested seats are no longer available.", null);

        var holdResult = await _catalogClient.HoldSeatsAsync(dto.TripId, dto.SeatNumbers, ct);
        if (!holdResult.Success)
            return (false, holdResult.Message, null);

        var totalFare = trip.FarePerSeat * dto.SeatNumbers.Count;

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            TripId = dto.TripId,
            CustomerName = dto.CustomerName,
            CustomerEmail = dto.CustomerEmail,
            CustomerPhone = dto.CustomerPhone,
            NumberOfSeats = dto.SeatNumbers.Count,
            TotalFare = totalFare,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };
        _db.Bookings.Add(booking);

        foreach (var seatNumber in dto.SeatNumbers)
            _db.BookingSeats.Add(new BookingSeat { Id = Guid.NewGuid(), BookingId = booking.Id, SeatNumber = seatNumber });

        _db.Payments.Add(new Payment
        {
            Id = Guid.NewGuid(),
            BookingId = booking.Id,
            Amount = totalFare,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Booking {BookingId} created (Pending), seats held on trip {TripId}", booking.Id, dto.TripId);

        await _eventPublisher.PublishBookingCreatedAsync(new BookingCreatedEvent
        {
            BookingId = booking.Id,
            TripId = booking.TripId,
            SeatNumbers = dto.SeatNumbers,
            Amount = totalFare,
            CreatedAt = booking.CreatedAt
        }, ct);

        return (true, null, ToDto(booking, dto.SeatNumbers));
    }

    public async Task<BookingResponseDto?> GetBookingByIdAsync(Guid id, CancellationToken ct = default)
    {
        var booking = await _db.Bookings.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id, ct);
        if (booking is null) return null;

        var seatNumbers = await _db.BookingSeats.AsNoTracking()
            .Where(bs => bs.BookingId == id).Select(bs => bs.SeatNumber).ToListAsync(ct);

        return ToDto(booking, seatNumbers);
    }

    private static BookingResponseDto ToDto(Booking b, List<string> seatNumbers) => new()
    {
        Id = b.Id, TripId = b.TripId, SeatNumbers = seatNumbers, CustomerName = b.CustomerName,
        NumberOfSeats = b.NumberOfSeats, TotalFare = b.TotalFare, Status = b.Status,
        CreatedAt = b.CreatedAt, ConfirmedAt = b.ConfirmedAt
    };
}

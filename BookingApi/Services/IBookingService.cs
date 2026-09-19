using BookingApi.DTOs;

namespace BookingApi.Services;

public interface IBookingService
{
    Task<(bool Success, string? Error, BookingResponseDto? Booking)> CreateBookingAsync(CreateBookingDto dto, CancellationToken ct = default);
    Task<BookingResponseDto?> GetBookingByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<BookingResponseDto>> GetAllBookingsAsync(CancellationToken ct = default);
}

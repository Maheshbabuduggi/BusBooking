using BookingApi.DTOs;

namespace BookingApi.Services;

public interface ICatalogApiClient
{
    Task<TripInfoDto?> GetTripAsync(Guid tripId, CancellationToken ct = default);
    Task<List<SeatInfoDto>> GetSeatsAsync(Guid tripId, CancellationToken ct = default);
    Task<SeatActionResultDto> HoldSeatsAsync(Guid tripId, List<string> seatNumbers, CancellationToken ct = default);
}

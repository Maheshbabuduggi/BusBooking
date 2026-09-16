// ISeatService.cs
using CatalogApi.DTOs;

namespace CatalogApi.Services;

public interface ISeatService
{
    Task<IEnumerable<SeatResponseDto>> GetSeatsForTripAsync(Guid tripId, CancellationToken ct = default);
    Task<SeatActionResultDto> HoldSeatsAsync(Guid tripId, List<string> seatNumbers, CancellationToken ct = default);
    Task<SeatActionResultDto> ConfirmSeatsAsync(Guid tripId, List<string> seatNumbers, CancellationToken ct = default);
    Task<SeatActionResultDto> ReleaseSeatsAsync(Guid tripId, List<string> seatNumbers, CancellationToken ct = default);
}

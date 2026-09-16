// ICatalogApiClient.cs
namespace BookingProcessor.Services;

public interface ICatalogApiClient
{
    Task ConfirmSeatsAsync(Guid tripId, List<string> seatNumbers, CancellationToken ct = default);
    Task ReleaseSeatsAsync(Guid tripId, List<string> seatNumbers, CancellationToken ct = default);
}

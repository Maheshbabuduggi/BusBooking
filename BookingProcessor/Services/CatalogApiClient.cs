// CatalogApiClient.cs
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
namespace BookingProcessor.Services;

public class CatalogApiClient : ICatalogApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CatalogApiClient> _logger;

    public CatalogApiClient(HttpClient httpClient, ILogger<CatalogApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task ConfirmSeatsAsync(Guid tripId, List<string> seatNumbers, CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync($"/api/trips/{tripId}/seats/confirm", new { SeatNumbers = seatNumbers }, ct);
        if (!response.IsSuccessStatusCode)
            _logger.LogWarning("Failed to confirm seats for trip {TripId}: {Status}", tripId, response.StatusCode);
    }

    public async Task ReleaseSeatsAsync(Guid tripId, List<string> seatNumbers, CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync($"/api/trips/{tripId}/seats/release", new { SeatNumbers = seatNumbers }, ct);
        if (!response.IsSuccessStatusCode)
            _logger.LogWarning("Failed to release seats for trip {TripId}: {Status}", tripId, response.StatusCode);
    }
}


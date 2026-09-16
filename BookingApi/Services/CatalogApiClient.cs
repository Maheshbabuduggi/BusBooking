using System.Net.Http.Json;
using BookingApi.DTOs;

namespace BookingApi.Services;

public class CatalogApiClient : ICatalogApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CatalogApiClient> _logger;

    public CatalogApiClient(HttpClient httpClient, ILogger<CatalogApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<TripInfoDto?> GetTripAsync(Guid tripId, CancellationToken ct = default)
    {
        var response = await _httpClient.GetAsync($"/api/trips/{tripId}", ct);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<TripInfoDto>(cancellationToken: ct);
    }

    public async Task<List<SeatInfoDto>> GetSeatsAsync(Guid tripId, CancellationToken ct = default)
    {
        var response = await _httpClient.GetAsync($"/api/trips/{tripId}/seats", ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<SeatInfoDto>>(cancellationToken: ct) ?? new();
    }

    public async Task<SeatActionResultDto> HoldSeatsAsync(Guid tripId, List<string> seatNumbers, CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync(
            $"/api/trips/{tripId}/seats/hold", new SeatActionRequestDto { SeatNumbers = seatNumbers }, ct);

        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<SeatActionResultDto>(cancellationToken: ct)
                ?? new SeatActionResultDto { Success = true };

        _logger.LogWarning("CatalogApi rejected seat hold for trip {TripId}: {Status}", tripId, response.StatusCode);
        return new SeatActionResultDto { Success = false, Message = "Requested seats are no longer available." };
    }
}

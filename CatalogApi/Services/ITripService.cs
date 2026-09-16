// ITripService.cs
using CatalogApi.DTOs;

namespace CatalogApi.Services;

public interface ITripService
{
    Task<TripResponseDto> CreateTripAsync(CreateTripDto dto, CancellationToken ct = default);
    Task<TripResponseDto?> GetTripByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<TripResponseDto>> SearchTripsAsync(string? origin, string? destination, DateTime? date, CancellationToken ct = default);
}

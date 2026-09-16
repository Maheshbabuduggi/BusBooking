// IRouteService.cs
using CatalogApi.DTOs;

namespace CatalogApi.Services;

public interface IRouteService
{
    Task<RouteResponseDto> CreateRouteAsync(CreateRouteDto dto, CancellationToken ct = default);
    Task<IEnumerable<RouteResponseDto>> GetAllRoutesAsync(CancellationToken ct = default);
}

// RouteService.cs
using Microsoft.EntityFrameworkCore;
using CatalogApi.Data;
using CatalogApi.DTOs;
using RouteEntity = CatalogApi.Data.Entities.Route;

namespace CatalogApi.Services;

public class RouteService : IRouteService
{
    private readonly CatalogDbContext _db;

    public RouteService(CatalogDbContext db) => _db = db;

    public async Task<RouteResponseDto> CreateRouteAsync(CreateRouteDto dto, CancellationToken ct = default)
    {
        var route = new RouteEntity
        {
            Id = Guid.NewGuid(),
            Origin = dto.Origin,
            Destination = dto.Destination,
            DistanceKm = dto.DistanceKm,
            EstimatedDurationMinutes = dto.EstimatedDurationMinutes,
            CreatedAt = DateTime.UtcNow
        };
        _db.Routes.Add(route);
        await _db.SaveChangesAsync(ct);
        return ToDto(route);
    }

    public async Task<IEnumerable<RouteResponseDto>> GetAllRoutesAsync(CancellationToken ct = default)
    {
        var routes = await _db.Routes.AsNoTracking().OrderBy(r => r.Origin).ToListAsync(ct);
        return routes.Select(ToDto);
    }

    private static RouteResponseDto ToDto(RouteEntity r) => new()
    {
        Id = r.Id, Origin = r.Origin, Destination = r.Destination,
        DistanceKm = r.DistanceKm, EstimatedDurationMinutes = r.EstimatedDurationMinutes, CreatedAt = r.CreatedAt
    };
}

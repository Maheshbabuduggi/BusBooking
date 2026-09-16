// BusService.cs
using Microsoft.EntityFrameworkCore;
using CatalogApi.Data;
using CatalogApi.Data.Entities;
using CatalogApi.DTOs;

namespace CatalogApi.Services;

public class BusService : IBusService
{
    private readonly CatalogDbContext _db;

    public BusService(CatalogDbContext db) => _db = db;

    public async Task<BusResponseDto> CreateBusAsync(CreateBusDto dto, CancellationToken ct = default)
    {
        var bus = new Bus
        {
            Id = Guid.NewGuid(),
            RegistrationNumber = dto.RegistrationNumber,
            OperatorName = dto.OperatorName,
            TotalSeats = dto.TotalSeats,
            BusType = dto.BusType,
            CreatedAt = DateTime.UtcNow
        };
        _db.Buses.Add(bus);
        await _db.SaveChangesAsync(ct);
        return ToDto(bus);
    }

    public async Task<IEnumerable<BusResponseDto>> GetAllBusesAsync(CancellationToken ct = default)
    {
        var buses = await _db.Buses.AsNoTracking().OrderByDescending(b => b.CreatedAt).ToListAsync(ct);
        return buses.Select(ToDto);
    }

    private static BusResponseDto ToDto(Bus b) => new()
    {
        Id = b.Id, RegistrationNumber = b.RegistrationNumber, OperatorName = b.OperatorName,
        TotalSeats = b.TotalSeats, BusType = b.BusType, CreatedAt = b.CreatedAt
    };
}

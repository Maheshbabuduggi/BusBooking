// IBusService.cs
using CatalogApi.DTOs;

namespace CatalogApi.Services;

public interface IBusService
{
    Task<BusResponseDto> CreateBusAsync(CreateBusDto dto, CancellationToken ct = default);
    Task<IEnumerable<BusResponseDto>> GetAllBusesAsync(CancellationToken ct = default);
}

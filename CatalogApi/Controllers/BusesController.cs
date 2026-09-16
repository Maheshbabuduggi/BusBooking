using Microsoft.AspNetCore.Mvc;
using CatalogApi.DTOs;
using CatalogApi.Services;

namespace CatalogApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BusesController : ControllerBase
{
    private readonly IBusService _busService;
    public BusesController(IBusService busService) => _busService = busService;

    [HttpPost]
    public async Task<ActionResult<BusResponseDto>> CreateBus([FromBody] CreateBusDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _busService.CreateBusAsync(dto, ct);
        return CreatedAtAction(nameof(GetAllBuses), new { }, result);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BusResponseDto>>> GetAllBuses(CancellationToken ct)
        => Ok(await _busService.GetAllBusesAsync(ct));
}

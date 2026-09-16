using Microsoft.AspNetCore.Mvc;
using CatalogApi.DTOs;
using CatalogApi.Services;

namespace CatalogApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoutesController : ControllerBase
{
    private readonly IRouteService _routeService;
    public RoutesController(IRouteService routeService) => _routeService = routeService;

    [HttpPost]
    public async Task<ActionResult<RouteResponseDto>> CreateRoute([FromBody] CreateRouteDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _routeService.CreateRouteAsync(dto, ct);
        return CreatedAtAction(nameof(GetAllRoutes), new { }, result);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RouteResponseDto>>> GetAllRoutes(CancellationToken ct)
        => Ok(await _routeService.GetAllRoutesAsync(ct));
}

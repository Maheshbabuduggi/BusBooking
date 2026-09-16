using Microsoft.AspNetCore.Mvc;
using CatalogApi.DTOs;
using CatalogApi.Services;

namespace CatalogApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripsController : ControllerBase
{
    private readonly ITripService _tripService;
    private readonly ISeatService _seatService;

    public TripsController(ITripService tripService, ISeatService seatService)
    {
        _tripService = tripService;
        _seatService = seatService;
    }

    [HttpPost]
    public async Task<ActionResult<TripResponseDto>> CreateTrip([FromBody] CreateTripDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var result = await _tripService.CreateTripAsync(dto, ct);
            return CreatedAtAction(nameof(GetTripById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TripResponseDto>> GetTripById(Guid id, CancellationToken ct)
    {
        var trip = await _tripService.GetTripByIdAsync(id, ct);
        return trip is null ? NotFound() : Ok(trip);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TripResponseDto>>> SearchTrips(
        [FromQuery] string? origin, [FromQuery] string? destination, [FromQuery] DateTime? date, CancellationToken ct)
        => Ok(await _tripService.SearchTripsAsync(origin, destination, date, ct));

    [HttpGet("{id:guid}/seats")]
    public async Task<ActionResult<IEnumerable<SeatResponseDto>>> GetSeats(Guid id, CancellationToken ct)
        => Ok(await _seatService.GetSeatsForTripAsync(id, ct));

    [HttpPost("{id:guid}/seats/hold")]
    public async Task<ActionResult<SeatActionResultDto>> HoldSeats(Guid id, [FromBody] SeatActionRequestDto dto, CancellationToken ct)
    {
        var result = await _seatService.HoldSeatsAsync(id, dto.SeatNumbers, ct);
        return result.Success ? Ok(result) : Conflict(result);
    }

    [HttpPost("{id:guid}/seats/confirm")]
    public async Task<ActionResult<SeatActionResultDto>> ConfirmSeats(Guid id, [FromBody] SeatActionRequestDto dto, CancellationToken ct)
        => Ok(await _seatService.ConfirmSeatsAsync(id, dto.SeatNumbers, ct));

    [HttpPost("{id:guid}/seats/release")]
    public async Task<ActionResult<SeatActionResultDto>> ReleaseSeats(Guid id, [FromBody] SeatActionRequestDto dto, CancellationToken ct)
        => Ok(await _seatService.ReleaseSeatsAsync(id, dto.SeatNumbers, ct));
}

using Microsoft.AspNetCore.Mvc;
using BookingApi.DTOs;
using BookingApi.Services;

namespace BookingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;
    public BookingsController(IBookingService bookingService) => _bookingService = bookingService;

    [HttpPost]
    public async Task<ActionResult<BookingResponseDto>> CreateBooking([FromBody] CreateBookingDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (success, error, booking) = await _bookingService.CreateBookingAsync(dto, ct);
        if (!success) return Conflict(new { error });

        return CreatedAtAction(nameof(GetBookingById), new { id = booking!.Id }, booking);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BookingResponseDto>> GetBookingById(Guid id, CancellationToken ct)
    {
        var booking = await _bookingService.GetBookingByIdAsync(id, ct);
        return booking is null ? NotFound() : Ok(booking);
    }
}

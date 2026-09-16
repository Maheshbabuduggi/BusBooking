using System.ComponentModel.DataAnnotations;

namespace BookingApi.DTOs;

public class CreateBookingDto
{
    [Required] public Guid TripId { get; set; }
    [Required, MinLength(1)] public List<string> SeatNumbers { get; set; } = new();
    [Required, MaxLength(200)] public string CustomerName { get; set; } = string.Empty;
    [Required, EmailAddress, MaxLength(200)] public string CustomerEmail { get; set; } = string.Empty;
    [Required, MaxLength(20)] public string CustomerPhone { get; set; } = string.Empty;
}

public class BookingResponseDto
{
    public Guid Id { get; set; }
    public Guid TripId { get; set; }
    public List<string> SeatNumbers { get; set; } = new();
    public string CustomerName { get; set; } = string.Empty;
    public int NumberOfSeats { get; set; }
    public decimal TotalFare { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
}

namespace BookingProcessor.Data.Entities;

public class Booking
{
    public Guid Id { get; set; }
    public Guid TripId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public int NumberOfSeats { get; set; }
    public decimal TotalFare { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
}

namespace BookingApi.Data.Entities;

public class Booking
{
    public Guid Id { get; set; }
    public Guid TripId { get; set; }              // references CatalogApi's Trip.Id — no FK here, different bounded context
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public int NumberOfSeats { get; set; }
    public decimal TotalFare { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Confirmed, PaymentFailed, Expired, Cancelled
    public DateTime CreatedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
}

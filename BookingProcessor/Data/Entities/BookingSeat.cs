namespace BookingProcessor.Data.Entities;

public class BookingSeat
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public string SeatNumber { get; set; } = string.Empty;
}

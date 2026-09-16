namespace BookingProcessor.Messaging;

public class BookingCreatedEvent
{
    public Guid BookingId { get; set; }
    public Guid TripId { get; set; }
    public List<string> SeatNumbers { get; set; } = new();
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
}

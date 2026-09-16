namespace CatalogApi.Data.Entities;

public class Seat
{
    public Guid Id { get; set; }
    public Guid TripId { get; set; }
    public string SeatNumber { get; set; } = string.Empty; // "S1", "S2", ...
    public string Status { get; set; } = "Available";      // Available, Held, Booked
    public DateTime? HeldAt { get; set; }

    public Trip? Trip { get; set; }
}

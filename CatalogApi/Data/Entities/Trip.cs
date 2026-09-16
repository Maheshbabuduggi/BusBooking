namespace CatalogApi.Data.Entities;

public class Trip
{
    public Guid Id { get; set; }
    public Guid RouteId { get; set; }
    public Guid BusId { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public decimal FarePerSeat { get; set; }
    public string Status { get; set; } = "Scheduled"; // Scheduled, Departed, Completed, Cancelled
    public DateTime CreatedAt { get; set; }

    public Route? Route { get; set; }
    public Bus? Bus { get; set; }
    public List<Seat> Seats { get; set; } = new();
}

using System.ComponentModel.DataAnnotations;

namespace CatalogApi.DTOs;

public class CreateTripDto
{
    [Required] public Guid RouteId { get; set; }
    [Required] public Guid BusId { get; set; }
    [Required] public DateTime DepartureTime { get; set; }
    [Required] public DateTime ArrivalTime { get; set; }
    [Range(1, 100000)] public decimal FarePerSeat { get; set; }
}

public class TripResponseDto
{
    public Guid Id { get; set; }
    public Guid RouteId { get; set; }
    public Guid BusId { get; set; }
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public decimal FarePerSeat { get; set; }
    public string Status { get; set; } = string.Empty;
    public int TotalSeats { get; set; }
    public int AvailableSeats { get; set; }
}

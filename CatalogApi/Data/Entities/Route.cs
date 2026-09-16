namespace CatalogApi.Data.Entities;

public class Route
{
    public Guid Id { get; set; }
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public decimal DistanceKm { get; set; }
    public int EstimatedDurationMinutes { get; set; }
    public DateTime CreatedAt { get; set; }
}

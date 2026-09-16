using System.ComponentModel.DataAnnotations;

namespace CatalogApi.DTOs;

public class CreateRouteDto
{
    [Required, MaxLength(100)] public string Origin { get; set; } = string.Empty;
    [Required, MaxLength(100)] public string Destination { get; set; } = string.Empty;
    [Range(1, 5000)] public decimal DistanceKm { get; set; }
    [Range(1, 3000)] public int EstimatedDurationMinutes { get; set; }
}

public class RouteResponseDto
{
    public Guid Id { get; set; }
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public decimal DistanceKm { get; set; }
    public int EstimatedDurationMinutes { get; set; }
    public DateTime CreatedAt { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace CatalogApi.DTOs;

public class CreateBusDto
{
    [Required, MaxLength(50)] public string RegistrationNumber { get; set; } = string.Empty;
    [Required, MaxLength(200)] public string OperatorName { get; set; } = string.Empty;
    [Range(1, 100)] public int TotalSeats { get; set; }
    [Required, MaxLength(50)] public string BusType { get; set; } = string.Empty;
}

public class BusResponseDto
{
    public Guid Id { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;
    public string OperatorName { get; set; } = string.Empty;
    public int TotalSeats { get; set; }
    public string BusType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

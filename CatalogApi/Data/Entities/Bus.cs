namespace CatalogApi.Data.Entities;

public class Bus
{
    public Guid Id { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;
    public string OperatorName { get; set; } = string.Empty;
    public int TotalSeats { get; set; }
    public string BusType { get; set; } = string.Empty; // e.g. "AC_Sleeper", "NonAC_Seater"
    public DateTime CreatedAt { get; set; }
}

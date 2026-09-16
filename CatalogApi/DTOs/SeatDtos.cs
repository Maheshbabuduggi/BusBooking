namespace CatalogApi.DTOs;

public class SeatResponseDto
{
    public Guid Id { get; set; }
    public string SeatNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class SeatActionRequestDto
{
    public List<string> SeatNumbers { get; set; } = new();
}

public class SeatActionResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}

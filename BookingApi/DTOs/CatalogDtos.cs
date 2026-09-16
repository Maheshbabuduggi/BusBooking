namespace BookingApi.DTOs;

public class TripInfoDto
{
    public Guid Id { get; set; }
    public decimal FarePerSeat { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class SeatInfoDto
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

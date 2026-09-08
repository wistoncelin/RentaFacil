namespace BookingService.Application.DTOs;

public class BookingResponse
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public int VehicleId { get; set; }
    public string BookingReference { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
}

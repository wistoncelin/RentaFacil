namespace BookingService.Application.DTOs;

public class CreateBookingRequest
{
    public int ClientId { get; set; }
    public int VehicleId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal DailyRate { get; set; }
}

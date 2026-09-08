namespace VehicleService.Domain.Entities;

public class VehicleReservation
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string BookingReference { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Vehicle Vehicle { get; set; } = null!;
}

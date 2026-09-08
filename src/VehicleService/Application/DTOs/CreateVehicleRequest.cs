using VehicleService.Domain.Entities;

namespace VehicleService.Application.DTOs;

public class CreateVehicleRequest
{
    public string Plate { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public VehicleType Type { get; set; }
    public decimal DailyRate { get; set; }
}

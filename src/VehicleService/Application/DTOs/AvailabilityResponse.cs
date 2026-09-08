namespace VehicleService.Application.DTOs;

public class AvailabilityResponse
{
    public string VehicleType { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<VehicleResponse> AvailableVehicles { get; set; } = new();
}

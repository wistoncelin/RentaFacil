using VehicleService.Domain.Entities;

namespace VehicleService.Application.DTOs;

public class AvailabilityRequest
{
    public VehicleType VehicleType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

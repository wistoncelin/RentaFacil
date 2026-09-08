using VehicleService.Domain.Entities;

namespace VehicleService.Application.Interfaces;

public interface IVehicleRepository
{
    Task<Vehicle?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Vehicle>> GetAvailableVehiclesAsync(VehicleType type, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task AddAsync(Vehicle vehicle, CancellationToken cancellationToken = default);
    Task<bool> ExistsByPlateAsync(string plate, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task AddReservationAsync(VehicleReservation reservation, CancellationToken cancellationToken = default);
}

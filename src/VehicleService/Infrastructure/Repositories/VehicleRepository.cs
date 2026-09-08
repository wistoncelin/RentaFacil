using Microsoft.EntityFrameworkCore;
using VehicleService.Application.Interfaces;
using VehicleService.Domain.Entities;
using VehicleService.Infrastructure.Persistence;

namespace VehicleService.Infrastructure.Repositories;

public class VehicleRepository : IVehicleRepository
{
    private readonly VehicleDbContext _context;

    public VehicleRepository(VehicleDbContext context)
    {
        _context = context;
    }

    public async Task<Vehicle?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Vehicles
            .Include(v => v.Reservations)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async Task<List<Vehicle>> GetAvailableVehiclesAsync(VehicleType type, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var vehicles = await _context.Vehicles
            .Where(v => v.Type == type && v.IsActive)
            .Include(v => v.Reservations)
            .ToListAsync(cancellationToken);

        return vehicles
            .Where(v => v.Reservations.All(r => r.EndDate < startDate || r.StartDate > endDate))
            .ToList();
    }

    public async Task AddAsync(Vehicle vehicle, CancellationToken cancellationToken = default)
    {
        await _context.Vehicles.AddAsync(vehicle, cancellationToken);
    }

    public async Task<bool> ExistsByPlateAsync(string plate, CancellationToken cancellationToken = default)
    {
        return await _context.Vehicles.AnyAsync(v => v.Plate == plate, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddReservationAsync(VehicleReservation reservation, CancellationToken cancellationToken = default)
    {
        await _context.Reservations.AddAsync(reservation, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

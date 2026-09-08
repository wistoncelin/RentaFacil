using VehicleService.Application.DTOs;
using VehicleService.Application.Interfaces;
using VehicleService.Domain.Entities;

namespace VehicleService.Application.Services;

public class VehicleApplicationService
{
    private readonly IVehicleRepository _repository;

    public VehicleApplicationService(IVehicleRepository repository)
    {
        _repository = repository;
    }

    public async Task<VehicleResponse> CreateVehicleAsync(CreateVehicleRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Plate))
        {
            throw new ArgumentException("La placa es obligatoria.", nameof(request.Plate));
        }

        if (request.DailyRate <= 0)
        {
            throw new ArgumentException("La tarifa diaria debe ser mayor a cero.", nameof(request.DailyRate));
        }

        if (await _repository.ExistsByPlateAsync(request.Plate, cancellationToken))
        {
            throw new InvalidOperationException($"Ya existe un vehículo con la placa {request.Plate}.");
        }

        var vehicle = new Vehicle
        {
            Plate = request.Plate.Trim(),
            Brand = request.Brand.Trim(),
            Model = request.Model.Trim(),
            Type = request.Type,
            DailyRate = request.DailyRate,
            IsActive = true
        };

        await _repository.AddAsync(vehicle, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return new VehicleResponse
        {
            Id = vehicle.Id,
            Plate = vehicle.Plate,
            Brand = vehicle.Brand,
            Model = vehicle.Model,
            Type = vehicle.Type,
            DailyRate = vehicle.DailyRate,
            IsActive = vehicle.IsActive
        };
    }

    public async Task<AvailabilityResponse> GetAvailabilityAsync(AvailabilityRequest request, CancellationToken cancellationToken = default)
    {
        if (request.EndDate < request.StartDate)
        {
            throw new ArgumentException("La fecha final no puede ser anterior a la inicial.");
        }

        var availableVehicles = await _repository.GetAvailableVehiclesAsync(request.VehicleType, request.StartDate, request.EndDate, cancellationToken);

        return new AvailabilityResponse
        {
            VehicleType = request.VehicleType.ToString(),
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            AvailableVehicles = availableVehicles.Select(v => new VehicleResponse
            {
                Id = v.Id,
                Plate = v.Plate,
                Brand = v.Brand,
                Model = v.Model,
                Type = v.Type,
                DailyRate = v.DailyRate,
                IsActive = v.IsActive
            }).ToList()
        };
    }

    public async Task ReserveVehicleAsync(int vehicleId, DateTime startDate, DateTime endDate, string bookingReference, CancellationToken cancellationToken = default)
    {
        var vehicle = await _repository.GetByIdAsync(vehicleId, cancellationToken);
        if (vehicle is null)
        {
            throw new KeyNotFoundException($"No existe el vehículo con Id {vehicleId}.");
        }

        var availableVehicles = await _repository.GetAvailableVehiclesAsync(vehicle.Type, startDate, endDate, cancellationToken);
        if (!availableVehicles.Any(v => v.Id == vehicleId))
        {
            throw new InvalidOperationException("El vehículo no está disponible en ese rango de fechas.");
        }

        var reservation = new VehicleReservation
        {
            VehicleId = vehicleId,
            StartDate = startDate,
            EndDate = endDate,
            BookingReference = bookingReference
        };

        await _repository.AddReservationAsync(reservation, cancellationToken);
    }
}

using Xunit;
using Microsoft.EntityFrameworkCore;
using VehicleService.Application.DTOs;
using VehicleService.Application.Services;
using VehicleService.Domain.Entities;
using VehicleService.Infrastructure.Persistence;
using VehicleService.Infrastructure.Repositories;

namespace Rentafacil.Tests;

public class VehicleApplicationServiceTests
{
    [Fact]
    public async Task CreateVehicleAsync_ShouldPersistVehicle()
    {
        var options = new DbContextOptionsBuilder<VehicleDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var dbContext = new VehicleDbContext(options);
        var service = new VehicleApplicationService(new VehicleRepository(dbContext));

        var result = await service.CreateVehicleAsync(new CreateVehicleRequest
        {
            Plate = "XYZ999",
            Brand = "Ford",
            Model = "Focus",
            Type = VehicleType.Sedan,
            DailyRate = 60m
        });

        Assert.Equal("XYZ999", result.Plate);
        Assert.Equal(VehicleType.Sedan, result.Type);
    }

    [Fact]
    public async Task GetAvailabilityAsync_ShouldFilterVehicleByDateRange()
    {
        var options = new DbContextOptionsBuilder<VehicleDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var dbContext = new VehicleDbContext(options);
        var availableVehicle = new Vehicle
        {
            Plate = "ABC111",
            Brand = "Toyota",
            Model = "Corolla",
            Type = VehicleType.Sedan,
            DailyRate = 50m,
            IsActive = true
        };

        var reservedVehicle = new Vehicle
        {
            Plate = "ABC222",
            Brand = "Renault",
            Model = "Logan",
            Type = VehicleType.Sedan,
            DailyRate = 40m,
            IsActive = true
        };

        dbContext.Vehicles.AddRange(availableVehicle, reservedVehicle);
        await dbContext.SaveChangesAsync();

        dbContext.Reservations.Add(new VehicleReservation
        {
            VehicleId = reservedVehicle.Id,
            StartDate = new DateTime(2026, 09, 10),
            EndDate = new DateTime(2026, 09, 15),
            BookingReference = "RF-123"
        });

        await dbContext.SaveChangesAsync();

        var service = new VehicleApplicationService(new VehicleRepository(dbContext));
        var result = await service.GetAvailabilityAsync(new AvailabilityRequest
        {
            VehicleType = VehicleType.Sedan,
            StartDate = new DateTime(2026, 09, 10),
            EndDate = new DateTime(2026, 09, 15)
        });

        Assert.Single(result.AvailableVehicles);
        Assert.Equal(availableVehicle.Id, result.AvailableVehicles[0].Id);
    }
}

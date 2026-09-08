using Xunit;
using BookingService.Application.DTOs;
using BookingService.Application.Services;
using BookingService.Domain.Entities;
using BookingService.Infrastructure.Clients;
using BookingService.Infrastructure.Persistence;
using BookingService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Rentafacil.Tests;

public class BookingApplicationServiceTests
{
    [Fact]
    public async Task CreateClientAsync_ShouldPersistClient()
    {
        var options = new DbContextOptionsBuilder<BookingDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var dbContext = new BookingDbContext(options);
        var repository = new BookingRepository(dbContext);
        var service = new BookingApplicationService(repository, new StubVehicleReservationClient());

        var result = await service.CreateClientAsync(new CreateClientRequest
        {
            DocumentNumber = "999888777",
            Name = "Luis",
            LastName = "Pérez",
            Email = "luis@rentafacil.com"
        });

        Assert.Equal("999888777", result.DocumentNumber);
        Assert.Equal("Luis", result.Name);
    }

    [Fact]
    public async Task CreateBookingAsync_ShouldCreateBookingAndReserveVehicle()
    {
        var options = new DbContextOptionsBuilder<BookingDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var dbContext = new BookingDbContext(options);
        var client = new Client
        {
            DocumentNumber = "222333444",
            Name = "Marta",
            LastName = "Soto",
            Email = "marta@rentafacil.com"
        };

        dbContext.Clients.Add(client);
        await dbContext.SaveChangesAsync();

        var repository = new BookingRepository(dbContext);
        var service = new BookingApplicationService(repository, new StubVehicleReservationClient());

        var result = await service.CreateBookingAsync(new CreateBookingRequest
        {
            ClientId = client.Id,
            VehicleId = 3,
            StartDate = new DateTime(2026, 09, 20),
            EndDate = new DateTime(2026, 09, 25),
            DailyRate = 80m
        });

        Assert.Equal("Confirmed", result.Status);
        Assert.NotEmpty(result.BookingReference);
    }

    private sealed class StubVehicleReservationClient : IVehicleReservationClient
    {
        public Task ReserveVehicleAsync(int vehicleId, DateTime startDate, DateTime endDate, string bookingReference, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}

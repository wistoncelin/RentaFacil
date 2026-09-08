using BookingService.Application.DTOs;
using BookingService.Application.Interfaces;
using BookingService.Domain.Entities;
using BookingService.Infrastructure.Clients;

namespace BookingService.Application.Services;

public class BookingApplicationService
{
    private readonly IBookingRepository _repository;
    private readonly IVehicleReservationClient _vehicleReservationClient;

    public BookingApplicationService(IBookingRepository repository, IVehicleReservationClient vehicleReservationClient)
    {
        _repository = repository;
        _vehicleReservationClient = vehicleReservationClient;
    }

    public async Task<ClientResponse> CreateClientAsync(CreateClientRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.DocumentNumber))
        {
            throw new ArgumentException("El documento es obligatorio.");
        }

        if (await _repository.GetClientByDocumentAsync(request.DocumentNumber.Trim(), cancellationToken) is not null)
        {
            throw new InvalidOperationException($"El cliente con documento {request.DocumentNumber} ya existe.");
        }

        var client = new Client
        {
            DocumentNumber = request.DocumentNumber.Trim(),
            Name = request.Name.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email.Trim()
        };

        await _repository.AddClientAsync(client, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return new ClientResponse
        {
            Id = client.Id,
            DocumentNumber = client.DocumentNumber,
            Name = client.Name,
            LastName = client.LastName,
            Email = client.Email
        };
    }

    public async Task<BookingResponse> CreateBookingAsync(CreateBookingRequest request, CancellationToken cancellationToken = default)
    {
        if (request.EndDate < request.StartDate)
        {
            throw new ArgumentException("La fecha final no puede ser anterior a la inicial.");
        }

        var client = await _repository.GetClientByIdAsync(request.ClientId, cancellationToken);
        if (client is null)
        {
            throw new KeyNotFoundException($"No existe el cliente con Id {request.ClientId}.");
        }

        var bookingReference = $"RF-{DateTime.UtcNow:yyyyMMddHHmmss}-{request.ClientId}";
        var totalAmount = request.DailyRate * (decimal)(request.EndDate.Date - request.StartDate.Date).TotalDays;

        await _vehicleReservationClient.ReserveVehicleAsync(request.VehicleId, request.StartDate, request.EndDate, bookingReference, cancellationToken);

        var booking = new Booking
        {
            ClientId = request.ClientId,
            VehicleId = request.VehicleId,
            BookingReference = bookingReference,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            TotalAmount = totalAmount,
            Status = "Confirmed"
        };

        await _repository.AddBookingAsync(booking, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return new BookingResponse
        {
            Id = booking.Id,
            ClientId = booking.ClientId,
            VehicleId = booking.VehicleId,
            BookingReference = booking.BookingReference,
            StartDate = booking.StartDate,
            EndDate = booking.EndDate,
            TotalAmount = booking.TotalAmount,
            Status = booking.Status
        };
    }

    public async Task<List<BookingResponse>> GetBookingHistoryByClientAsync(int clientId, CancellationToken cancellationToken = default)
    {
        var bookings = await _repository.GetBookingsByClientAsync(clientId, cancellationToken);

        return bookings.Select(b => new BookingResponse
        {
            Id = b.Id,
            ClientId = b.ClientId,
            VehicleId = b.VehicleId,
            BookingReference = b.BookingReference,
            StartDate = b.StartDate,
            EndDate = b.EndDate,
            TotalAmount = b.TotalAmount,
            Status = b.Status
        }).ToList();
    }
}

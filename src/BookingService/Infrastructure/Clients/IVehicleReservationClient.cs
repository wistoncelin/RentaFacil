namespace BookingService.Infrastructure.Clients;

public interface IVehicleReservationClient
{
    Task ReserveVehicleAsync(int vehicleId, DateTime startDate, DateTime endDate, string bookingReference, CancellationToken cancellationToken = default);
}

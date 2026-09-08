using System.Net.Http.Json;

namespace BookingService.Infrastructure.Clients;

public class VehicleReservationClient : IVehicleReservationClient
{
    private readonly HttpClient _httpClient;

    public VehicleReservationClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task ReserveVehicleAsync(int vehicleId, DateTime startDate, DateTime endDate, string bookingReference, CancellationToken cancellationToken = default)
    {
        var request = new
        {
            vehicleId,
            startDate,
            endDate,
            bookingReference
        };

        var response = await _httpClient.PostAsJsonAsync("api/vehicles/reserve", request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"La reserva del vehículo falló: {content}");
        }
    }
}

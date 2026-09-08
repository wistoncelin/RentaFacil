using BookingService.Domain.Entities;

namespace BookingService.Application.Interfaces;

public interface IBookingRepository
{
    Task<Client?> GetClientByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Client?> GetClientByDocumentAsync(string documentNumber, CancellationToken cancellationToken = default);
    Task AddClientAsync(Client client, CancellationToken cancellationToken = default);
    Task<List<Booking>> GetBookingsByClientAsync(int clientId, CancellationToken cancellationToken = default);
    Task AddBookingAsync(Booking booking, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

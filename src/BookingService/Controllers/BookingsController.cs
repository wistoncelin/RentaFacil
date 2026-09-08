using BookingService.Application.DTOs;
using BookingService.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly BookingApplicationService _service;
    private readonly ILogger<BookingsController> _logger;

    public BookingsController(BookingApplicationService service, ILogger<BookingsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost("clients")]
    public async Task<ActionResult<ClientResponse>> CreateClient([FromBody] CreateClientRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _service.CreateClientAsync(request, cancellationToken);
            return CreatedAtAction(nameof(CreateClient), response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creando cliente.");
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<BookingResponse>> CreateBooking([FromBody] CreateBookingRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _service.CreateBookingAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creando reserva.");
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("client/{clientId:int}")]
    public async Task<ActionResult<List<BookingResponse>>> GetHistoryByClient(int clientId, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _service.GetBookingHistoryByClientAsync(clientId, cancellationToken);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error consultando historial del cliente {ClientId}.", clientId);
            return BadRequest(ex.Message);
        }
    }
}

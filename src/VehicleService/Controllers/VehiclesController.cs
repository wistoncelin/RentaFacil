using Microsoft.AspNetCore.Mvc;
using VehicleService.Application.DTOs;
using VehicleService.Application.Services;

namespace VehicleService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehiclesController : ControllerBase
{
    private readonly VehicleApplicationService _service;
    private readonly ILogger<VehiclesController> _logger;

    public VehiclesController(VehicleApplicationService service, ILogger<VehiclesController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<VehicleResponse>> Create([FromBody] CreateVehicleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _service.CreateVehicleAsync(request, cancellationToken);
            return CreatedAtAction(nameof(Create), response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registrando vehículo.");
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("availability")]
    public async Task<ActionResult<AvailabilityResponse>> GetAvailability([FromQuery] AvailabilityRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _service.GetAvailabilityAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error consultando disponibilidad.");
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("reserve")]
    public async Task<ActionResult> Reserve([FromBody] ReserveVehicleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _service.ReserveVehicleAsync(request.VehicleId, request.StartDate, request.EndDate, request.BookingReference, cancellationToken);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reservando vehículo.");
            return BadRequest(ex.Message);
        }
    }
}

public class ReserveVehicleRequest
{
    public int VehicleId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string BookingReference { get; set; } = string.Empty;
}

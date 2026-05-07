using MediatR;
using Microsoft.AspNetCore.Mvc;
using BelgiumPulse.Application.AirQuality.Queries.GetAirQualityMeasurements;

namespace BelgiumPulse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AirQualityController : ControllerBase
{
    private readonly IMediator _mediator;

    public AirQualityController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Récupère les mesures de qualité de l'air
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<AirQualityMeasurementDto>), 200)]
    public async Task<IActionResult> GetMeasurements(
        [FromQuery] string? municipality = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetAirQualityMeasurementsQuery(municipality), cancellationToken);
        return Ok(result);
    }
}
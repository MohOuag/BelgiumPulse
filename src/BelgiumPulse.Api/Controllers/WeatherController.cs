using MediatR;
using Microsoft.AspNetCore.Mvc;
using BelgiumPulse.Application.Weather.Queries.GetActiveWeatherAlerts;

namespace BelgiumPulse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    private readonly IMediator _mediator;

    public WeatherController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Récupère les alertes météo actives
    /// </summary>
    [HttpGet("alerts")]
    [ProducesResponseType(typeof(List<WeatherAlertDto>), 200)]
    public async Task<IActionResult> GetActiveAlerts(
        [FromQuery] string? region = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetActiveWeatherAlertsQuery(region), cancellationToken);
        return Ok(result);
    }
}
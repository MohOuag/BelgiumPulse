using MediatR;
using Microsoft.AspNetCore.Mvc;
using BelgiumPulse.Application.Stib.Queries.GetAllStibLines;
using BelgiumPulse.Application.Stib.Queries.GetStibLineByNumber;
using BelgiumPulse.Application.Stib.Commands.ReportDisruption;
using Microsoft.AspNetCore.Authorization;

namespace BelgiumPulse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StibController : ControllerBase
{
    private readonly IMediator _mediator;

    public StibController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Récupère toutes les lignes STIB
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<StibLineDto>), 200)]
    public async Task<IActionResult> GetAll(
        [FromQuery] bool onlyDisrupted = false,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetAllStibLinesQuery(onlyDisrupted), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Récupère une ligne STIB par son numéro
    /// </summary>
    [HttpGet("{lineNumber}")]
    [ProducesResponseType(typeof(StibLineDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetByNumber(
        string lineNumber,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetStibLineByNumberQuery(lineNumber), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Signale une perturbation sur une ligne STIB
    /// </summary>
    [HttpPost("{lineNumber}/disruption")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ReportDisruption(
        string lineNumber,
        [FromBody] ReportDisruptionRequest request,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(
            new ReportDisruptionCommand(lineNumber, request.Message),
            cancellationToken);
        return Ok();
    }
}

public record ReportDisruptionRequest(string Message);
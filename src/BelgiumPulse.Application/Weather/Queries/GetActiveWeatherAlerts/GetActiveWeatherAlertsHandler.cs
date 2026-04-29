using MediatR;
using BelgiumPulse.Domain.Interfaces;

namespace BelgiumPulse.Application.Weather.Queries.GetActiveWeatherAlerts;

public class GetActiveWeatherAlertsHandler
    : IRequestHandler<GetActiveWeatherAlertsQuery, List<WeatherAlertDto>>
{
    private readonly IWeatherRepository _repository;

    public GetActiveWeatherAlertsHandler(IWeatherRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<WeatherAlertDto>> Handle(
        GetActiveWeatherAlertsQuery request,
        CancellationToken cancellationToken)
    {
        var alerts = request.Region is not null
            ? await _repository.GetAlertsByRegionAsync(
                request.Region, cancellationToken)
            : await _repository.GetActiveAlertsAsync(cancellationToken);

        return alerts.Select(a => new WeatherAlertDto(
            a.Id,
            a.Title,
            a.Description,
            a.Severity,
            a.Region,
            a.Location.Latitude,
            a.Location.Longitude,
            a.ValidFrom,
            a.ValidUntil
        )).ToList();
    }
}
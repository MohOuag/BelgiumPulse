using MediatR;

namespace BelgiumPulse.Application.Weather.Queries.GetActiveWeatherAlerts;

public record GetActiveWeatherAlertsQuery(
    string? Region = null
) : IRequest<List<WeatherAlertDto>>;
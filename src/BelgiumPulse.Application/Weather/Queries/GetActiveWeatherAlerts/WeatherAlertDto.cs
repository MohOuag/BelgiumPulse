namespace BelgiumPulse.Application.Weather.Queries.GetActiveWeatherAlerts;

public record WeatherAlertDto(
    Guid Id,
    string Title,
    string Description,
    string Severity,
    string Region,
    double Latitude,
    double Longitude,
    DateTime ValidFrom,
    DateTime ValidUntil
);
namespace BelgiumPulse.Application.AirQuality.Queries.GetAirQualityMeasurements;

public record AirQualityMeasurementDto(
    Guid Id,
    string StationName,
    string Municipality,
    double Latitude,
    double Longitude,
    int AqiValue,
    string AqiLevel,
    double PM25,
    double PM10,
    double NO2,
    bool IsHealthRisk,
    DateTime MeasuredAt
);
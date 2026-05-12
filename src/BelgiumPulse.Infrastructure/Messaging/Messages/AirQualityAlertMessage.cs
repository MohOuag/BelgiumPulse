namespace BelgiumPulse.Infrastructure.Messaging.Messages;

public record AirQualityAlertMessage(
    Guid MeasurementId,
    string StationName,
    string Municipality,
    int AqiValue,
    string AqiLevel,
    DateTime OccurredAt
);
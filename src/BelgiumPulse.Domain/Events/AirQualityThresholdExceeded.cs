using BelgiumPulse.Domain.Common;

namespace BelgiumPulse.Domain.Events;

public sealed record AirQualityThresholdExceeded : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
    public Guid MeasurementId { get; }
    public string StationName { get; }
    public string Municipality { get; }
    public int AqiValue { get; }

    public AirQualityThresholdExceeded(
        Guid measurementId,
        string stationName,
        string municipality,
        int aqiValue)
    {
        MeasurementId = measurementId;
        StationName = stationName;
        Municipality = municipality;
        AqiValue = aqiValue;
    }
}
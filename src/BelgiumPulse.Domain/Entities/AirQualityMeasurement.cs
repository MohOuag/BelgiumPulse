using BelgiumPulse.Domain.Common;
using BelgiumPulse.Domain.Events;
using BelgiumPulse.Domain.ValueObjects;

namespace BelgiumPulse.Domain.Entities;

public class AirQualityMeasurement : BaseEntity
{
    public string StationName { get; private set; }
    public string Municipality { get; private set; }
    public Coordinates Location { get; private set; }
    public AirQualityIndex Index { get; private set; }
    public double PM25 { get; private set; }
    public double PM10 { get; private set; }
    public double NO2 { get; private set; }
    public DateTime MeasuredAt { get; private set; }

    private AirQualityMeasurement() { }

    public static AirQualityMeasurement Create(
        string stationName,
        string municipality,
        Coordinates location,
        int aqiValue,
        double pm25,
        double pm10,
        double no2)
    {
        var measurement = new AirQualityMeasurement
        {
            StationName = stationName,
            Municipality = municipality,
            Location = location,
            Index = new AirQualityIndex(aqiValue),
            PM25 = pm25,
            PM10 = pm10,
            NO2 = no2,
            MeasuredAt = DateTime.UtcNow
        };

        if (measurement.Index.IsHealthRisk)
            measurement.AddDomainEvent(new AirQualityThresholdExceeded(
                measurement.Id, stationName, municipality, aqiValue));

        return measurement;
    }
}
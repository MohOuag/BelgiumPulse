using BelgiumPulse.Domain.Entities;

namespace BelgiumPulse.Domain.Interfaces;

public interface IAirQualityRepository
{
    Task<IEnumerable<AirQualityMeasurement>> GetLatestMeasurementsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<AirQualityMeasurement>> GetByMunicipalityAsync(string municipality, CancellationToken cancellationToken = default);
    Task<IEnumerable<AirQualityMeasurement>> GetHistoryAsync(string stationName, DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task AddAsync(AirQualityMeasurement measurement, CancellationToken cancellationToken = default);
}
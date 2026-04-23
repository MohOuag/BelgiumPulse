using BelgiumPulse.Domain.Entities;

namespace BelgiumPulse.Domain.Interfaces;

public interface IWeatherRepository
{
    Task<IEnumerable<WeatherAlert>> GetActiveAlertsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<WeatherAlert>> GetAlertsByRegionAsync(string region, CancellationToken cancellationToken = default);
    Task AddAsync(WeatherAlert alert, CancellationToken cancellationToken = default);
    Task DeactivateExpiredAlertsAsync(CancellationToken cancellationToken = default);
}
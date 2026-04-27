using Microsoft.EntityFrameworkCore;
using BelgiumPulse.Domain.Entities;
using BelgiumPulse.Domain.Interfaces;

namespace BelgiumPulse.Infrastructure.Persistence.Repositories;

public class WeatherRepository : IWeatherRepository
{
    private readonly BelgiumPulseDbContext _context;

    public WeatherRepository(BelgiumPulseDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<WeatherAlert>> GetActiveAlertsAsync(
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _context.WeatherAlerts
            .AsNoTracking()
            .Where(w => w.ValidFrom <= now && w.ValidUntil >= now)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WeatherAlert>> GetAlertsByRegionAsync(
        string region,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _context.WeatherAlerts
            .AsNoTracking()
            .Where(w => w.Region == region && w.ValidFrom <= now && w.ValidUntil >= now)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        WeatherAlert alert,
        CancellationToken cancellationToken = default)
    {
        await _context.WeatherAlerts.AddAsync(alert, cancellationToken);
    }

    public async Task DeactivateExpiredAlertsAsync(
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        // Bulk operation — pas de chargement en mémoire
        await _context.WeatherAlerts
            .Where(w => w.ValidUntil < now)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
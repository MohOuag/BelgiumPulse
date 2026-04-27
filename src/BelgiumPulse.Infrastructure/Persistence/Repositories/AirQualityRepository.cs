using Microsoft.EntityFrameworkCore;
using BelgiumPulse.Domain.Entities;
using BelgiumPulse.Domain.Interfaces;

namespace BelgiumPulse.Infrastructure.Persistence.Repositories;

public class AirQualityRepository : IAirQualityRepository
{
    private readonly BelgiumPulseDbContext _context;

    public AirQualityRepository(BelgiumPulseDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AirQualityMeasurement>> GetLatestMeasurementsAsync(
        CancellationToken cancellationToken = default)
    {
        // Dernière mesure par station
        return await _context.AirQualityMeasurements
            .AsNoTracking()
            .GroupBy(a => a.StationName)
            .Select(g => g.OrderByDescending(a => a.MeasuredAt).First())
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AirQualityMeasurement>> GetByMunicipalityAsync(
        string municipality,
        CancellationToken cancellationToken = default)
    {
        return await _context.AirQualityMeasurements
            .AsNoTracking()
            .Where(a => a.Municipality == municipality)
            .OrderByDescending(a => a.MeasuredAt)
            .Take(10)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AirQualityMeasurement>> GetHistoryAsync(
        string stationName,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default)
    {
        return await _context.AirQualityMeasurements
            .AsNoTracking()
            .Where(a => a.StationName == stationName
                     && a.MeasuredAt >= from
                     && a.MeasuredAt <= to)
            .OrderBy(a => a.MeasuredAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        AirQualityMeasurement measurement,
        CancellationToken cancellationToken = default)
    {
        await _context.AirQualityMeasurements.AddAsync(measurement, cancellationToken);
    }
}
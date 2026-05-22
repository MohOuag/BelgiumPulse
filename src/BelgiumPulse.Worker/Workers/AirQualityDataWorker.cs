using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using BelgiumPulse.Domain.Interfaces;
using BelgiumPulse.Infrastructure.ExternalApis.AirQuality;
using BelgiumPulse.Application.Common;

namespace BelgiumPulse.Worker.Workers;

public class AirQualityDataWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly BelAqiApiService _belAqiApiService;
    private readonly ILogger<AirQualityDataWorker> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(15);

    public AirQualityDataWorker(
        IServiceScopeFactory scopeFactory,
        BelAqiApiService belAqiApiService,
        ILogger<AirQualityDataWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _belAqiApiService = belAqiApiService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("AirQualityDataWorker démarré");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CollectAndStoreAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la collecte qualité de l'air");
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }

    private async Task CollectAndStoreAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Collecte qualité de l'air — {Time}", DateTime.UtcNow);

        var measurements = await _belAqiApiService
            .GetMeasurementsAsync(cancellationToken);

        if (!measurements.Any())
        {
            _logger.LogWarning("Aucune mesure qualité de l'air récupérée");
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var repository = scope.ServiceProvider
            .GetRequiredService<IAirQualityRepository>();
        var unitOfWork = scope.ServiceProvider
            .GetRequiredService<IUnitOfWork>();
        var cache = scope.ServiceProvider
            .GetRequiredService<ICacheService>();

        foreach (var measurement in measurements)
            await repository.AddAsync(measurement, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Invalide le cache
        await cache.RemoveAsync(CacheKeys.LatestAirQuality, cancellationToken);

        _logger.LogInformation(
            "Collecte qualité de l'air terminée — {Count} mesures traitées",
            measurements.Count);
    }
}
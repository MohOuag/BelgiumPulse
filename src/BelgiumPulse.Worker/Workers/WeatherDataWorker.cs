using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using BelgiumPulse.Domain.Interfaces;
using BelgiumPulse.Infrastructure.ExternalApis.Weather;
using BelgiumPulse.Application.Common;

namespace BelgiumPulse.Worker.Workers;

public class WeatherDataWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IrmApiService _irmApiService;
    private readonly ILogger<WeatherDataWorker> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(10);

    public WeatherDataWorker(
        IServiceScopeFactory scopeFactory,
        IrmApiService irmApiService,
        ILogger<WeatherDataWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _irmApiService = irmApiService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("WeatherDataWorker démarré");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CollectAndStoreAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la collecte météo");
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }

    private async Task CollectAndStoreAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Collecte données météo — {Time}", DateTime.UtcNow);

        var alerts = await _irmApiService.GetActiveAlertsAsync(cancellationToken);

        if (!alerts.Any())
        {
            _logger.LogInformation("Aucune alerte météo active");
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var repository = scope.ServiceProvider
            .GetRequiredService<IWeatherRepository>();
        var unitOfWork = scope.ServiceProvider
            .GetRequiredService<IUnitOfWork>();
        var cache = scope.ServiceProvider
            .GetRequiredService<ICacheService>();

        // Supprime les alertes expirées
        await repository.DeactivateExpiredAlertsAsync(cancellationToken);

        // Ajoute les nouvelles alertes
        foreach (var alert in alerts)
            await repository.AddAsync(alert, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Invalide le cache
        await cache.RemoveAsync(CacheKeys.ActiveWeatherAlerts, cancellationToken);

        _logger.LogInformation(
            "Collecte météo terminée — {Count} alertes traitées",
            alerts.Count);
    }
}
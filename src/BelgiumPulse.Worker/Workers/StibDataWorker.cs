using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using BelgiumPulse.Domain.Interfaces;
using BelgiumPulse.Infrastructure.ExternalApis.Stib;
using BelgiumPulse.Application.Common;

namespace BelgiumPulse.Worker.Workers;

public class StibDataWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly StibApiService _stibApiService;
    private readonly ILogger<StibDataWorker> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);

    public StibDataWorker(
        IServiceScopeFactory scopeFactory,
        StibApiService stibApiService,
        ILogger<StibDataWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _stibApiService = stibApiService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("StibDataWorker démarré");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CollectAndStoreAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la collecte STIB");
            }

            // Attend 5 minutes avant la prochaine collecte
            await Task.Delay(_interval, stoppingToken);
        }
    }

    private async Task CollectAndStoreAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Collecte données STIB — {Time}", DateTime.UtcNow);

        // 1. Récupère les données depuis l'API STIB
        var lines = await _stibApiService.GetLinesAsync(cancellationToken);

        if (!lines.Any())
        {
            _logger.LogWarning("Aucune ligne STIB récupérée");
            return;
        }

        // 2. Crée un scope pour les services Scoped (Repository, UnitOfWork)
        using var scope = _scopeFactory.CreateScope();
        var repository = scope.ServiceProvider
            .GetRequiredService<IStibRepository>();
        var unitOfWork = scope.ServiceProvider
            .GetRequiredService<IUnitOfWork>();
        var cache = scope.ServiceProvider
            .GetRequiredService<ICacheService>();

        // 3. Met à jour chaque ligne en base
        foreach (var line in lines)
        {
            var existing = await repository
                .GetByLineNumberAsync(line.LineNumber, cancellationToken);

            if (existing is null)
                await repository.AddAsync(line, cancellationToken);
            else
            {
                // Met à jour le statut opérationnel
                if (!line.IsOperational && existing.IsOperational)
                    existing.ReportDisruption(
                        line.DisruptionMessage ?? "Perturbation détectée");
                else if (line.IsOperational && !existing.IsOperational)
                    existing.ResumeService();

                await repository.UpdateAsync(existing, cancellationToken);
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        // 4. Invalide le cache — données fraîches en base
        await cache.RemoveAsync(CacheKeys.AllStibLines, cancellationToken);
        await cache.RemoveAsync(CacheKeys.DisruptedStibLines, cancellationToken);

        _logger.LogInformation(
            "Collecte STIB terminée — {Count} lignes traitées",
            lines.Count);
    }
}
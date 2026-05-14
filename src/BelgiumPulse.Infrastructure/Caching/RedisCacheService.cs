using System.Text.Json;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using BelgiumPulse.Domain.Interfaces;

namespace BelgiumPulse.Infrastructure.Caching;

public class RedisCacheService : ICacheService
{
    private readonly IDatabase _database;
    private readonly ILogger<RedisCacheService> _logger;

    public RedisCacheService(
        IConnectionMultiplexer redis,
        ILogger<RedisCacheService> logger)
    {
        _database = redis.GetDatabase();
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(
        string key,
        CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var value = await _database.StringGetAsync(key);

            if (!value.HasValue)
            {
                _logger.LogDebug("Cache MISS — clé: {Key}", key);
                return null;
            }

            _logger.LogDebug("Cache HIT — clé: {Key}", key);
            return JsonSerializer.Deserialize<T>(value!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lecture cache — clé: {Key}", key);
            return null; // Fallback — si Redis est down, on continue sans cache
        }
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan expiration,
        CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var json = JsonSerializer.Serialize(value);
            await _database.StringSetAsync(key, json, expiration);

            _logger.LogDebug(
                "Cache SET — clé: {Key}, expiration: {Expiration}",
                key, expiration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur écriture cache — clé: {Key}", key);
            // On ne propage pas l'erreur — le cache est optionnel
        }
    }

    public async Task RemoveAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _database.KeyDeleteAsync(key);
            _logger.LogDebug("Cache REMOVE — clé: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur suppression cache — clé: {Key}", key);
        }
    }

    // Pattern Cache-Aside — le plus utilisé en enterprise
    public async Task<T> GetOrSetAsync<T>(
        string key,
        Func<Task<T>> factory,
        TimeSpan expiration,
        CancellationToken cancellationToken = default) where T : class
    {
        // 1. Cherche dans le cache
        var cached = await GetAsync<T>(key, cancellationToken);
        if (cached is not null)
            return cached;

        // 2. Cache miss — appelle la factory (base de données, API externe...)
        var value = await factory();

        // 3. Stocke dans le cache pour les prochaines fois
        await SetAsync(key, value, expiration, cancellationToken);

        return value;
    }
}
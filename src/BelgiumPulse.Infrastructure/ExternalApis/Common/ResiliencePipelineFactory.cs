using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;
using Microsoft.Extensions.Logging;

namespace BelgiumPulse.Infrastructure.ExternalApis.Common;

public static class ResiliencePipelineFactory
{
    public static ResiliencePipeline<T> Create<T>(
        string serviceName,
        ILogger logger)
    {
        return new ResiliencePipelineBuilder<T>()

            // 1. Timeout — abandonne après 10 secondes
            .AddTimeout(new TimeoutStrategyOptions
            {
                Timeout = TimeSpan.FromSeconds(10),
                OnTimeout = args =>
                {
                    logger.LogWarning(
                        "Timeout sur {ServiceName} après {Timeout}s",
                        serviceName, 10);
                    return default;
                }
            })

            // 2. Retry — réessaie 3 fois avec backoff exponentiel
            // 1er retry : attend 1s
            // 2ème retry : attend 2s
            // 3ème retry : attend 4s
            .AddRetry(new RetryStrategyOptions<T>
            {
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(1),
                BackoffType = DelayBackoffType.Exponential,
                OnRetry = args =>
                {
                    logger.LogWarning(
                        "Retry #{AttemptNumber} sur {ServiceName}. Raison: {Outcome}",
                        args.AttemptNumber + 1,
                        serviceName,
                        args.Outcome.Exception?.Message);
                    return default;
                }
            })

            // 3. Circuit Breaker — coupe après 3 échecs consécutifs
            // pendant 30 secondes
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions<T>
            {
                FailureRatio = 0.5,
                MinimumThroughput = 3,
                BreakDuration = TimeSpan.FromSeconds(30),
                OnOpened = args =>
                {
                    logger.LogError(
                        "Circuit OUVERT pour {ServiceName} — pause de 30s",
                        serviceName);
                    return default;
                },
                OnClosed = args =>
                {
                    logger.LogInformation(
                        "Circuit FERMÉ pour {ServiceName} — service rétabli",
                        serviceName);
                    return default;
                }
            })

            .Build();
    }
}
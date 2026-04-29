using MediatR;
using Microsoft.Extensions.Logging;

namespace BelgiumPulse.Application.Common.Behaviors;

public class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        _logger.LogInformation("Début traitement {RequestName}", requestName);

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        var response = await next(); //  appelle le Handler suivant

        stopwatch.Stop();

        _logger.LogInformation(
            "Fin traitement {RequestName} en {ElapsedMs}ms",
            requestName,
            stopwatch.ElapsedMilliseconds);

        return response;
    }
}
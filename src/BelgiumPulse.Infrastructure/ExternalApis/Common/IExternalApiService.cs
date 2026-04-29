namespace BelgiumPulse.Infrastructure.ExternalApis.Common;

public interface IExternalApiService
{
    string ServiceName { get; }
    Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default);
}
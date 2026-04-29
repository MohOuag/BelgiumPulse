using BelgiumPulse.Domain.Entities;
using BelgiumPulse.Domain.ValueObjects;
using BelgiumPulse.Infrastructure.ExternalApis.Common;
using BelgiumPulse.Infrastructure.ExternalApis.Weather.Models;
using Microsoft.Extensions.Logging;
using Polly;
using System.Net.Http.Json;

namespace BelgiumPulse.Infrastructure.ExternalApis.Weather;

public class IrmApiService : IExternalApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<IrmApiService> _logger;
    private readonly ResiliencePipeline<List<WeatherAlert>> _pipeline;

    public string ServiceName => "IRM Météo";

    public IrmApiService(
        HttpClient httpClient,
        ILogger<IrmApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _pipeline = ResiliencePipelineFactory
            .Create<List<WeatherAlert>>(ServiceName, logger);
    }

    public async Task<List<WeatherAlert>> GetActiveAlertsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _pipeline.ExecuteAsync(async ct =>
        {
            _logger.LogInformation("Appel API IRM — récupération des alertes météo");

            var response = await _httpClient.GetFromJsonAsync<IrmApiResponse>(
                "warnings/active", ct);

            if (response is null)
                return new List<WeatherAlert>();

            return response.Warnings
                .Where(w => w.ValidUntil > DateTime.UtcNow)
                .Select(dto => WeatherAlert.Create(
                    dto.Title,
                    dto.Description,
                    dto.Level,
                    dto.Region,
                    new Coordinates(dto.Latitude, dto.Longitude),
                    dto.ValidFrom,
                    dto.ValidUntil))
                .ToList();

        }, cancellationToken);
    }

    public async Task<bool> IsAvailableAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("health", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
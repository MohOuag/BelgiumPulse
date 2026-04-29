using BelgiumPulse.Domain.Entities;
using BelgiumPulse.Domain.ValueObjects;
using BelgiumPulse.Infrastructure.ExternalApis.AirQuality.Models;
using BelgiumPulse.Infrastructure.ExternalApis.Common;
using Microsoft.Extensions.Logging;
using Polly;
using System.Net.Http.Json;

namespace BelgiumPulse.Infrastructure.ExternalApis.AirQuality;

public class BelAqiApiService : IExternalApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BelAqiApiService> _logger;
    private readonly ResiliencePipeline<List<AirQualityMeasurement>> _pipeline;

    public string ServiceName => "BelAQI";

    public BelAqiApiService(
        HttpClient httpClient,
        ILogger<BelAqiApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _pipeline = ResiliencePipelineFactory
            .Create<List<AirQualityMeasurement>>(ServiceName, logger);
    }

    public async Task<List<AirQualityMeasurement>> GetMeasurementsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _pipeline.ExecuteAsync(async ct =>
        {
            _logger.LogInformation("Appel API BelAQI — récupération qualité de l'air");

            var response = await _httpClient.GetFromJsonAsync<BelAqiApiResponse>(
                "stations/current", ct);

            if (response is null)
                return new List<AirQualityMeasurement>();

            return response.Stations.Select(dto =>
                AirQualityMeasurement.Create(
                    dto.Name,
                    dto.Municipality,
                    new Coordinates(dto.Latitude, dto.Longitude),
                    dto.BelAqiIndex,
                    dto.Pm25,
                    dto.Pm10,
                    dto.No2))
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
using BelgiumPulse.Domain.Entities;
using BelgiumPulse.Domain.ValueObjects;
using BelgiumPulse.Infrastructure.ExternalApis.Common;
using BelgiumPulse.Infrastructure.ExternalApis.Stib.Models;
using Microsoft.Extensions.Logging;
using Polly;
using System.Net.Http.Json;

namespace BelgiumPulse.Infrastructure.ExternalApis.Stib;

public class StibApiService : IExternalApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<StibApiService> _logger;
    private readonly ResiliencePipeline<List<StibLine>> _pipeline;

    public string ServiceName => "STIB/MIVB";

    public StibApiService(
        HttpClient httpClient,
        ILogger<StibApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _pipeline = ResiliencePipelineFactory
            .Create<List<StibLine>>(ServiceName, logger);
    }

    public async Task<List<StibLine>> GetLinesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _pipeline.ExecuteAsync(async ct =>
        {
            _logger.LogInformation("Appel API STIB — récupération des lignes");

            // Appel réel à l'API STIB Open Data
            var response = await _httpClient.GetFromJsonAsync<StibApiResponse>(
                "lines", ct);

            if (response is null)
                return new List<StibLine>();

            // Mapping API Response → Domain Entity
            return response.Lines.Select(dto =>
            {
                var line = StibLine.Create(dto.Id, dto.Name, dto.Mode);

                if (dto.HasDisruption && dto.DisruptionDescription is not null)
                    line.ReportDisruption(dto.DisruptionDescription);

                if (dto.Latitude.HasValue && dto.Longitude.HasValue)
                    line.UpdatePosition(
                        new Coordinates(dto.Latitude.Value, dto.Longitude.Value));

                return line;
            }).ToList();

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
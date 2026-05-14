using MediatR;
using BelgiumPulse.Domain.Interfaces;
using BelgiumPulse.Application.Common;

namespace BelgiumPulse.Application.Stib.Queries.GetAllStibLines;

public class GetAllStibLinesHandler
    : IRequestHandler<GetAllStibLinesQuery, List<StibLineDto>>
{
    private readonly IStibRepository _repository;
    private readonly ICacheService _cache;

    public GetAllStibLinesHandler(IStibRepository repository, ICacheService cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<List<StibLineDto>> Handle(
        GetAllStibLinesQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = request.OnlyDisrupted
            ? CacheKeys.DisruptedStibLines
            : CacheKeys.AllStibLines;

        return await _cache.GetOrSetAsync(
            cacheKey,
            async () =>
            {
                var lines = request.OnlyDisrupted
                    ? await _repository.GetDisruptedLinesAsync(cancellationToken)
                    : await _repository.GetAllAsync(cancellationToken);

                return lines.Select(l => new StibLineDto(
                    l.Id,
                    l.LineNumber,
                    l.LineName,
                    l.TransportType,
                    l.IsOperational,
                    l.DisruptionMessage,
                    l.CurrentPosition?.Latitude,
                    l.CurrentPosition?.Longitude,
                    l.LastUpdated
                )).ToList();
            },
            expiration: TimeSpan.FromMinutes(5), // nouvelles données toutes les 5 min
            cancellationToken: cancellationToken);
    }
}
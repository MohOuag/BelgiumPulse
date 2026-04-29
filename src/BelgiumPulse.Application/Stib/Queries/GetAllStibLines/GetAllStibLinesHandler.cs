using MediatR;
using BelgiumPulse.Domain.Interfaces;

namespace BelgiumPulse.Application.Stib.Queries.GetAllStibLines;

public class GetAllStibLinesHandler
    : IRequestHandler<GetAllStibLinesQuery, List<StibLineDto>>
{
    private readonly IStibRepository _repository;

    public GetAllStibLinesHandler(IStibRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<StibLineDto>> Handle(
        GetAllStibLinesQuery request,
        CancellationToken cancellationToken)
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
    }
}
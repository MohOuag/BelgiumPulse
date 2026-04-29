using MediatR;
using BelgiumPulse.Application.Common.Exceptions;
using BelgiumPulse.Application.Stib.Queries.GetAllStibLines;
using BelgiumPulse.Domain.Interfaces;

namespace BelgiumPulse.Application.Stib.Queries.GetStibLineByNumber;

public class GetStibLineByNumberHandler
    : IRequestHandler<GetStibLineByNumberQuery, StibLineDto>
{
    private readonly IStibRepository _repository;

    public GetStibLineByNumberHandler(IStibRepository repository)
    {
        _repository = repository;
    }

    public async Task<StibLineDto> Handle(
        GetStibLineByNumberQuery request,
        CancellationToken cancellationToken)
    {
        var line = await _repository
            .GetByLineNumberAsync(request.LineNumber, cancellationToken);

        if (line is null)
            throw new NotFoundException(nameof(line), request.LineNumber);

        return new StibLineDto(
            line.Id,
            line.LineNumber,
            line.LineName,
            line.TransportType,
            line.IsOperational,
            line.DisruptionMessage,
            line.CurrentPosition?.Latitude,
            line.CurrentPosition?.Longitude,
            line.LastUpdated
        );
    }
}
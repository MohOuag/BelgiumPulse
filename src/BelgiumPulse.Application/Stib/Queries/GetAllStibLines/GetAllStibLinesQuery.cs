using MediatR;

namespace BelgiumPulse.Application.Stib.Queries.GetAllStibLines;

public record GetAllStibLinesQuery(
    bool OnlyDisrupted = false
) : IRequest<List<StibLineDto>>;
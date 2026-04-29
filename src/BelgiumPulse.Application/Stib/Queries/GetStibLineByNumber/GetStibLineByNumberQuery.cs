using MediatR;
using BelgiumPulse.Application.Stib.Queries.GetAllStibLines;

namespace BelgiumPulse.Application.Stib.Queries.GetStibLineByNumber;

public record GetStibLineByNumberQuery(string LineNumber)
    : IRequest<StibLineDto>;
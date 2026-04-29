using MediatR;

namespace BelgiumPulse.Application.Stib.Commands.ReportDisruption;

public record ReportDisruptionCommand(
    string LineNumber,
    string Message
) : IRequest<bool>;
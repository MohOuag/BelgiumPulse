using BelgiumPulse.Domain.Common;

namespace BelgiumPulse.Domain.Events;

public sealed record StibDisruptionDetected : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
    public Guid LineId { get; }
    public string LineNumber { get; }
    public string DisruptionMessage { get; }

    public StibDisruptionDetected(Guid lineId, string lineNumber, string disruptionMessage)
    {
        LineId = lineId;
        LineNumber = lineNumber;
        DisruptionMessage = disruptionMessage;
    }
}
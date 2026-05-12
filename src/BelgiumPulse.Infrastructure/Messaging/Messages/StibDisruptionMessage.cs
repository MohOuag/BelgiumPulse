namespace BelgiumPulse.Infrastructure.Messaging.Messages;

public record StibDisruptionMessage(
    Guid LineId,
    string LineNumber,
    string DisruptionMessage,
    DateTime OccurredAt
);
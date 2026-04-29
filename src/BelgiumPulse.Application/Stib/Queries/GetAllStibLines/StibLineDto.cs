namespace BelgiumPulse.Application.Stib.Queries.GetAllStibLines;

public record StibLineDto(
    Guid Id,
    string LineNumber,
    string LineName,
    string TransportType,
    bool IsOperational,
    string? DisruptionMessage,
    double? Latitude,
    double? Longitude,
    DateTime LastUpdated
);
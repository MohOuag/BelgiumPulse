using BelgiumPulse.Domain.Common;
using BelgiumPulse.Domain.Events;
using BelgiumPulse.Domain.ValueObjects;

namespace BelgiumPulse.Domain.Entities;

public class StibLine : BaseEntity
{
    public string LineNumber { get; private set; }
    public string LineName { get; private set; }
    public string TransportType { get; private set; } // Bus, Tram, Metro
    public bool IsOperational { get; private set; }
    public string? DisruptionMessage { get; private set; }
    public Coordinates? CurrentPosition { get; private set; }
    public DateTime LastUpdated { get; private set; }

    private StibLine() { } // EF Core

    public static StibLine Create(string lineNumber, string lineName, string transportType)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(lineNumber);
        ArgumentException.ThrowIfNullOrWhiteSpace(lineName);

        return new StibLine
        {
            LineNumber = lineNumber,
            LineName = lineName,
            TransportType = transportType,
            IsOperational = true,
            LastUpdated = DateTime.UtcNow
        };
    }

    public void ReportDisruption(string message)
    {
        IsOperational = false;
        DisruptionMessage = message;
        UpdatedAt = DateTime.UtcNow;
        LastUpdated = DateTime.UtcNow;

        AddDomainEvent(new StibDisruptionDetected(Id, LineNumber, message));
    }

    public void ResumeService()
    {
        IsOperational = true;
        DisruptionMessage = null;
        UpdatedAt = DateTime.UtcNow;
        LastUpdated = DateTime.UtcNow;
    }

    public void UpdatePosition(Coordinates position)
    {
        CurrentPosition = position;
        LastUpdated = DateTime.UtcNow;
    }
}
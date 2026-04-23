using BelgiumPulse.Domain.Common;
using BelgiumPulse.Domain.ValueObjects;

namespace BelgiumPulse.Domain.Entities;

public class WeatherAlert : BaseEntity
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public string Severity { get; private set; } // Green, Yellow, Orange, Red
    public string Region { get; private set; }
    public Coordinates Location { get; private set; }
    public DateTime ValidFrom { get; private set; }
    public DateTime ValidUntil { get; private set; }
    public bool IsActive => DateTime.UtcNow >= ValidFrom && DateTime.UtcNow <= ValidUntil;

    private WeatherAlert() { }

    public static WeatherAlert Create(
        string title,
        string description,
        string severity,
        string region,
        Coordinates location,
        DateTime validFrom,
        DateTime validUntil)
    {
        if (validUntil <= validFrom)
            throw new ArgumentException("ValidUntil must be after ValidFrom");

        return new WeatherAlert
        {
            Title = title,
            Description = description,
            Severity = severity,
            Region = region,
            Location = location,
            ValidFrom = validFrom,
            ValidUntil = validUntil
        };
    }
}
using BelgiumPulse.Domain.Common;

namespace BelgiumPulse.Domain.Entities;

public class UserAlert : BaseEntity
{
    public string UserId { get; private set; }
    public string AlertType { get; private set; } // StibDisruption, WeatherAlert, AirQuality
    public string TargetId { get; private set; } // LineNumber ou Municipality
    public bool IsActive { get; private set; }
    public DateTime? LastTriggeredAt { get; private set; }

    private UserAlert() { }

    public static UserAlert Create(string userId, string alertType, string targetId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        ArgumentException.ThrowIfNullOrWhiteSpace(targetId);

        return new UserAlert
        {
            UserId = userId,
            AlertType = alertType,
            TargetId = targetId,
            IsActive = true
        };
    }

    public void Trigger()
    {
        LastTriggeredAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
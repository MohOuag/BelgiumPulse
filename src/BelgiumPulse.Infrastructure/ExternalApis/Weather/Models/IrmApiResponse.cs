namespace BelgiumPulse.Infrastructure.ExternalApis.Weather.Models;

public class IrmApiResponse
{
    public List<IrmAlertDto> Warnings { get; set; } = new();
}

public class IrmAlertDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty; // green, yellow, orange, red
    public string Region { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidUntil { get; set; }
}
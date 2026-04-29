namespace BelgiumPulse.Infrastructure.ExternalApis.Stib.Models;

public class StibApiResponse
{
    public List<StibLineDto> Lines { get; set; } = new();
}

public class StibLineDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Mode { get; set; } = string.Empty; // TRAM, BUS, METRO
    public bool HasDisruption { get; set; }
    public string? DisruptionDescription { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}
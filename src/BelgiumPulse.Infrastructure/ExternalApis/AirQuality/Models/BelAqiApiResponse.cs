namespace BelgiumPulse.Infrastructure.ExternalApis.AirQuality.Models;

public class BelAqiApiResponse
{
    public List<BelAqiStationDto> Stations { get; set; } = new();
}

public class BelAqiStationDto
{
    public string Name { get; set; } = string.Empty;
    public string Municipality { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int BelAqiIndex { get; set; }
    public double Pm25 { get; set; }
    public double Pm10 { get; set; }
    public double No2 { get; set; }
}
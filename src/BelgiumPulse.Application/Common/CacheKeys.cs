
namespace BelgiumPulse.Application.Common
{
    public static class CacheKeys
    {
        public static string AllStibLines => "stib:lines:all";
        public static string DisruptedStibLines => "stib:lines:disrupted";
        public static string StibLineByNumber(string number) => $"stib:lines:{number}";
        public static string ActiveWeatherAlerts => "weather:alerts:active";
        public static string WeatherAlertsByRegion(string region) => $"weather:alerts:{region}";
        public static string LatestAirQuality => "airquality:latest";
        public static string AirQualityByMunicipality(string municipality) => $"airquality:{municipality}";
    }
}

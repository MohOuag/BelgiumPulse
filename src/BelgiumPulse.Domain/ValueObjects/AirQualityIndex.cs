namespace BelgiumPulse.Domain.ValueObjects;

public sealed record AirQualityIndex
{
    public int Value { get; }
    public string Level { get; }

    // BelAQI scale : 1 (excellent) → 10 (horrible)
    public AirQualityIndex(int value)
    {
        if (value < 1 || value > 10)
            throw new ArgumentOutOfRangeException(nameof(value), "BelAQI index must be between 1 and 10");

        Value = value;
        Level = value switch
        {
            1 => "Excellent",
            2 => "Very Good",
            3 => "Good",
            4 => "Fairly Good",
            5 => "Moderate",
            6 => "Poor",
            7 => "Very Poor",
            8 => "Bad",
            9 => "Very Bad",
            10 => "Horrible",
            _ => "Unknown"
        };
    }

    public bool IsHealthRisk => Value >= 7;
    public override string ToString() => $"{Level} ({Value}/10)";
}
using MediatR;

namespace BelgiumPulse.Application.AirQuality.Queries.GetAirQualityMeasurements;

public record GetAirQualityMeasurementsQuery(
    string? Municipality = null
) : IRequest<List<AirQualityMeasurementDto>>;
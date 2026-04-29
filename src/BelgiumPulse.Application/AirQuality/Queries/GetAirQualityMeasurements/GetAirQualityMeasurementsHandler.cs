using MediatR;
using BelgiumPulse.Domain.Interfaces;

namespace BelgiumPulse.Application.AirQuality.Queries.GetAirQualityMeasurements;

public class GetAirQualityMeasurementsHandler
    : IRequestHandler<GetAirQualityMeasurementsQuery, List<AirQualityMeasurementDto>>
{
    private readonly IAirQualityRepository _repository;

    public GetAirQualityMeasurementsHandler(IAirQualityRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<AirQualityMeasurementDto>> Handle(
        GetAirQualityMeasurementsQuery request,
        CancellationToken cancellationToken)
    {
        var measurements = request.Municipality is not null
            ? await _repository.GetByMunicipalityAsync(
                request.Municipality, cancellationToken)
            : await _repository.GetLatestMeasurementsAsync(cancellationToken);

        return measurements.Select(m => new AirQualityMeasurementDto(
            m.Id,
            m.StationName,
            m.Municipality,
            m.Location.Latitude,
            m.Location.Longitude,
            m.Index.Value,
            m.Index.Level,
            m.PM25,
            m.PM10,
            m.NO2,
            m.Index.IsHealthRisk,
            m.MeasuredAt
        )).ToList();
    }
}
using BelgiumPulse.Domain.Interfaces;
using BelgiumPulse.Infrastructure.Persistence;
using BelgiumPulse.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace BelgiumPulse.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services)
    {
        services.AddScoped<IStibRepository, StibRepository>();
        services.AddScoped<IWeatherRepository, WeatherRepository>();
        services.AddScoped<IAirQualityRepository, AirQualityRepository>();
        services.AddScoped<IUserAlertRepository, UserAlertRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
using BelgiumPulse.Domain.Interfaces;
using BelgiumPulse.Infrastructure.Caching;
using BelgiumPulse.Infrastructure.ExternalApis.AirQuality;
using BelgiumPulse.Infrastructure.ExternalApis.Stib;
using BelgiumPulse.Infrastructure.ExternalApis.Weather;
using BelgiumPulse.Infrastructure.Messaging.Consumers;
using BelgiumPulse.Infrastructure.Messaging.Publishers;
using BelgiumPulse.Infrastructure.Persistence;
using BelgiumPulse.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using StackExchange.Redis;

namespace BelgiumPulse.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IStibRepository, StibRepository>();
        services.AddScoped<IWeatherRepository, WeatherRepository>();
        services.AddScoped<IAirQualityRepository, AirQualityRepository>();
        services.AddScoped<IUserAlertRepository, UserAlertRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // HttpClients pour les APIs externes
        services.AddHttpClient<StibApiService>(client =>
        {
            client.BaseAddress = new Uri(
                configuration["ExternalApis:Stib:BaseUrl"]
                ?? "https://data.stib-mivb.be/api/");
            client.Timeout = TimeSpan.FromSeconds(15);
        });

        services.AddHttpClient<IrmApiService>(client =>
        {
            client.BaseAddress = new Uri(
                configuration["ExternalApis:Irm:BaseUrl"]
                ?? "https://api.meteo.be/v1/");
            client.Timeout = TimeSpan.FromSeconds(15);
        });

        services.AddHttpClient<BelAqiApiService>(client =>
        {
            client.BaseAddress = new Uri(
                configuration["ExternalApis:BelAqi:BaseUrl"]
                ?? "https://api.irceline.be/v1/");
            client.Timeout = TimeSpan.FromSeconds(15);
        });

        // RabbitMQ Connection
        services.AddSingleton<IConnection>(sp =>
        {
            var factory = new ConnectionFactory
            {
                HostName = configuration["RabbitMq:Host"] ?? "localhost",
                Port = int.Parse(configuration["RabbitMq:Port"] ?? "5672"),
                UserName = configuration["RabbitMq:Username"] ?? "guest",
                Password = configuration["RabbitMq:Password"] ?? "guest"
            };
            return factory.CreateConnectionAsync().GetAwaiter().GetResult();
        });

        // Publisher
        services.AddSingleton<IMessagePublisher, RabbitMqPublisher>();

        // Consumers — BackgroundService
        services.AddHostedService<StibDisruptionConsumer>();
        services.AddHostedService<AirQualityAlertConsumer>();

        // Redis
        var redisConnection = configuration["Redis:ConnectionString"]
            ?? "localhost:6379";

        services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(redisConnection));

        services.AddSingleton<ICacheService, RedisCacheService>();

        return services;
    }
}
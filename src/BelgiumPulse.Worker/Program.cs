using BelgiumPulse.Infrastructure;
using BelgiumPulse.Infrastructure.ExternalApis.Stib;
using BelgiumPulse.Infrastructure.ExternalApis.Weather;
using BelgiumPulse.Infrastructure.ExternalApis.AirQuality;
using BelgiumPulse.Infrastructure.Persistence;
using BelgiumPulse.Worker.Workers;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

// Infrastructure — repositories, cache, RabbitMQ
builder.Services.AddInfrastructure(builder.Configuration);

// Base de données
builder.Services.AddDbContext<BelgiumPulseDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Workers
builder.Services.AddHostedService<StibDataWorker>();
builder.Services.AddHostedService<WeatherDataWorker>();
builder.Services.AddHostedService<AirQualityDataWorker>();

var host = builder.Build();
host.Run();
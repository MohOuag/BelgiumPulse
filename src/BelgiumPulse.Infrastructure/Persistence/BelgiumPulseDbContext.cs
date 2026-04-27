using BelgiumPulse.Domain.Entities;
using BelgiumPulse.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Polly;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace BelgiumPulse.Infrastructure.Persistence;

public class BelgiumPulseDbContext : DbContext
{
    public BelgiumPulseDbContext(DbContextOptions<BelgiumPulseDbContext> options)
        : base(options) { }

    public DbSet<StibLine> StibLines => Set<StibLine>();
    public DbSet<WeatherAlert> WeatherAlerts => Set<WeatherAlert>();
    public DbSet<AirQualityMeasurement> AirQualityMeasurements => Set<AirQualityMeasurement>();
    public DbSet<UserAlert> UserAlerts => Set<UserAlert>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Applique automatiquement toutes les configurations
        // du même assembly — plus besoin de les lister une par une
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(new AuditInterceptor());
        base.OnConfiguring(optionsBuilder);
    }
}
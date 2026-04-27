using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BelgiumPulse.Domain.Entities;
using BelgiumPulse.Domain.ValueObjects;

namespace BelgiumPulse.Infrastructure.Persistence.Configurations;

public class AirQualityMeasurementConfiguration
    : IEntityTypeConfiguration<AirQualityMeasurement>
{
    public void Configure(EntityTypeBuilder<AirQualityMeasurement> builder)
    {
        builder.ToTable("AirQualityMeasurements");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.StationName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Municipality)
            .IsRequired()
            .HasMaxLength(100);

        // Value Converter — stocke juste l'int en base
        builder.Property(a => a.Index)
            .HasConversion(
                v => v.Value,
                v => new AirQualityIndex(v))
            .HasColumnName("AqiValue");

        builder.Property(a => a.PM25)
            .HasColumnType("decimal(6,2)")
            .HasColumnName("PM25");

        builder.Property(a => a.PM10)
            .HasColumnType("decimal(6,2)")
            .HasColumnName("PM10");

        builder.Property(a => a.NO2)
            .HasColumnType("decimal(6,2)")
            .HasColumnName("NO2");

        // Owned Entity — Coordinates
        builder.OwnsOne(a => a.Location, coords =>
        {
            coords.Property(c => c.Latitude)
                  .HasColumnName("Latitude")
                  .HasColumnType("decimal(10,7)");

            coords.Property(c => c.Longitude)
                  .HasColumnName("Longitude")
                  .HasColumnType("decimal(10,7)");
        });

        // Index pour les requêtes historiques
        builder.HasIndex(a => new { a.StationName, a.MeasuredAt });
        builder.HasIndex(a => a.Municipality);

        builder.Ignore(a => a.DomainEvents);
    }
}
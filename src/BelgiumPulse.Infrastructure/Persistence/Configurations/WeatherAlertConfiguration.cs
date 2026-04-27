using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BelgiumPulse.Domain.Entities;

namespace BelgiumPulse.Infrastructure.Persistence.Configurations;

public class WeatherAlertConfiguration : IEntityTypeConfiguration<WeatherAlert>
{
    public void Configure(EntityTypeBuilder<WeatherAlert> builder)
    {
        builder.ToTable("WeatherAlerts");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(w => w.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(w => w.Severity)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(w => w.Region)
            .IsRequired()
            .HasMaxLength(100);

        // Owned Entity — Coordinates
        builder.OwnsOne(w => w.Location, coords =>
        {
            coords.Property(c => c.Latitude)
                  .HasColumnName("Latitude")
                  .HasColumnType("decimal(10,7)");

            coords.Property(c => c.Longitude)
                  .HasColumnName("Longitude")
                  .HasColumnType("decimal(10,7)");
        });

        // Index pour filtrer les alertes actives rapidement
        builder.HasIndex(w => new { w.ValidFrom, w.ValidUntil });
        builder.HasIndex(w => w.Region);

        builder.Ignore(w => w.DomainEvents);

        // IsActive est calculé — pas persisté en base
        builder.Ignore(w => w.IsActive);
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BelgiumPulse.Domain.Entities;

namespace BelgiumPulse.Infrastructure.Persistence.Configurations;

public class StibLineConfiguration : IEntityTypeConfiguration<StibLine>
{
    public void Configure(EntityTypeBuilder<StibLine> builder)
    {
        builder.ToTable("StibLines");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.LineNumber)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(s => s.LineName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.TransportType)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(s => s.DisruptionMessage)
            .HasMaxLength(500);

        // Owned Entity — Coordinates aplati dans la même table
        builder.OwnsOne(s => s.CurrentPosition, coords =>
        {
            coords.Property(c => c.Latitude)
                  .HasColumnName("Latitude")
                  .HasColumnType("decimal(10,7)");

            coords.Property(c => c.Longitude)
                  .HasColumnName("Longitude")
                  .HasColumnType("decimal(10,7)");
        });

        // Index pour accélérer les recherches par numéro de ligne
        builder.HasIndex(s => s.LineNumber)
               .IsUnique();

        // Ignore les Domain Events — pas de persistance
        builder.Ignore(s => s.DomainEvents);
    }
}
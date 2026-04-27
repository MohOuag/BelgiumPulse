using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BelgiumPulse.Domain.Entities;

namespace BelgiumPulse.Infrastructure.Persistence.Configurations;

public class UserAlertConfiguration : IEntityTypeConfiguration<UserAlert>
{
    public void Configure(EntityTypeBuilder<UserAlert> builder)
    {
        builder.ToTable("UserAlerts");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.UserId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.AlertType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.TargetId)
            .IsRequired()
            .HasMaxLength(100);

        // Index pour retrouver rapidement les alertes d'un user
        builder.HasIndex(u => u.UserId);
        builder.HasIndex(u => new { u.AlertType, u.TargetId, u.IsActive });

        builder.Ignore(u => u.DomainEvents);
    }
}
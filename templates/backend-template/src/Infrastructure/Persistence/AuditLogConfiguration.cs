using EnterpriseTemplate.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseTemplate.Infrastructure.Persistence;

/// <summary>
/// Entity Framework configuration for AuditLog entity
/// </summary>
public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.EntityType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.EntityId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Action)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.UserId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.UserName)
            .HasMaxLength(200);

        builder.Property(x => x.IpAddress)
            .HasMaxLength(45); // IPv6 max length

        builder.Property(x => x.UserAgent)
            .HasMaxLength(500);

        builder.Property(x => x.ChangeReason)
            .HasMaxLength(1000);

        // JSON columns for old/new values
        builder.Property(x => x.OldValues)
            .HasColumnType("jsonb"); // PostgreSQL jsonb type

        builder.Property(x => x.NewValues)
            .HasColumnType("jsonb"); // PostgreSQL jsonb type

        // Indexes for common query patterns
        builder.HasIndex(x => new { x.EntityType, x.EntityId })
            .HasDatabaseName("IX_AuditLogs_Entity");

        builder.HasIndex(x => x.Timestamp)
            .HasDatabaseName("IX_AuditLogs_Timestamp");

        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("IX_AuditLogs_UserId");

        builder.HasIndex(x => x.Action)
            .HasDatabaseName("IX_AuditLogs_Action");
    }
}

using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using EnterpriseTemplate.Domain.Abstractions;
using EnterpriseTemplate.Domain.Entities;
using EnterpriseTemplate.Application.Abstractions;

namespace EnterpriseTemplate.Infrastructure.Auditing;

/// <summary>
/// EF Core interceptor that automatically captures audit information when entities are saved
/// </summary>
public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUser _currentUser;
    private readonly TimeProvider _timeProvider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditInterceptor(
        ICurrentUser currentUser,
        TimeProvider timeProvider,
        IHttpContextAccessor httpContextAccessor)
    {
        _currentUser = currentUser;
        _timeProvider = timeProvider;
        _httpContextAccessor = httpContextAccessor;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        ApplyAuditInformation(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ApplyAuditInformation(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ApplyAuditInformation(DbContext? context)
    {
        if (context == null) return;

        var now = _timeProvider.GetUtcNow();
        var userId = _currentUser.Id ?? "system";
        var userName = GetUserName();
        var ipAddress = GetClientIpAddress();
        var userAgent = GetUserAgent();

        var auditEntries = new List<AuditEntry>();

        // Process each changed entity
        foreach (var entry in context.ChangeTracker.Entries())
        {
            // Skip audit log entries to prevent infinite loops
            if (entry.Entity is AuditLog) continue;

            // Apply auditable information
            if (entry.Entity is IAuditable auditable)
            {
                ApplyAuditableChanges(entry, auditable, now, userId);
            }

            // Apply soft delete information
            if (entry.Entity is ISoftDeletable softDeletable && entry.State == EntityState.Modified)
            {
                ApplySoftDeleteChanges(entry, softDeletable, now, userId);
            }

            // Create audit log entries for tracked changes
            if (ShouldAudit(entry))
            {
                auditEntries.Add(CreateAuditEntry(entry, now, userId, userName, ipAddress, userAgent));
            }
        }

        // Add audit log entries to the context
        foreach (var auditEntry in auditEntries)
        {
            context.Set<AuditLog>().Add(auditEntry.ToAuditLog());
        }
    }

    private static void ApplyAuditableChanges(EntityEntry entry, IAuditable auditable, DateTimeOffset now, string userId)
    {
        switch (entry.State)
        {
            case EntityState.Added:
                auditable.CreatedAt = now;
                auditable.CreatedBy = userId;
                auditable.UpdatedAt = now;
                auditable.UpdatedBy = userId;
                break;

            case EntityState.Modified:
                auditable.UpdatedAt = now;
                auditable.UpdatedBy = userId;
                break;
        }
    }

    private static void ApplySoftDeleteChanges(EntityEntry entry, ISoftDeletable softDeletable, DateTimeOffset now, string userId)
    {
        // Check if this is a soft delete operation (DeletedAt changed from null to a value)
        var deletedAtProperty = entry.Property(nameof(ISoftDeletable.DeletedAt));
        if (deletedAtProperty.IsModified &&
            deletedAtProperty.OriginalValue == null &&
            deletedAtProperty.CurrentValue != null)
        {
            softDeletable.DeletedAt = now;
            softDeletable.DeletedBy = userId;
        }
    }

    private static bool ShouldAudit(EntityEntry entry)
    {
        return entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted;
    }

    private static AuditEntry CreateAuditEntry(
        EntityEntry entry,
        DateTimeOffset timestamp,
        string userId,
        string? userName,
        string? ipAddress,
        string? userAgent)
    {
        var entityType = entry.Entity.GetType().Name;
        var entityId = GetEntityId(entry);
        var action = GetAuditAction(entry);

        return new AuditEntry
        {
            EntityType = entityType,
            EntityId = entityId,
            Action = action,
            OldValues = action == "DELETE" ? SerializeEntity(entry.OriginalValues) :
                       action == "UPDATE" ? SerializeEntity(entry.OriginalValues) : null,
            NewValues = action == "CREATE" ? SerializeEntity(entry.CurrentValues) :
                       action == "UPDATE" ? SerializeEntity(entry.CurrentValues) : null,
            Timestamp = timestamp,
            UserId = userId,
            UserName = userName,
            IpAddress = ipAddress,
            UserAgent = userAgent
        };
    }

    private static string GetEntityId(EntityEntry entry)
    {
        var keyValues = entry.Properties
            .Where(p => p.Metadata.IsPrimaryKey())
            .Select(p => p.CurrentValue?.ToString())
            .Where(v => v != null);

        return string.Join(",", keyValues);
    }

    private static string GetAuditAction(EntityEntry entry)
    {
        return entry.State switch
        {
            EntityState.Added => "CREATE",
            EntityState.Modified => "UPDATE",
            EntityState.Deleted => "DELETE",
            _ => "UNKNOWN"
        };
    }

    private static string? SerializeEntity(PropertyValues propertyValues)
    {
        var values = new Dictionary<string, object?>();

        foreach (var property in propertyValues.Properties)
        {
            var value = propertyValues[property];
            values[property.Name] = value;
        }

        return JsonSerializer.Serialize(values, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        });
    }

    private string? GetUserName()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        return httpContext?.User?.Identity?.Name;
    }

    private string? GetClientIpAddress()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return null;

        // Check for forwarded IP first (in case of proxy/load balancer)
        var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        return httpContext.Connection.RemoteIpAddress?.ToString();
    }

    private string? GetUserAgent()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        return httpContext?.Request.Headers["User-Agent"].FirstOrDefault();
    }
}

/// <summary>
/// Internal class to hold audit information before converting to AuditLog entity
/// </summary>
internal class AuditEntry
{
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }

    public AuditLog ToAuditLog()
    {
        return new AuditLog
        {
            Id = Guid.NewGuid(),
            EntityType = EntityType,
            EntityId = EntityId,
            Action = Action,
            OldValues = OldValues,
            NewValues = NewValues,
            Timestamp = Timestamp,
            UserId = UserId,
            UserName = UserName,
            IpAddress = IpAddress,
            UserAgent = UserAgent
        };
    }
}

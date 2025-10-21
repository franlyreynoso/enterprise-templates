using EnterpriseTemplate.Domain.Entities;

namespace EnterpriseTemplate.Application.Auditing;

/// <summary>
/// Data transfer object for audit log information
/// </summary>
public sealed class AuditLogDto
{
    public Guid Id { get; set; }
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
    public string? ChangeReason { get; set; }

    public static AuditLogDto FromEntity(AuditLog auditLog)
    {
        return new AuditLogDto
        {
            Id = auditLog.Id,
            EntityType = auditLog.EntityType,
            EntityId = auditLog.EntityId,
            Action = auditLog.Action,
            OldValues = auditLog.OldValues,
            NewValues = auditLog.NewValues,
            Timestamp = auditLog.Timestamp,
            UserId = auditLog.UserId,
            UserName = auditLog.UserName,
            IpAddress = auditLog.IpAddress,
            UserAgent = auditLog.UserAgent,
            ChangeReason = auditLog.ChangeReason
        };
    }
}

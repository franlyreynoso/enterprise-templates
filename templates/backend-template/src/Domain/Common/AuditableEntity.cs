using EnterpriseTemplate.Domain.Abstractions;

namespace EnterpriseTemplate.Domain.Common;

/// <summary>
/// Base class for entities that support audit tracking
/// </summary>
public abstract class AuditableEntity : IAuditable
{
    public DateTimeOffset CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTimeOffset UpdatedAt { get; set; }
    public string UpdatedBy { get; set; } = string.Empty;
}

/// <summary>
/// Base class for entities that support audit tracking and soft deletion
/// </summary>
public abstract class AuditableSoftDeletableEntity : AuditableEntity, ISoftDeletable
{
    public DateTimeOffset? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}

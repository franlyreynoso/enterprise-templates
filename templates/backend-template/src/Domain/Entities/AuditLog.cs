namespace EnterpriseTemplate.Domain.Entities;

/// <summary>
/// Entity representing an audit log entry
/// </summary>
public class AuditLog
{
    public Guid Id { get; set; }

    /// <summary>
    /// The type of entity that was modified (e.g., "Todo", "Order")
    /// </summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>
    /// The ID of the entity that was modified
    /// </summary>
    public string EntityId { get; set; } = string.Empty;

    /// <summary>
    /// The action performed (CREATE, UPDATE, DELETE)
    /// </summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// JSON representation of the entity before the change (null for CREATE)
    /// </summary>
    public string? OldValues { get; set; }

    /// <summary>
    /// JSON representation of the entity after the change (null for DELETE)
    /// </summary>
    public string? NewValues { get; set; }

    /// <summary>
    /// When the change occurred
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// ID of the user who made the change
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Name of the user who made the change (for display purposes)
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// IP address of the client making the change
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// User agent of the client making the change
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// Additional context or reason for the change
    /// </summary>
    public string? ChangeReason { get; set; }
}

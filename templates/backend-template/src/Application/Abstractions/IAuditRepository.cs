using EnterpriseTemplate.Domain.Entities;

namespace EnterpriseTemplate.Application.Abstractions;

/// <summary>
/// Repository interface for audit log operations
/// </summary>
public interface IAuditRepository
{
    /// <summary>
    /// Get audit trail for a specific entity
    /// </summary>
    Task<(List<AuditLog> AuditLogs, int TotalCount)> GetEntityAuditTrailAsync(
        string entityType,
        string entityId,
        int pageSize,
        int pageNumber,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get audit trail for a specific user
    /// </summary>
    Task<(List<AuditLog> AuditLogs, int TotalCount)> GetUserAuditTrailAsync(
        string userId,
        int pageSize,
        int pageNumber,
        DateTimeOffset? fromDate = null,
        DateTimeOffset? toDate = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get recent audit activities
    /// </summary>
    Task<(List<AuditLog> AuditLogs, int TotalCount)> GetRecentAuditActivitiesAsync(
        int pageSize,
        int pageNumber,
        string? entityType = null,
        string? action = null,
        CancellationToken cancellationToken = default);
}

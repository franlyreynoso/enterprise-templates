using MediatR;

namespace EnterpriseTemplate.Application.Auditing;

/// <summary>
/// Query to get audit trail for a specific entity
/// </summary>
/// <param name="EntityType">The type of entity (e.g., "Todo")</param>
/// <param name="EntityId">The ID of the entity</param>
/// <param name="PageSize">Number of records per page (default 50, max 100)</param>
/// <param name="PageNumber">Page number (1-based)</param>
public sealed record GetEntityAuditTrail(
    string EntityType,
    string EntityId,
    int PageSize = 50,
    int PageNumber = 1
) : IRequest<AuditTrailResponse>;

/// <summary>
/// Query to get audit trail for a specific user
/// </summary>
/// <param name="UserId">The ID of the user</param>
/// <param name="PageSize">Number of records per page (default 50, max 100)</param>
/// <param name="PageNumber">Page number (1-based)</param>
/// <param name="FromDate">Optional start date filter</param>
/// <param name="ToDate">Optional end date filter</param>
public sealed record GetUserAuditTrail(
    string UserId,
    int PageSize = 50,
    int PageNumber = 1,
    DateTimeOffset? FromDate = null,
    DateTimeOffset? ToDate = null
) : IRequest<AuditTrailResponse>;

/// <summary>
/// Query to get recent audit activities
/// </summary>
/// <param name="PageSize">Number of records per page (default 20, max 100)</param>
/// <param name="PageNumber">Page number (1-based)</param>
/// <param name="EntityType">Optional filter by entity type</param>
/// <param name="Action">Optional filter by action (CREATE, UPDATE, DELETE)</param>
public sealed record GetRecentAuditActivities(
    int PageSize = 20,
    int PageNumber = 1,
    string? EntityType = null,
    string? Action = null
) : IRequest<AuditTrailResponse>;

/// <summary>
/// Response containing audit trail data with pagination
/// </summary>
public sealed class AuditTrailResponse
{
    public List<AuditLogDto> AuditLogs { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => PageNumber < TotalPages;
    public bool HasPreviousPage => PageNumber > 1;
}

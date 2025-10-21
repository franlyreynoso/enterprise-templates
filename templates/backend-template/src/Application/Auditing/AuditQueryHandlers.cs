using MediatR;
using EnterpriseTemplate.Application.Abstractions;

namespace EnterpriseTemplate.Application.Auditing;

/// <summary>
/// Handler for getting audit trail for a specific entity
/// </summary>
public sealed class GetEntityAuditTrailHandler : IRequestHandler<GetEntityAuditTrail, AuditTrailResponse>
{
    private readonly IAuditRepository _auditRepository;

    public GetEntityAuditTrailHandler(IAuditRepository auditRepository)
    {
        _auditRepository = auditRepository;
    }

    public async Task<AuditTrailResponse> Handle(GetEntityAuditTrail request, CancellationToken cancellationToken)
    {
        var pageSize = Math.Min(request.PageSize, 100); // Max 100 items per page

        var (auditLogs, totalCount) = await _auditRepository.GetEntityAuditTrailAsync(
            request.EntityType,
            request.EntityId,
            pageSize,
            request.PageNumber,
            cancellationToken);

        return new AuditTrailResponse
        {
            AuditLogs = auditLogs.Select(AuditLogDto.FromEntity).ToList(),
            TotalCount = totalCount,
            PageSize = pageSize,
            PageNumber = request.PageNumber
        };
    }
}

/// <summary>
/// Handler for getting audit trail for a specific user
/// </summary>
public sealed class GetUserAuditTrailHandler : IRequestHandler<GetUserAuditTrail, AuditTrailResponse>
{
    private readonly IAuditRepository _auditRepository;

    public GetUserAuditTrailHandler(IAuditRepository auditRepository)
    {
        _auditRepository = auditRepository;
    }

    public async Task<AuditTrailResponse> Handle(GetUserAuditTrail request, CancellationToken cancellationToken)
    {
        var pageSize = Math.Min(request.PageSize, 100);

        var (auditLogs, totalCount) = await _auditRepository.GetUserAuditTrailAsync(
            request.UserId,
            pageSize,
            request.PageNumber,
            request.FromDate,
            request.ToDate,
            cancellationToken);

        return new AuditTrailResponse
        {
            AuditLogs = auditLogs.Select(AuditLogDto.FromEntity).ToList(),
            TotalCount = totalCount,
            PageSize = pageSize,
            PageNumber = request.PageNumber
        };
    }
}

/// <summary>
/// Handler for getting recent audit activities
/// </summary>
public sealed class GetRecentAuditActivitiesHandler : IRequestHandler<GetRecentAuditActivities, AuditTrailResponse>
{
    private readonly IAuditRepository _auditRepository;

    public GetRecentAuditActivitiesHandler(IAuditRepository auditRepository)
    {
        _auditRepository = auditRepository;
    }

    public async Task<AuditTrailResponse> Handle(GetRecentAuditActivities request, CancellationToken cancellationToken)
    {
        var pageSize = Math.Min(request.PageSize, 100);

        var (auditLogs, totalCount) = await _auditRepository.GetRecentAuditActivitiesAsync(
            pageSize,
            request.PageNumber,
            request.EntityType,
            request.Action,
            cancellationToken);

        return new AuditTrailResponse
        {
            AuditLogs = auditLogs.Select(AuditLogDto.FromEntity).ToList(),
            TotalCount = totalCount,
            PageSize = pageSize,
            PageNumber = request.PageNumber
        };
    }
}

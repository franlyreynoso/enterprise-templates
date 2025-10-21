using Microsoft.EntityFrameworkCore;
using EnterpriseTemplate.Application.Abstractions;
using EnterpriseTemplate.Domain.Entities;

namespace EnterpriseTemplate.Infrastructure.Persistence;

/// <summary>
/// Entity Framework implementation of audit repository
/// </summary>
public class EfAuditRepository : IAuditRepository
{
    private readonly AppDbContext _context;

    public EfAuditRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(List<AuditLog> AuditLogs, int TotalCount)> GetEntityAuditTrailAsync(
        string entityType,
        string entityId,
        int pageSize,
        int pageNumber,
        CancellationToken cancellationToken = default)
    {
        var skip = (pageNumber - 1) * pageSize;

        var query = _context.AuditLogs
            .Where(a => a.EntityType == entityType && a.EntityId == entityId)
            .OrderByDescending(a => a.Timestamp);

        var totalCount = await query.CountAsync(cancellationToken);
        var auditLogs = await query
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (auditLogs, totalCount);
    }

    public async Task<(List<AuditLog> AuditLogs, int TotalCount)> GetUserAuditTrailAsync(
        string userId,
        int pageSize,
        int pageNumber,
        DateTimeOffset? fromDate = null,
        DateTimeOffset? toDate = null,
        CancellationToken cancellationToken = default)
    {
        var skip = (pageNumber - 1) * pageSize;

        var query = _context.AuditLogs
            .Where(a => a.UserId == userId);

        // Apply date filters if provided
        if (fromDate.HasValue)
            query = query.Where(a => a.Timestamp >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(a => a.Timestamp <= toDate.Value);

        query = query.OrderByDescending(a => a.Timestamp);

        var totalCount = await query.CountAsync(cancellationToken);
        var auditLogs = await query
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (auditLogs, totalCount);
    }

    public async Task<(List<AuditLog> AuditLogs, int TotalCount)> GetRecentAuditActivitiesAsync(
        int pageSize,
        int pageNumber,
        string? entityType = null,
        string? action = null,
        CancellationToken cancellationToken = default)
    {
        var skip = (pageNumber - 1) * pageSize;

        var query = _context.AuditLogs.AsQueryable();

        // Apply filters if provided
        if (!string.IsNullOrEmpty(entityType))
            query = query.Where(a => a.EntityType == entityType);

        if (!string.IsNullOrEmpty(action))
            query = query.Where(a => a.Action == action);

        query = query.OrderByDescending(a => a.Timestamp);

        var totalCount = await query.CountAsync(cancellationToken);
        var auditLogs = await query
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (auditLogs, totalCount);
    }
}

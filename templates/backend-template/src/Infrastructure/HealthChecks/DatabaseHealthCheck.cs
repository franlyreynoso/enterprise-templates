using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using EnterpriseTemplate.Infrastructure.Persistence;

namespace EnterpriseTemplate.Infrastructure.HealthChecks;

/// <summary>
/// Health check for Entity Framework DbContext database connectivity
/// </summary>
public class DatabaseHealthCheck : IHealthCheck
{
    private readonly AppDbContext _context;

    public DatabaseHealthCheck(AppDbContext context)
    {
        _context = context;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Simple query to verify database connectivity
            await _context.Database.ExecuteSqlRawAsync("SELECT 1", cancellationToken);

            var connectionString = _context.Database.GetConnectionString();
            var dbName = ExtractDatabaseName(connectionString);
            
            return HealthCheckResult.Healthy($"Database '{dbName}' connection is healthy");
        }
        catch (Exception ex)
        {
            var connectionString = _context.Database.GetConnectionString();
            var dbName = ExtractDatabaseName(connectionString);
            var host = ExtractHost(connectionString);
            
            return HealthCheckResult.Unhealthy(
                $"Database connection failed. Unable to connect to database '{dbName}' on '{host}'. " +
                $"Please ensure PostgreSQL is running and accessible. Error: {ex.Message}", 
                ex);
        }
    }

    private static string ExtractDatabaseName(string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            return "unknown";
            
        var dbMatch = System.Text.RegularExpressions.Regex.Match(connectionString, @"Database=([^;]+)", 
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        return dbMatch.Success ? dbMatch.Groups[1].Value : "unknown";
    }

    private static string ExtractHost(string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            return "unknown";
            
        var hostMatch = System.Text.RegularExpressions.Regex.Match(connectionString, @"Host=([^;]+)", 
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        var portMatch = System.Text.RegularExpressions.Regex.Match(connectionString, @"Port=([^;]+)", 
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        
        var host = hostMatch.Success ? hostMatch.Groups[1].Value : "localhost";
        var port = portMatch.Success ? portMatch.Groups[1].Value : "5432";
        
        return $"{host}:{port}";
    }
}

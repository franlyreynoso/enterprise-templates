using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace EnterpriseTemplate.Infrastructure.HealthChecks;

/// <summary>
/// Health check for Redis cache connectivity
/// </summary>
public class RedisHealthCheck : IHealthCheck
{
    private readonly IConnectionMultiplexer? _redis;

    public RedisHealthCheck(IConnectionMultiplexer? redis = null)
    {
        _redis = redis;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (_redis == null)
            {
                return HealthCheckResult.Degraded("Redis is not configured");
            }

            if (!_redis.IsConnected)
            {
                return HealthCheckResult.Unhealthy("Redis is not connected");
            }

            // Test a simple ping operation
            var database = _redis.GetDatabase();
            var pingResult = await database.PingAsync();

            return HealthCheckResult.Healthy($"Redis is healthy (Ping: {pingResult.TotalMilliseconds:F2}ms)");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Redis health check failed", ex);
        }
    }
}

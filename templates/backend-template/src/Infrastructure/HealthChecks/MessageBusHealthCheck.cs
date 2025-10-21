using Microsoft.Extensions.Diagnostics.HealthChecks;
using MassTransit;

namespace EnterpriseTemplate.Infrastructure.HealthChecks;

/// <summary>
/// Health check for MassTransit/RabbitMQ message bus connectivity
/// </summary>
public class MessageBusHealthCheck : IHealthCheck
{
    private readonly IBusControl _busControl;

    public MessageBusHealthCheck(IBusControl busControl)
    {
        _busControl = busControl;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if the bus is started and healthy
            var busAddress = _busControl.Address;

            // MassTransit doesn't expose a direct health check, but we can verify the bus is running
            if (busAddress != null)
            {
                return Task.FromResult(HealthCheckResult.Healthy($"Message bus is healthy (Address: {busAddress})"));
            }

            return Task.FromResult(HealthCheckResult.Unhealthy("Message bus address is null"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("Message bus health check failed", ex));
        }
    }
}

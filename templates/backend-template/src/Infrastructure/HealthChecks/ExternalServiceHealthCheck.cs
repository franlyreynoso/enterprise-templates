using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace EnterpriseTemplate.Infrastructure.HealthChecks;

/// <summary>
/// Health check for external HTTP services
/// </summary>
public class ExternalServiceHealthCheck : IHealthCheck
{
    private readonly HttpClient _httpClient;
    private readonly string _serviceUrl;
    private readonly string _serviceName;

    public ExternalServiceHealthCheck(HttpClient httpClient, string serviceUrl, string serviceName)
    {
        _httpClient = httpClient;
        _serviceUrl = serviceUrl;
        _serviceName = serviceName;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await _httpClient.GetAsync(_serviceUrl, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return HealthCheckResult.Healthy($"{_serviceName} is healthy (Status: {response.StatusCode})");
            }

            return HealthCheckResult.Unhealthy($"{_serviceName} returned unhealthy status: {response.StatusCode}");
        }
        catch (TaskCanceledException)
        {
            return HealthCheckResult.Unhealthy($"{_serviceName} health check timed out");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"{_serviceName} health check failed", ex);
        }
    }
}

using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;

namespace EnterpriseTemplate.Infrastructure.Resilience;

/// <summary>
/// Service for managing resilience policies for external service calls
/// </summary>
public interface IResilienceService
{
    /// <summary>
    /// Execute HTTP call with comprehensive resilience patterns
    /// </summary>
    Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> operation, string operationKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Execute HTTP call with comprehensive resilience patterns (non-generic)
    /// </summary>
    Task ExecuteAsync(Func<CancellationToken, Task> operation, string operationKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a retry policy for external service calls
    /// </summary>
    IAsyncPolicy<HttpResponseMessage> GetHttpRetryPolicy();
}

public class ResilienceService : IResilienceService
{
    private readonly ILogger<ResilienceService> _logger;
    private readonly IAsyncPolicy _defaultPolicy;
    private readonly IAsyncPolicy<HttpResponseMessage> _httpRetryPolicy;

    public ResilienceService(ILogger<ResilienceService> logger)
    {
        _logger = logger;

        // Create a default retry policy
        _defaultPolicy = Policy
            .Handle<HttpRequestException>()
            .Or<TaskCanceledException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    _logger.LogWarning("Retry attempt {RetryCount} after {Delay}ms. Operation: {OperationKey}",
                        retryCount, timespan.TotalMilliseconds, context.OperationKey);
                });

        // Create HTTP retry policy
        _httpRetryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    _logger.LogWarning("HTTP retry attempt {RetryCount} after {Delay}ms. Operation: {OperationKey}",
                        retryCount, timespan.TotalMilliseconds, context.OperationKey);
                });
    }

    public async Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> operation, string operationKey, CancellationToken cancellationToken = default)
    {
        var context = new Context(operationKey);

        return await _defaultPolicy.ExecuteAsync(async () =>
        {
            _logger.LogDebug("Executing operation {OperationKey} with resilience policy", operationKey);
            return await operation(cancellationToken);
        });
    }

    public async Task ExecuteAsync(Func<CancellationToken, Task> operation, string operationKey, CancellationToken cancellationToken = default)
    {
        var context = new Context(operationKey);

        await _defaultPolicy.ExecuteAsync(async () =>
        {
            _logger.LogDebug("Executing operation {OperationKey} with resilience policy", operationKey);
            await operation(cancellationToken);
        });
    }

    public IAsyncPolicy<HttpResponseMessage> GetHttpRetryPolicy() => _httpRetryPolicy;
}

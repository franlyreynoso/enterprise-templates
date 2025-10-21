using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace EnterpriseTemplate.Infrastructure.Caching;

/// <summary>
/// Service for managing distributed caching with Redis
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Get cached item by key
    /// </summary>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Set cached item with expiration
    /// </summary>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Remove cached item by key
    /// </summary>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove multiple cached items by pattern (Redis-specific)
    /// </summary>
    Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get or set cached item using cache-aside pattern
    /// </summary>
    Task<T?> GetOrSetAsync<T>(string key, Func<CancellationToken, Task<T?>> getItem, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class;
}

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public RedisCacheService(IDistributedCache distributedCache, ILogger<RedisCacheService> logger)
    {
        _distributedCache = distributedCache;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var cachedValue = await _distributedCache.GetStringAsync(key, cancellationToken);

            if (string.IsNullOrEmpty(cachedValue))
            {
                _logger.LogDebug("Cache miss for key: {Key}", key);
                return null;
            }

            _logger.LogDebug("Cache hit for key: {Key}", key);
            return JsonSerializer.Deserialize<T>(cachedValue, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cached item for key: {Key}", key);
            return null; // Graceful degradation - don't fail if cache is unavailable
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);

            var options = new DistributedCacheEntryOptions();
            if (expiration.HasValue)
            {
                options.SetAbsoluteExpiration(expiration.Value);
            }
            else
            {
                // Default expiration of 1 hour if not specified
                options.SetAbsoluteExpiration(TimeSpan.FromHours(1));
            }

            await _distributedCache.SetStringAsync(key, serializedValue, options, cancellationToken);
            _logger.LogDebug("Cached item for key: {Key} with expiration: {Expiration}", key, expiration ?? TimeSpan.FromHours(1));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cached item for key: {Key}", key);
            // Don't throw - graceful degradation
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _distributedCache.RemoveAsync(key, cancellationToken);
            _logger.LogDebug("Removed cached item for key: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cached item for key: {Key}", key);
            // Don't throw - graceful degradation
        }
    }

    public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        try
        {
            // Note: This is a simplified implementation
            // In a real-world scenario, you'd use Redis-specific commands or patterns
            _logger.LogInformation("Pattern-based cache invalidation requested for pattern: {Pattern}", pattern);

            // For demonstration purposes, we'll log this but not implement full pattern matching
            // A full implementation would require direct Redis connection to use SCAN/DEL commands
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cached items by pattern: {Pattern}", pattern);
            // Don't throw - graceful degradation
        }
    }

    public async Task<T?> GetOrSetAsync<T>(string key, Func<CancellationToken, Task<T?>> getItem, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
    {
        // Try to get from cache first
        var cachedItem = await GetAsync<T>(key, cancellationToken);
        if (cachedItem != null)
        {
            return cachedItem;
        }

        // Cache miss - get from source
        _logger.LogDebug("Cache miss for key: {Key}, fetching from source", key);

        try
        {
            var item = await getItem(cancellationToken);

            if (item != null)
            {
                // Cache the result
                await SetAsync(key, item, expiration, cancellationToken);
            }

            return item;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching item for cache key: {Key}", key);
            throw; // Re-throw as this is a business logic failure, not a cache failure
        }
    }
}

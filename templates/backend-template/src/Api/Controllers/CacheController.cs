using EnterpriseTemplate.Infrastructure.Caching;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseTemplate.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CacheController : ControllerBase
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<CacheController> _logger;

    public CacheController(ICacheService cacheService, ILogger<CacheController> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }

    /// <summary>
    /// Test caching by storing and retrieving a sample item
    /// </summary>
    [HttpPost("test/{key}")]
    public async Task<IActionResult> SetTestItem(string key, [FromBody] TestCacheItem item)
    {
        await _cacheService.SetAsync(key, item, TimeSpan.FromMinutes(5));
        _logger.LogInformation("Cached item with key: {Key}", key);
        return Ok(new { Message = "Item cached successfully", Key = key });
    }

    /// <summary>
    /// Get cached item by key
    /// </summary>
    [HttpGet("test/{key}")]
    public async Task<IActionResult> GetTestItem(string key)
    {
        var item = await _cacheService.GetAsync<TestCacheItem>(key);

        if (item == null)
        {
            return NotFound(new { Message = "Item not found in cache", Key = key });
        }

        _logger.LogInformation("Retrieved cached item with key: {Key}", key);
        return Ok(item);
    }

    /// <summary>
    /// Demonstrate cache-aside pattern with GetOrSet
    /// </summary>
    [HttpGet("expensive-operation/{id}")]
    public async Task<IActionResult> GetExpensiveData(int id)
    {
        var cacheKey = CacheKeys.ExpensiveOperation(id);

        var result = await _cacheService.GetOrSetAsync(cacheKey, async (ct) =>
        {
            _logger.LogInformation("Cache miss - performing expensive operation for ID: {Id}", id);

            // Simulate expensive operation
            await Task.Delay(2000, ct);

            return new ExpensiveOperationResult
            (
                Id: id,
                Data: $"Expensive result for {id}",
                ProcessedAt: DateTime.UtcNow,
                ProcessingTimeMs: 2000
            );
        }, TimeSpan.FromMinutes(10));

        return Ok(result);
    }

    /// <summary>
    /// Remove cached item
    /// </summary>
    [HttpDelete("test/{key}")]
    public async Task<IActionResult> RemoveTestItem(string key)
    {
        await _cacheService.RemoveAsync(key);
        _logger.LogInformation("Removed cached item with key: {Key}", key);
        return Ok(new { Message = "Item removed from cache", Key = key });
    }

    /// <summary>
    /// Remove cached items by pattern
    /// </summary>
    [HttpDelete("pattern/{pattern}")]
    public async Task<IActionResult> RemoveByPattern(string pattern)
    {
        await _cacheService.RemoveByPatternAsync(pattern);
        _logger.LogInformation("Removed cached items matching pattern: {Pattern}", pattern);
        return Ok(new { Message = "Items removed from cache", Pattern = pattern });
    }
}

public record TestCacheItem(string Name, string Value, DateTime CreatedAt);

public record ExpensiveOperationResult(int Id, string Data, DateTime ProcessedAt, int ProcessingTimeMs);

public static class CacheKeys
{
    public static string ExpensiveOperation(int id) => $"expensive:operation:{id}";
    public static string UserProfile(string userId) => $"user:profile:{userId}";
    public static string ProductCatalog(int categoryId) => $"product:catalog:{categoryId}";
}

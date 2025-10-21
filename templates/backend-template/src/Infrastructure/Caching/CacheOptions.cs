namespace EnterpriseTemplate.Infrastructure.Caching;

/// <summary>
/// Configuration options for caching
/// </summary>
public class CacheOptions
{
    public const string SectionName = "Cache";

    /// <summary>
    /// Default cache expiration in minutes
    /// </summary>
    public int DefaultExpirationMinutes { get; set; } = 60;

    /// <summary>
    /// Cache expiration for Todo items in minutes
    /// </summary>
    public int TodoExpirationMinutes { get; set; } = 30;

    /// <summary>
    /// Cache expiration for Audit logs in minutes
    /// </summary>
    public int AuditExpirationMinutes { get; set; } = 120;

    /// <summary>
    /// Whether to enable caching
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Cache key prefix for the application
    /// </summary>
    public string KeyPrefix { get; set; } = "EnterpriseTemplate:";

    /// <summary>
    /// Redis connection string
    /// </summary>
    public string? RedisConnectionString { get; set; }
}

/// <summary>
/// Cache key builder for consistent key generation
/// </summary>
public static class CacheKeys
{
    private const string PREFIX = "EnterpriseTemplate:";

    public static string Todo(string todoId) => $"{PREFIX}todo:{todoId}";
    public static string TodoList(string userId) => $"{PREFIX}todos:user:{userId}";
    public static string AuditTrail(string entityType, string entityId) => $"{PREFIX}audit:{entityType}:{entityId}";
    public static string UserAuditTrail(string userId) => $"{PREFIX}audit:user:{userId}";
    public static string RecentAuditActivities() => $"{PREFIX}audit:recent";
}

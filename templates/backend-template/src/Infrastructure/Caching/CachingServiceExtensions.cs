using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace EnterpriseTemplate.Infrastructure.Caching;

/// <summary>
/// Extension methods for configuring caching services
/// </summary>
public static class CachingServiceExtensions
{
    /// <summary>
    /// Add distributed caching services with Redis
    /// </summary>
    public static IServiceCollection AddCachingServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure cache options
        services.Configure<CacheOptions>(configuration.GetSection(CacheOptions.SectionName));

        // Add Redis distributed cache
        var redisConnectionString = configuration.GetConnectionString("Redis")
            ?? configuration["Cache:RedisConnectionString"]
            ?? "localhost:6379";

        // Register Redis connection multiplexer for health checks and direct Redis access
        services.AddSingleton<IConnectionMultiplexer>(provider =>
        {
            try
            {
                return ConnectionMultiplexer.Connect(redisConnectionString);
            }
            catch
            {
                // Return null if Redis is not available - this will make health check report degraded
                return null!;
            }
        });

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
            options.InstanceName = "EnterpriseTemplate";
        });

        // Register cache service
        services.AddScoped<ICacheService, RedisCacheService>();

        return services;
    }
}

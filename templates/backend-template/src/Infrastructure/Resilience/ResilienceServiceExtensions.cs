using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EnterpriseTemplate.Infrastructure.Resilience;

/// <summary>
/// Extension methods for configuring resilience services
/// </summary>
public static class ResilienceServiceExtensions
{
    /// <summary>
    /// Add resilience services with Polly policies
    /// </summary>
    public static IServiceCollection AddResilienceServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure resilience options
        services.Configure<ResilienceOptions>(configuration.GetSection(ResilienceOptions.SectionName));

        // Register the resilience service
        services.AddScoped<IResilienceService, ResilienceService>();

        return services;
    }
}

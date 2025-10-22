using EnterpriseTemplate.Application.Abstractions;
using EnterpriseTemplate.Application.Todos;
using EnterpriseTemplate.Infrastructure.Auditing;
using EnterpriseTemplate.Infrastructure.Caching;
using EnterpriseTemplate.Infrastructure.HealthChecks;
using EnterpriseTemplate.Infrastructure.Persistence;
using EnterpriseTemplate.Infrastructure.Resilience;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace EnterpriseTemplate.Infrastructure;

public static class InfrastructureRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection s, IConfiguration c)
    {
        // Audit infrastructure (register first so it can be used in DbContext)
        s.AddScoped<AuditInterceptor>();
        s.AddSingleton(TimeProvider.System);

        // Db provider + connection string
        var cs = c.GetConnectionString("Default") ?? c["ConnectionStrings:Default"];
        var provider = (c["Db:Provider"] ?? "Postgres").ToLowerInvariant();

        s.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            if (provider == "sqlserver")
                options.UseSqlServer(cs);
            else
                options.UseNpgsql(cs);

            // Add the audit interceptor
            options.AddInterceptors(serviceProvider.GetRequiredService<AuditInterceptor>());
        });

        // Repository implementations
        s.AddScoped<ITodoStore, EfTodoStore>();
        s.AddScoped<IAuditRepository, EfAuditRepository>();

        // Health checks
        s.AddHealthCheckServices(c);

        // Resilience services (Circuit Breaker, Retry, Timeout)
        s.AddResilienceServices(c);

        // Distributed caching with Redis
        s.AddCachingServices(c);

        // ... MassTransit, Redis, etc ...
        return s;
    }

    /// <summary>
    /// Add comprehensive health checks for infrastructure
    /// </summary>
    private static IServiceCollection AddHealthCheckServices(this IServiceCollection services, IConfiguration configuration)
    {
        var healthChecks = services.AddHealthChecks();

        // Database health check
        healthChecks.AddCheck<DatabaseHealthCheck>("database", tags: new[] { "ready", "database" });

        // Message bus health check (if configured)
        var busProvider = configuration["Bus:Provider"];
        if (!string.IsNullOrEmpty(busProvider) && busProvider.Equals("RabbitMQ", StringComparison.OrdinalIgnoreCase))
        {
            healthChecks.AddCheck<MessageBusHealthCheck>("messagebus", tags: new[] { "ready", "messaging" });
        }

        // Redis health check (if configured)
        healthChecks.AddCheck<RedisHealthCheck>("redis", tags: new[] { "ready", "cache" });

        // External service health check example - using a factory
        healthChecks.AddCheck("external-httpbin", () =>
        {
            using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
            var healthCheck = new ExternalServiceHealthCheck(httpClient, "https://httpbin.org/status/200", "HTTPBin Service");
            return healthCheck.CheckHealthAsync(new HealthCheckContext()).Result;
        }, tags: new[] { "external" });

        return services;
    }
}

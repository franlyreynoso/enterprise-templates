using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Enterprise.Ui.Observability;

public static class OtelExtensions
{
    public static IServiceCollection AddEnterpriseOtel(this IServiceCollection services, IConfiguration cfg, string serviceName)
    {
        var resource = ResourceBuilder.CreateDefault().AddService(serviceName);

        services.AddOpenTelemetry()
            .ConfigureResource(b => b.AddService(serviceName))
            .WithTracing(b => b
                .SetResourceBuilder(resource)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation())
            .WithMetrics(b => b
                .SetResourceBuilder(resource)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation());


        // TODO: Re-enable OTLP exporters once the extension methods are available
        // .AddOtlpExporter(o => o.Endpoint = new Uri(cfg["Otel:OtlpEndpoint"] ?? "http://localhost:4317"))

        return services;
    }
}

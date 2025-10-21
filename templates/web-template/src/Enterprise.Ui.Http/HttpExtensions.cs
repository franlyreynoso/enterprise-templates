using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System.Diagnostics;
using System.Net;

namespace Enterprise.Ui.Http;

public sealed class CorrelationHandler : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage req, CancellationToken ct)
    {
        req.Headers.TryAddWithoutValidation("x-correlation-id", Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString());
        return base.SendAsync(req, ct);
    }
}

public static class HttpExtensions
{
    public static IServiceCollection AddEnterpriseHttpClient(
        this IServiceCollection services, IConfiguration cfg, string name = "Api")
    {
        var baseUrl = cfg["Api:BaseUrl"] ?? "https://localhost:8080";

        services.AddTransient<CorrelationHandler>();

        var retry = HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(new[] { TimeSpan.FromMilliseconds(250), TimeSpan.FromMilliseconds(500), TimeSpan.FromSeconds(1) });

        var timeout = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(30));
        var circuit = HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(handledEventsAllowedBeforeBreaking: 5, durationOfBreak: TimeSpan.FromSeconds(15));

        services.AddHttpClient(name, c => c.BaseAddress = new Uri(baseUrl))
            .AddHttpMessageHandler<CorrelationHandler>()
            .AddPolicyHandler(retry)
            .AddPolicyHandler(timeout)
            .AddPolicyHandler(circuit);

        return services;
    }
}

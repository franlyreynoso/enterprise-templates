using EnterpriseTemplate.Application.Messaging;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EnterpriseTemplate.Infrastructure;

public static class BusRegistration
{
    public static IServiceCollection AddBus(this IServiceCollection services, IConfiguration config)
    {
        var provider = (config["Bus:Provider"] ?? "None").ToLowerInvariant();

        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();
            // Consumers can be added here, e.g. x.AddConsumer<SampleMessageConsumer>();

            x.AddConsumer<PingConsumer>();

            if (provider == "rabbitmq")
            {
                x.UsingRabbitMq((ctx, cfg) =>
                {
                    var host = config["RabbitMQ:Host"] ?? "rabbitmq";
                    var port = ushort.TryParse(config["RabbitMQ:Port"], out var p) ? p : (ushort)6672;
                    var user = config["RabbitMQ:User"] ?? "app";
                    var pass = config["RabbitMQ:Pass"] ?? "app";

                    cfg.Host(host, port, "/", h =>
                    {
                        h.Username(user);
                        h.Password(pass);
                    });

                    cfg.ConfigureEndpoints(ctx);
                });
            }
            else if (provider == "azureservicebus")
            {
                x.UsingAzureServiceBus((ctx, cfg) =>
                {
                    var conn = config["ServiceBus:Connection"];
                    if (string.IsNullOrWhiteSpace(conn))
                        throw new InvalidOperationException("ServiceBus:Connection is not configured");
                    cfg.Host(conn);
                    cfg.ConfigureEndpoints(ctx);
                });
            }
        });

        // Wait for the bus to actually connect instead of failing fast
        services.AddOptions<MassTransitHostOptions>().Configure(o =>
        {
            o.WaitUntilStarted = true;
            o.StartTimeout = TimeSpan.FromSeconds(45);
            o.StopTimeout = TimeSpan.FromSeconds(30);
        });

        return services;
    }
}

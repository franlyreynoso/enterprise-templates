using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EnterpriseTemplate.Infrastructure.Abstractions;

namespace EnterpriseTemplate.Infrastructure.AzurePack;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAzurePack(this IServiceCollection s, IConfiguration c)
        => AddAzurePack(s, c, false);

    public static IServiceCollection AddAzurePack(IServiceCollection s, IConfiguration c, bool _=false)
    {
        // First add core infrastructure (database, etc.)
        EnterpriseTemplate.Infrastructure.OnPremPack.ServiceCollectionExtensions.AddInfrastructure(s, c);
        
        // Register Azure implementations here (Key Vault, Service Bus, Blob, Redis, etc.)
        s.AddSingleton<ISecrets, NoopSecrets>();
        s.AddSingleton<IBlobStorage, NoopBlobStorage>();
        s.AddSingleton<IEventBus, NoopEventBus>();
        EnterpriseTemplate.Infrastructure.BusRegistration.AddBus(s, c);
        return s;
    }
}

file sealed class NoopSecrets : ISecrets { public string? Get(string key) => null; }
file sealed class NoopBlobStorage : IBlobStorage { public Task UploadAsync(string path, Stream content, CancellationToken ct=default) => Task.CompletedTask; }
file sealed class NoopEventBus : IEventBus { public Task PublishAsync<T>(T message, CancellationToken ct=default) => Task.CompletedTask; }

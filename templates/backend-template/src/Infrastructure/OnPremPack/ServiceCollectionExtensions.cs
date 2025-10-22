using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EnterpriseTemplate.Infrastructure.Abstractions;

namespace EnterpriseTemplate.Infrastructure.OnPremPack;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOnPremPack(this IServiceCollection s, IConfiguration c)
        => AddOnPremPack(s, c, false);

    public static IServiceCollection AddOnPremPack(IServiceCollection s, IConfiguration c, bool _ = false)
    {
        // First add core infrastructure (database, etc.)
        InfrastructureRegistration.AddInfrastructure(s, c);

        // Register on-prem implementations here (file secrets, RabbitMQ, filesystem, etc.)
        s.AddSingleton<ISecrets, FileSecrets>();
        s.AddSingleton<IBlobStorage, FileSystemBlobStorage>();
        s.AddSingleton<IEventBus, NoopEventBus>();
        EnterpriseTemplate.Infrastructure.BusRegistration.AddBus(s, c);
        return s;
    }
}

file sealed class FileSecrets : ISecrets
{
    public string? Get(string key) => Environment.GetEnvironmentVariable(key);
}

file sealed class FileSystemBlobStorage : IBlobStorage
{
    public Task UploadAsync(string path, Stream content, CancellationToken ct = default)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        using var fs = File.Create(path);
        content.CopyTo(fs);
        return Task.CompletedTask;
    }
}

file sealed class NoopEventBus : IEventBus { public Task PublishAsync<T>(T message, CancellationToken ct = default) => Task.CompletedTask; }

namespace EnterpriseTemplate.Infrastructure.Abstractions;
public interface ISecrets { string? Get(string key); }
public interface IBlobStorage { Task UploadAsync(string path, Stream content, CancellationToken ct=default); }
public interface IEventBus { Task PublishAsync<T>(T message, CancellationToken ct=default); }

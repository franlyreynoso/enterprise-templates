using MassTransit;
using Microsoft.Extensions.Logging;

namespace EnterpriseTemplate.Application.Messaging;

public sealed class PingConsumer : IConsumer<PingMessage>
{
    private readonly ILogger<PingConsumer> _logger;

    public PingConsumer(ILogger<PingConsumer> logger) => _logger = logger;

    public Task Consume(ConsumeContext<PingMessage> ctx)
    {
        _logger.LogInformation("✅ Received Ping: {Text} @ {At}", ctx.Message.Text, ctx.Message.Timestamp);
        return Task.CompletedTask;
    }
}

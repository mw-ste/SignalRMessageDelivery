using MassTransit;

namespace Backend.Messaging;

public record MassTransitMessageEvent(string Message);

public class MassTransitMessageEventConsumer : IConsumer<MassTransitMessageEvent>
{
    private readonly ILogger<MassTransitMessageEventConsumer> _logger;

    public MassTransitMessageEventConsumer(ILogger<MassTransitMessageEventConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<MassTransitMessageEvent> context)
    {
        _logger.LogInformation($"MassTransit message: {context.Message.Message}");
        return Task.CompletedTask;
    }
}
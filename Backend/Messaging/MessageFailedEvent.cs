using MediatR;
using Shared;

namespace Backend.Messaging;

public record MessageFailedEvent(MessageContext MessageContext) : INotification;

public class MessageFailedEventHandler : INotificationHandler<MessageFailedEvent>
{
    private readonly ILogger<MessageFailedEventHandler> _logger;

    public MessageFailedEventHandler(ILogger<MessageFailedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(MessageFailedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogWarning($"Failed to get answer for message {notification.MessageContext}");
        return Task.CompletedTask;
    }
}
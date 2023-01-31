using MediatR;
using Shared;

namespace Backend.Messaging;

public record MessageFailedEvent(MessageContext MessageContext) : INotification;

public class MessageFailedEventHandler : INotificationHandler<MessageFailedEvent>
{

    public MessageFailedEventHandler()
    {
    }

    public Task Handle(MessageFailedEvent notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
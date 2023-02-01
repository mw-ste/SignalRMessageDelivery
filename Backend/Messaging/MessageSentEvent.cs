using Backend.Aggregates;
using Backend.Database;
using MediatR;
using Shared;

namespace Backend.Messaging;

public record MessageSentEvent(MessageContext MessageContext, string Message) : INotification;

public class MessageSentEventHandler : INotificationHandler<MessageSentEvent>
{
    private readonly IPendingMessageRepository _messageRepository;

    public MessageSentEventHandler(
        IPendingMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task Handle(MessageSentEvent notification, CancellationToken cancellationToken)
    {
        var message = new PendingMessage(
            notification.MessageContext,
            notification.Message);

        message.Send();

        await _messageRepository.Save(message);
    }
}
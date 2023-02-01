using Backend.Aggregates;
using Backend.Database;
using MediatR;
using MessageContext = Shared.MessageContext;

namespace Backend.Messaging;

public record MessageCreatedEvent(string Sender, string Client, string Message, string MessageId) : INotification;

public class MessageCreatedEventHandler : INotificationHandler<MessageCreatedEvent>
{
    private readonly IPendingMessageRepository _messageRepository;

    public MessageCreatedEventHandler(
        IPendingMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task Handle(MessageCreatedEvent notification, CancellationToken cancellationToken)
    {
        var message = new PendingMessage(
            notification.Message,
            new MessageContext(notification.Sender, notification.Client, notification.MessageId));

        message.Send();

        await _messageRepository.Save(message);
    }
}
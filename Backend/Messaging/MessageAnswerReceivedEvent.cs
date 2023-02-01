using Backend.Database;
using MediatR;

namespace Backend.Messaging;

public record MessageAnswerReceivedEvent(string Sender, string Client, string Message, string MessageId) : INotification;

public class MessageAnswerReceivedEventHandler : INotificationHandler<MessageAnswerReceivedEvent>
{
    private readonly ISenderRepository _senderRepository;

    public MessageAnswerReceivedEventHandler(ISenderRepository senderRepository)
    {
        _senderRepository = senderRepository;
    }

    public async Task Handle(MessageAnswerReceivedEvent notification, CancellationToken cancellationToken)
    {
        var sender = await _senderRepository.Find(notification.Sender);
        sender.ReceiveAnswer(notification.Message, notification.Client, notification.MessageId);
        await _senderRepository.Save(sender);
    }
}
using MediatR;

namespace Backend.Messaging;

public record MessageAnswerReceivedEvent(string Sender, string Client, string Message, string MessageId) : INotification;

public class MessageAnswerReceivedEventHandler : INotificationHandler<MessageAnswerReceivedEvent>
{
    private readonly ISenderDatabase _senderDatabase;
    private readonly IMediator _mediator;

    public MessageAnswerReceivedEventHandler(ISenderDatabase senderDatabase, IMediator mediator)
    {
        _senderDatabase = senderDatabase;
        _mediator = mediator;
    }

    public async Task Handle(MessageAnswerReceivedEvent notification, CancellationToken cancellationToken)
    {
        var sender = await _senderDatabase.Find(notification.Sender);
        sender.ReceiveAnswer(notification.Message, notification.Client, notification.MessageId);
        await _senderDatabase.Save(sender, _mediator);
    }
}
using Backend.Database;
using MediatR;
using Shared;

namespace Backend.Messaging;

public record AnswerReceivedEvent(MessageContext MessageContext, string Answer) : INotification;

public class AnswerReceivedEventHandler : INotificationHandler<AnswerReceivedEvent>
{
    private readonly ISenderRepository _senderRepository;

    public AnswerReceivedEventHandler(ISenderRepository senderRepository)
    {
        _senderRepository = senderRepository;
    }

    public async Task Handle(AnswerReceivedEvent notification, CancellationToken cancellationToken)
    {
        var sender = await _senderRepository.Find(notification.MessageContext.Receiver);
        sender.ReceiveAnswer(notification.MessageContext, notification.Answer);
        await _senderRepository.Save(sender);
    }
}

public class DeletePendingMessageHandler : INotificationHandler<AnswerReceivedEvent>
{
    private readonly IPendingMessageRepository _pendingMessageRepository;

    public DeletePendingMessageHandler(IPendingMessageRepository pendingMessageRepository)
    {
        _pendingMessageRepository = pendingMessageRepository;
    }

    public async Task Handle(AnswerReceivedEvent notification, CancellationToken cancellationToken) => 
        await _pendingMessageRepository.Delete(notification.MessageContext.MessageId);
}
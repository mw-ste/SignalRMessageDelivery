using Backend.Database;
using MediatR;
using Shared;

namespace Backend.Messaging;

public record AnswerReceivedFromSignalREvent(SignalRMessageContext MessageContext, string Answer) : INotification;

public class AnswerReceivedFromSignalREventHandler : INotificationHandler<AnswerReceivedFromSignalREvent>
{
    private readonly IPendingMessageRepository _pendingMessageRepository;

    public AnswerReceivedFromSignalREventHandler(IPendingMessageRepository pendingMessageRepository)
    {
        _pendingMessageRepository = pendingMessageRepository;
    }

    public async Task Handle(AnswerReceivedFromSignalREvent notification, CancellationToken cancellationToken)
    {
        var pendingMessage = await _pendingMessageRepository.TryFind(notification.MessageContext.MessageContext.MessageId);

        if (pendingMessage == null)
        {
            return;
        }
        
        pendingMessage.HandleAnswer(notification.MessageContext, notification.Answer);
        await _pendingMessageRepository.Save(pendingMessage);
    }
}
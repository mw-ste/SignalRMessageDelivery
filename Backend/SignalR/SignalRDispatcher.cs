using Backend.Messaging;
using MediatR;
using Shared;

namespace Backend.SignalR;

public interface ISignalRDispatcher
{
    Task PublishClientMessageAnswer(
        MessageContext messageContext,
        string messageAnswer);
}

public class SignalRDispatcher : ISignalRDispatcher
{
    private readonly IPendingMessageDatabase _pendingMessageDatabase;
    private readonly IMediator _mediator;

    public SignalRDispatcher(
        IPendingMessageDatabase pendingMessageDatabase,
        IMediator mediator)
    {
        _pendingMessageDatabase = pendingMessageDatabase;
        _mediator = mediator;
    }

    public async Task PublishClientMessageAnswer(MessageContext messageContext, string messageAnswer)
    {
        var pendingMessage = await _pendingMessageDatabase.TryFind(messageContext.MessageId);
        if (pendingMessage == null)
        {
            return;
        }

        await _pendingMessageDatabase.Delete(messageContext.MessageId);

        var @event = new MessageAnswerReceivedEvent(messageContext.Receiver, messageContext.Sender, messageAnswer, messageContext.MessageId);
        await _mediator.Publish(@event);
    }
}
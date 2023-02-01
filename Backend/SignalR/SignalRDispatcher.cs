using Backend.Database;
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
    private readonly IPendingMessageRepository _pendingMessageRepository;
    private readonly IMediator _mediator;

    public SignalRDispatcher(
        IPendingMessageRepository pendingMessageRepository,
        IMediator mediator)
    {
        _pendingMessageRepository = pendingMessageRepository;
        _mediator = mediator;
    }

    public async Task PublishClientMessageAnswer(MessageContext messageContext, string messageAnswer)
    {
        var pendingMessage = await _pendingMessageRepository.TryFind(messageContext.MessageId);
        if (pendingMessage == null)
        {
            return;
        }

        await _pendingMessageRepository.Delete(messageContext.MessageId);

        var @event = new MessageAnswerReceivedEvent(messageContext.Receiver, messageContext.Sender, messageAnswer, messageContext.MessageId);
        await _mediator.Publish(@event);
    }
}
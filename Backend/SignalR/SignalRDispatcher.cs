using Backend.Database;
using Backend.Messaging;
using MediatR;
using Shared;

namespace Backend.SignalR;

public interface ISignalRDispatcher
{
    Task PublishClientMessageAnswer(
        SignalRMessageContext messageContext,
        string answer);
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

    public async Task PublishClientMessageAnswer(SignalRMessageContext signalRMessageContext, string answer)
    {
        var messageContext = signalRMessageContext.MessageContext;
        var pendingMessage = await _pendingMessageRepository.TryFind(messageContext.MessageId);
        if (pendingMessage == null)
        {
            return;
        }

        pendingMessage.HandleAnswer(signalRMessageContext, answer);
        await _pendingMessageRepository.Save(pendingMessage);
    }
}
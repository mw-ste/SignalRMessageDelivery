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
    private readonly IMediator _mediator;

    public SignalRDispatcher(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task PublishClientMessageAnswer(MessageContext messageContext, string messageAnswer)
    {
        var @event = new MessageAnswerReceivedEvent(messageContext.Receiver, messageContext.Sender, messageAnswer, messageContext.MessageId);
        return _mediator.Publish(@event);
    }
}

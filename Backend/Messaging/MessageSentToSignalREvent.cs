using Backend.MassTransit;
using Backend.SignalR;
using MassTransit;
using MediatR;
using Shared;

namespace Backend.Messaging;

public record MessageSentToSignalREvent(SignalRMessageContext MessageContext, string Message) : INotification;

public class MessageSentToSignalREventHandler : INotificationHandler<MessageSentToSignalREvent>
{
    private readonly ISignalRGateway _signalRGateway;
    private readonly IMessageScheduler _messageScheduler;

    public MessageSentToSignalREventHandler(
        ISignalRGateway signalRGateway,
        IMessageScheduler messageScheduler)
    {
        _signalRGateway = signalRGateway;
        _messageScheduler = messageScheduler;
    }

    public async Task Handle(MessageSentToSignalREvent notification, CancellationToken cancellationToken)
    {
        await _signalRGateway.SendMessageToClient(notification.MessageContext, notification.Message);

        await _messageScheduler.SchedulePublish(
            DateTime.UtcNow + TimeSpan.FromSeconds(5),
            new CheckPendingMessageState(notification.MessageContext.MessageContext.MessageId),
            cancellationToken);
    }
}
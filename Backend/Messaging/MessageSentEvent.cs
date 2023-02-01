using Backend.MassTransit;
using Backend.SignalR;
using MassTransit;
using MediatR;
using MessageContext = Shared.MessageContext;

namespace Backend.Messaging;

public record MessageSentEvent(MessageContext MessageContext, string Message) : INotification;

public class MessageSentEventHandler : INotificationHandler<MessageSentEvent>
{
    private readonly ISignalRGateway _signalRGateway;
    private readonly IMessageScheduler _messageScheduler;

    public MessageSentEventHandler(
        ISignalRGateway signalRGateway,
        IMessageScheduler messageScheduler)
    {
        _signalRGateway = signalRGateway;
        _messageScheduler = messageScheduler;
    }

    public async Task Handle(MessageSentEvent notification, CancellationToken cancellationToken)
    {
        await _signalRGateway.SendMessageToClient(notification.MessageContext, notification.Message);

        await _messageScheduler.SchedulePublish(
            DateTime.UtcNow + TimeSpan.FromSeconds(5),
            new CheckPendingMessageState(notification.MessageContext.MessageId),
            cancellationToken);
    }
}
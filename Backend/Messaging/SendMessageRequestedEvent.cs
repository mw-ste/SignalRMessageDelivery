using Backend.SignalR;
using MediatR;
using Shared;

namespace Backend.Messaging;

public record SendMessageRequestedEvent(string Sender, string Client, string Message, string MessageId) : INotification;

public class SendMessageRequestedEventHandler : INotificationHandler<SendMessageRequestedEvent>
{
    private readonly ISignalRGateway _signalRGateway;

    public SendMessageRequestedEventHandler(
        ISignalRGateway signalRGateway)
    {
        _signalRGateway = signalRGateway;
    }

    public async Task Handle(SendMessageRequestedEvent notification, CancellationToken cancellationToken)
    {
        await _signalRGateway.SendMessageToClient(
            new MessageContext(
                notification.Sender,
                notification.Client,
                notification.MessageId),
            notification.Message);
    }
}
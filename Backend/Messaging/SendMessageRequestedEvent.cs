using Backend.SignalR;
using MediatR;
using Shared;

namespace Backend.Messaging;

public record SendMessageRequestedEvent(string Sender, string Client, string Message, string MessageId) : INotification;

public class SendMessageRequestedEventHandler : INotificationHandler<SendMessageRequestedEvent>
{
    private readonly ISignalRGateway _signalRGateway;
    private readonly ILogger<SendMessageRequestedEventHandler> _logger;

    public SendMessageRequestedEventHandler(
        ISignalRGateway signalRGateway,
        ILogger<SendMessageRequestedEventHandler> logger)
    {
        _signalRGateway = signalRGateway;
        _logger = logger;
    }

    public Task Handle(SendMessageRequestedEvent notification, CancellationToken cancellationToken)
    {
        //_logger.LogInformation("SendMessageRequestedEvent " +
                               //$"Sender {notification.Sender} " +
                               //$"Client {notification.Client} " +
                               //$"Message {notification.Message} " +
                               //$"MessageId {notification.MessageId}");

        _signalRGateway.SendMessageToClient(
            new MessageContext(
                notification.Sender, 
                notification.Client, 
                notification.MessageId), 
            notification.Message);

        return Task.CompletedTask;
    }
}
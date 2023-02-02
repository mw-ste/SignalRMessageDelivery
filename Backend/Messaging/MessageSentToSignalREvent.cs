using Backend.Database;
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
    private readonly ISignalRConnectionRepository _connectionRepository;
    private readonly ILogger<MessageSentToSignalREventHandler> _logger;

    public MessageSentToSignalREventHandler(
        ISignalRGateway signalRGateway,
        IMessageScheduler messageScheduler,
        ISignalRConnectionRepository connectionRepository,
        ILogger<MessageSentToSignalREventHandler> logger)
    {
        _signalRGateway = signalRGateway;
        _messageScheduler = messageScheduler;
        _connectionRepository = connectionRepository;
        _logger = logger;
    }

    public async Task Handle(MessageSentToSignalREvent notification, CancellationToken cancellationToken)
    {
        var receiver = notification.MessageContext.MessageContext.Receiver;
        var connection = await _connectionRepository.TryFind(receiver);

        if (connection == null)
        {
            _logger.LogWarning($"No connection found for client {receiver}");
            throw new FailedToSendSignalRMessageException();
        }

        try
        {
            var result = await _signalRGateway.SendMessageToClientConnection(
                notification.MessageContext,
                notification.Message,
                connection.ConnectionId);

            if (result == false)
            {
                _logger.LogWarning("Message response doesn't indicate success!");
                throw new FailedToSendSignalRMessageException();
            }
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception.Message);
            throw new FailedToSendSignalRMessageException();
        }

        //await _signalRGateway.SendMessageToClient(notification.MessageContext, notification.Message);

        //await _messageScheduler.SchedulePublish(
        //    DateTime.UtcNow + TimeSpan.FromSeconds(5),
        //    new CheckPendingMessageState(notification.MessageContext.MessageContext.MessageId),
        //    cancellationToken);
    }
}
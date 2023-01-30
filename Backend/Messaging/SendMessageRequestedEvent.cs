using MediatR;

namespace Backend.Messaging;

public record SendMessageRequestedEvent(string Sender, string Client, string Message) : INotification;

public class SendMessageRequestedEventHandler : INotificationHandler<SendMessageRequestedEvent>
{
    private readonly ILogger<SendMessageRequestedEventHandler> _logger;

    public SendMessageRequestedEventHandler(ILogger<SendMessageRequestedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(SendMessageRequestedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("SendMessageRequestedEvent " +
                               $"Sender {notification.Sender} " +
                               $"Client {notification.Client} " +
                               $"Message {notification.Message}");

        return Task.CompletedTask;
    }
}
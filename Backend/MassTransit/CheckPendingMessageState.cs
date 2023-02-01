using Backend.Database;
using MassTransit;

namespace Backend.MassTransit;

public record CheckPendingMessageState(string MessageId);

public class CheckPendingMessageStateConsumer : IConsumer<CheckPendingMessageState>
{
    private readonly IPendingMessageDatabase _messageDatabase;
    private readonly ILogger<CheckPendingMessageStateConsumer> _logger;

    public CheckPendingMessageStateConsumer(
        IPendingMessageDatabase messageDatabase,
        ILogger<CheckPendingMessageStateConsumer> logger)
    {
        _messageDatabase = messageDatabase;
        _logger = logger;
    }

    private void LogWithTimestamp(string text, LogLevel logLevel) => 
        _logger.Log(logLevel, $"[{DateTime.UtcNow}] {text}");

    public async Task Consume(ConsumeContext<CheckPendingMessageState> context)
    {
        LogWithTimestamp($"Retrying message {context.Message.MessageId}", LogLevel.Information);
        var messageId = context.Message.MessageId;
        var message = await _messageDatabase.TryFind(messageId);
        if (message == null)
        {
            LogWithTimestamp($"Message {context.Message.MessageId} already finished", LogLevel.Information);
            return;
        }

        message.Send();

        if (message.Retryable)
        {
            LogWithTimestamp($"Message {context.Message.MessageId} is being retried the last time now", LogLevel.Warning);
            return;
        }

        await _messageDatabase.Save(message);
    }
}
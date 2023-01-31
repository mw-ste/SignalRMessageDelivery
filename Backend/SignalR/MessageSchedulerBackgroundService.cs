using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Backend.SignalR;

public class MessageSchedulerBackgroundService : BackgroundService
{
    private readonly IPendingMessageDatabase _pendingMessageDatabase;
    private readonly IHubContext<SignalRHub> _hubContext;
    private readonly IMediator _mediator;
    private readonly ILogger<MessageSchedulerBackgroundService> _logger;
    private readonly ILogger<PendingMessage> _messageLogger;

    private readonly List<PendingMessage> _deadMessages = new List<PendingMessage>();
    public IEnumerable<PendingMessage> DeadMessages => _deadMessages;

    public MessageSchedulerBackgroundService(
        IPendingMessageDatabase pendingMessageDatabase,
        IHubContext<SignalRHub> hubContext,
        IMediator mediator,
        ILogger<MessageSchedulerBackgroundService> logger,
        ILogger<PendingMessage> messageLogger)
    {
        _pendingMessageDatabase = pendingMessageDatabase;
        _hubContext = hubContext;
        _mediator = mediator;
        _logger = logger;
        _messageLogger = messageLogger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var pendingMessages = await _pendingMessageDatabase.List();
            foreach (var pendingMessage in pendingMessages)
            {
                if (DateTime.Now.ToUniversalTime() < pendingMessage.SendTimestamp + pendingMessage.RetryDelay)
                {
                    continue;
                }

                if (await pendingMessage.Retry(_hubContext, _mediator, _messageLogger))
                {
                    _logger.LogWarning($"Retrying message: {pendingMessage.MessageContext}");
                    await _pendingMessageDatabase.Save(pendingMessage, _mediator);
                    continue;
                }

                _logger.LogWarning($"Failed to get answer for message: {pendingMessage.MessageContext}");
                _deadMessages.Add(pendingMessage);
                await _pendingMessageDatabase.Delete(pendingMessage.Id);
            }

            await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
        }
    }

    public async Task ReviveDeadMessages()
    {
        async Task ReviveMessage(PendingMessage pendingMessage)
        {
            _deadMessages.Remove(pendingMessage);
            pendingMessage.TimeToLive = 3;
            await pendingMessage.Retry(_hubContext, _mediator, _messageLogger);
            await _pendingMessageDatabase.Save(pendingMessage, _mediator);
        }

        var x = _deadMessages
            .ToList()
            .Select(ReviveMessage)
            .ToArray();

        await Task.WhenAll(x);
    }
}
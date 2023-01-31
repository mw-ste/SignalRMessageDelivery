using Backend.Messaging;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Shared;

namespace Backend.SignalR;

public interface ISignalRGateway
{
    Task SendMessageToClient(MessageContext messageContext, string message);
}

public class SignalRGateway : ISignalRGateway
{
    private readonly IHubContext<SignalRHub> _hubContext;
    private readonly IHubContext<SignalRHub, ISignalRClient> _typedHubContext;
    private readonly IPendingMessageDatabase _pendingMessageDatabase;
    private readonly IMediator _mediator;
    private readonly ILogger<PendingMessage> _logger;

    public SignalRGateway(
        IHubContext<SignalRHub> hubContext,
        IHubContext<SignalRHub, ISignalRClient> typedHubContext,
        IPendingMessageDatabase pendingMessageDatabase,
        IMediator mediator,
        ILogger<PendingMessage> logger)
    {
        _hubContext = hubContext;
        _typedHubContext = typedHubContext;
        _pendingMessageDatabase = pendingMessageDatabase;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task SendMessageToClient(MessageContext messageContext, string message)
    {
        var pendingMessage = new PendingMessage(
            "ReceiveClientMessage",
            messageContext,
            message);

        await pendingMessage.Send(_hubContext, _logger);
        await _pendingMessageDatabase.Save(pendingMessage, _mediator);
    }
}

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
    private readonly IPendingMessageDatabase _pendingMessageDatabase;
    private readonly IMediator _mediator;

    public SignalRGateway(
        IHubContext<SignalRHub> hubContext,
        IPendingMessageDatabase pendingMessageDatabase,
        IMediator mediator)
    {
        _hubContext = hubContext;
        _pendingMessageDatabase = pendingMessageDatabase;
        _mediator = mediator;
    }

    public async Task SendMessageToClient(MessageContext messageContext, string message)
    {
        var pendingMessage = new PendingMessage(
            nameof(ISignalRClient.ReceiveClientMessage),
            messageContext,
            message);

        await pendingMessage.Send(_hubContext);
        await _pendingMessageDatabase.Save(pendingMessage, _mediator);
    }
}

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

    public SignalRGateway(IHubContext<SignalRHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task SendMessageToClient(MessageContext messageContext, string message)
    {
        return _hubContext
            .Clients
            .Groups(messageContext.Receiver)
            .SendAsync(
                "ReceiveClientMessage",
                messageContext,
                message);
    }
}

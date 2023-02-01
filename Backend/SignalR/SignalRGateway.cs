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

    public async Task SendMessageToClient(MessageContext messageContext, string message)
    {
        await _hubContext
            .Clients
            .Group(messageContext.Receiver)
            .SendCoreAsync(
                nameof(ISignalRClient.ReceiveClientMessage),
                new object[]{ messageContext, message});
    }
}

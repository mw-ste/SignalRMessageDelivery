using Microsoft.AspNetCore.SignalR;
using Shared;

namespace Backend.SignalR;

public interface ISignalRGateway
{
    Task SendMessageToClient(
        SignalRMessageContext messageContext, 
        string message);
    
    Task<bool> SendMessageToClientConnection(
        SignalRMessageContext messageContext, 
        string message,
        string connectionId);
}

public class SignalRGateway : ISignalRGateway
{
    private readonly IHubContext<SignalRHub> _hubContext;
    private readonly IHubContext<SignalRHub, ISignalRClient> _typedHubContext;

    public SignalRGateway(
        IHubContext<SignalRHub> hubContext, 
        IHubContext<SignalRHub, ISignalRClient> typedHubContext)
    {
        _hubContext = hubContext;
        _typedHubContext = typedHubContext;
    }

    public async Task SendMessageToClient(
        SignalRMessageContext messageContext, 
        string message)
    {
        await _hubContext
            .Clients
            .Group(messageContext.MessageContext.Receiver)
            .SendCoreAsync(
                nameof(ISignalRClient.ReceiveClientMessage),
                new object[]{ messageContext, message});
    }

    public async Task<bool> SendMessageToClientConnection(
        SignalRMessageContext messageContext, 
        string message, 
        string connectionId)
    {
        //problem: this doesn't fail in any way, if the call is not working...
        //problem: can't have return types even on typed hub context
        //problem: better not keep track of connection Ids yourself!

        await _typedHubContext
            .Clients
            .Client(connectionId)
            .ReceiveClientMessage(messageContext, message);

        return true;
    }
}

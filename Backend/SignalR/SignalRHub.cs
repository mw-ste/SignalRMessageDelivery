using Microsoft.AspNetCore.SignalR;
using Shared;

namespace Backend.SignalR;

public class SignalRHub : Hub<ISignalrClient>, ISignalrHub
{
    private readonly ISignalRDispatcher _signalRDispatcher;

    public SignalRHub(ISignalRDispatcher signalRDispatcher)
    {
        _signalRDispatcher = signalRDispatcher;
    }

    public Task SendMessageToBackEnd(MessageContext messageContext, string message)
    {
        return _signalRDispatcher.PublishClientMessageAnswer(messageContext, message);
    }

    public async Task RegisterClient(string clientId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, clientId);
        await Clients.Caller.ReceiveRegistrationSuccess();
    }
}
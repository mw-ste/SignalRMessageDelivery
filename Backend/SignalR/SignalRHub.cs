using Microsoft.AspNetCore.SignalR;
using Shared;

namespace Backend.SignalR;

public class SignalRHub : Hub<ISignalrClient>, ISignalrHub
{
    public Task SendMessageToBackEnd(MessageContext messageContext, string message)
    {
        throw new NotImplementedException();
    }

    public async Task RegisterClient(string clientId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, clientId);
        //await Clients.Caller.RegisterClientSuccess(clientId);
    }
}
using Microsoft.AspNetCore.SignalR;
using Shared;

namespace Backend.SignalR;

public class SignalRHub : Hub<ISignalRClient>, ISignalRHub
{
    private readonly ISignalRDispatcher _signalRDispatcher;
    private readonly ILogger<SignalRHub> _logger;

    public SignalRHub(
        ISignalRDispatcher signalRDispatcher,
        ILogger<SignalRHub> logger)
    {
        _signalRDispatcher = signalRDispatcher;
        _logger = logger;
    }

    public async Task SendAnswerToBackEnd(SignalRMessageContext messageContext, string answer)
    {
        _logger.LogInformation($"Received message answer: {messageContext}");
        await _signalRDispatcher.PublishClientMessageAnswer(messageContext, answer);
    }

    public async Task RegisterClient(string clientId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, clientId);
        await _signalRDispatcher.AddSignalRConnection(clientId, Context.ConnectionId);
        await Clients.Caller.ReceiveRegistrationSuccess();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await _signalRDispatcher.RemoveSignalRConnection(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}
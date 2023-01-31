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

    public async Task SendMessageToBackEnd(MessageContext messageContext, string message)
    {
        _logger.LogInformation($"Received message answer: {messageContext}");
        await Task.Delay(TimeSpan.FromSeconds(2));
        await _signalRDispatcher.PublishClientMessageAnswer(messageContext, message);
    }

    public async Task RegisterClient(string clientId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, clientId);
        await Clients.Caller.ReceiveRegistrationSuccess();
    }
}
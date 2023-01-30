using Microsoft.AspNetCore.SignalR.Client;
using Shared;

namespace SignalRClient;

public class SignalREchoClient : ISignalRClient
{
    private readonly HubConnection _hubConnection;
    private readonly string _name;

    public event Action? ConnectionClosed;

    public SignalREchoClient(HubConnection hubConnection, string name)
    {
        _hubConnection = hubConnection;
        _name = name;
        SubscribeToHub();

        _hubConnection.Closed += OnClosed;
    }

    public async Task ConnectToHub()
    {
        await _hubConnection.StartAsync();
    }

    private Task OnClosed(Exception? exception)
    {
        Console.WriteLine(
            "Hub connection was closed!\n" +
            $"Reason: \"{exception?.Message}\"");

        return Disconnect();
    }

    private void SubscribeToHub()
    {
        _hubConnection.On<MessageContext, string>(nameof(ReceiveClientMessage), ReceiveClientMessage);
        _hubConnection.On(nameof(ReceiveRegistrationSuccess), ReceiveRegistrationSuccess);
    }

    public Task RegisterClient()
    {
        Console.WriteLine($"Registering client {_name} ...");
        return _hubConnection.SendCoreAsync("RegisterClient", new object[] { _name });
    }

    public Task ReceiveRegistrationSuccess()
    {
        Console.WriteLine($"... client {_name} successfully registered.");
        return Task.CompletedTask;
    }

    public async Task ReceiveClientMessage(MessageContext messageContext, string message)
    {
        Console.WriteLine($"Received message with id {messageContext.MessageId} from {messageContext.Sender}: {message}");
        await Task.Delay(TimeSpan.FromSeconds(3));
        await SendMessageAnswer(messageContext.Reverse(), message);
    }

    public Task SendMessageAnswer(MessageContext messageContext, string message)
    {
        Console.WriteLine($"Sending message with id {messageContext.MessageId} to {messageContext.Receiver}: {message}");
        return _hubConnection.SendCoreAsync("SendMessageToBackEnd", new object[] { messageContext, message });
    }

    public async Task Disconnect()
    {
        await _hubConnection.StopAsync();
        await _hubConnection.DisposeAsync();
        ConnectionClosed?.Invoke();
    }
}
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
        LogToConsoleWithTimeStamp(
            "Hub connection was closed!\n" +
            $"Reason: \"{exception?.Message}\"");

        return Disconnect();
    }

    private void SubscribeToHub()
    {
        _hubConnection.On<SignalRMessageContext, string>(nameof(ReceiveClientMessage), ReceiveClientMessage);
        _hubConnection.On(nameof(ReceiveRegistrationSuccess), ReceiveRegistrationSuccess);
    }

    public Task RegisterClient()
    {
        LogToConsoleWithTimeStamp($"Registering client {_name} ...");
        
        return _hubConnection.SendCoreAsync(
            nameof(ISignalRHub.RegisterClient), 
            new object[] { _name });
    }

    public Task ReceiveRegistrationSuccess()
    {
        LogToConsoleWithTimeStamp($"... client {_name} successfully registered.");
        return Task.CompletedTask;
    }

    public Task ReceiveClientMessage(SignalRMessageContext messageContext, string message)
    {
        //TODO awaiting this would make it run synchronously, thus blocking all further calls from SignalR
        //await OnReceiveClientMessage(messageContext, message);

        Task.Run(() => ProcessMessage(messageContext, message));
        return Task.FromResult(Task.CompletedTask);
    }

    private async Task ProcessMessage(SignalRMessageContext messageContext, string message)
    {
        LogToConsoleWithTimeStamp($"Received message {messageContext}");
        await Task.Delay(RandomDelay.DelayInSeconds(2, 2));
        await SendMessageAnswer(messageContext.Reverse(), message);
    }

    public async Task SendMessageAnswer(SignalRMessageContext messageContext, string message)
    {
        LogToConsoleWithTimeStamp($"Sending message {messageContext}");
        await _hubConnection.SendCoreAsync(
            nameof(ISignalRHub.SendAnswerToBackEnd), 
            new object[] { messageContext, message });
    }

    public async Task Disconnect()
    {
        await _hubConnection.StopAsync();
        await _hubConnection.DisposeAsync();
        ConnectionClosed?.Invoke();
    }

    private static void LogToConsoleWithTimeStamp(string text)
    {
        Console.WriteLine($"[{DateTime.Now.ToUniversalTime()}] {text}");
    }
}
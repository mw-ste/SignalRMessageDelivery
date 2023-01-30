namespace Shared;

public interface ISignalRHub
{
    Task SendMessageToBackEnd(MessageContext messageContext, string message);
    Task RegisterClient(string clientId);
}
namespace Shared;

public interface ISignalRHub
{
    Task SendAnswerToBackEnd(SignalRMessageContext messageContext, string answer);
    Task RegisterClient(string clientId);
}
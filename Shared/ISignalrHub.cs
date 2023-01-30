namespace Shared;

public interface ISignalrHub
{
    Task SendMessageToBackEnd(MessageContext messageContext, string message);
}
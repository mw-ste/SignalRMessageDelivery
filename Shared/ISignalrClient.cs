namespace Shared;

public interface ISignalrClient
{
    Task ReceiveClientMessage(MessageContext messageContext, string message);
    Task ReceiveRegistrationSuccess();
}
namespace Shared;

public interface ISignalRClient
{
    Task ReceiveClientMessage(MessageContext messageContext, string message);
    Task ReceiveRegistrationSuccess();
}
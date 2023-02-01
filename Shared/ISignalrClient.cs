namespace Shared;

public interface ISignalRClient
{
    Task ReceiveClientMessage(SignalRMessageContext messageContext, string message);
    Task ReceiveRegistrationSuccess();
}
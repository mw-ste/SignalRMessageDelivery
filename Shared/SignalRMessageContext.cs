namespace Shared;

public record SignalRMessageContext(MessageContext MessageContext, DateTime Timestamp)
{
    public override string ToString() => $"{MessageContext} [{Timestamp}]";

    public SignalRMessageContext Reverse() => this with { MessageContext = MessageContext.Reverse() };
}
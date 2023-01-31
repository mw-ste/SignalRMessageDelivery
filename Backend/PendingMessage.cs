using Backend.Messaging;
using Backend.SignalR;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Shared;

namespace Backend;

public class PendingMessage : Aggregate<string>
{
    public bool Failed => TimeToLive <= 0;
    public string Method { get; set; }
    public MessageContext MessageContext { get; set; }
    public object[] Arguments { get; set; }

    public int TimeToLive { get; set; }
    public DateTime SendTimestamp { get; set; }
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(5);

#pragma warning disable CS8618
    [JsonConstructor]
    protected PendingMessage() : base("")
    {
        //only used for deserialization
    }
#pragma warning restore CS8618

    public PendingMessage(
        string method,
        MessageContext context,
        params object[] arguments)
        : base(context.MessageId)
    {
        Method = method;
        MessageContext = context;
        Arguments = arguments;
        TimeToLive = 3;
    }

    public Task Send(IHubContext<SignalRHub> hubContext)
    {
        TimeToLive -= 1;
        SendTimestamp = DateTime.Now.ToUniversalTime();

        return SendInternal(hubContext.Clients.Groups(MessageContext.Receiver));
    }

    private Task SendInternal(IClientProxy clientProxy) => 
        clientProxy.SendCoreAsync(Method, Arguments.Prepend(MessageContext).ToArray());

    public async Task<bool> Retry(IHubContext<SignalRHub> hubContext, IMediator mediator)
    {
        if (Failed)
        {
            await mediator.Publish(new MessageFailedEvent(MessageContext.Reverse()));
            return false;
        }

        await Send(hubContext);
        return true;
    }

    public PendingMessage Revive()
    {
        TimeToLive = 3;
        return this;
    }
}
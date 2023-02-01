using Backend.Messaging;
using Newtonsoft.Json;
using Shared;

namespace Backend.Aggregates;

public class PendingMessage : Aggregate<string>
{
    public bool Retryable => TimeToLive > 0;

    public string Message { get; set; }
    public MessageContext MessageContext { get; set; }

    public int TimeToLive { get; set; }
    public DateTime SendTimestamp { get; set; }
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(5);

    #region json constructor

    [JsonConstructor]
    #pragma warning disable CS8618
    protected PendingMessage() 
        : base("")
    {
    }
    #pragma warning restore CS8618

    #endregion

    public PendingMessage(
        string message,
        MessageContext context) 
        : base(context.MessageId)
    {
        Message = message;
        MessageContext = context;
        TimeToLive = 3;
    }

    public void Send()
    {
        if (!Retryable)
        {
            AddEvent(new MessageFailedEvent(MessageContext.Reverse()));
            return;
        }

        TimeToLive -= 1;
        SendTimestamp = DateTime.UtcNow;

        AddEvent(new MessageSentEvent(MessageContext, Message));
    }

    public PendingMessage Revive()
    {
        TimeToLive = 3;
        return this;
    }
}
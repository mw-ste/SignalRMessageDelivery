using Backend.Messaging;
using MediatR;
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
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(3);

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
        MessageContext messageContext,
        string message) 
        : base(messageContext.MessageId)
    {
        Message = message;
        MessageContext = messageContext;
        TimeToLive = 5;
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

        AddEvent(new MessageSentToSignalREvent(new SignalRMessageContext(MessageContext, SendTimestamp), Message));
    }

    public void HandleAnswer(SignalRMessageContext messageContext, string answer)
    {
        if (messageContext.MessageContext.MessageId != Id ||
            messageContext.Timestamp != SendTimestamp)
        {
            return;
        }

        AddEvent(new AnswerReceivedEvent(messageContext.MessageContext, answer));
    }

    public PendingMessage Revive()
    {
        TimeToLive = 3;
        return this;
    }
}

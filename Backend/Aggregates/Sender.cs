using Backend.Messaging;
using Shared;

namespace Backend.Aggregates;

public class Sender : Aggregate<string>
{
    public List<string> MessageLog { get; init; } = new List<string>();

    public Sender(string id) : base(id)
    {
    }

    public void SendMessage(string message, string client)
    {
        var messageId = Guid.NewGuid().ToString();
        var messageContext = new MessageContext(Id, client, messageId);
        LogMessage(messageContext, message);
        AddEvent(new MessageSentEvent(messageContext, message));
    }

    public void ReceiveAnswer(MessageContext messageContext, string answer) => 
        LogMessage(messageContext, answer);

    private void LogMessage(MessageContext messageContext, string message) => 
        MessageLog.Add($"{messageContext}: {message}");
}
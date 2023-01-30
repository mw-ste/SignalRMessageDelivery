using Backend.Messaging;

namespace Backend;

public class Sender : Aggregate<string>
{
    private readonly List<string> _messageLog = new List<string>();
    public IEnumerable<string> MessageLog => _messageLog;

    public Sender(string id) : base(id)
    {
    }

    public void SendMessage(string message, string client)
    {
        var messageId = Guid.NewGuid().ToString();
        LogMessage(message, messageId, Id, client);
        AddEvent(new SendMessageRequestedEvent(Id, client, message, messageId));
    }

    public void ReceiveAnswer(string message, string client, string messageId)
    {
        LogMessage(message, messageId, client, Id);
    }

    private void LogMessage(string message, string messageId, string sender, string receiver) => 
        _messageLog.Add($"[{messageId}] {sender} --> {receiver}: {message}");
}
﻿using Backend.Messaging;

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
        LogMessage(message, messageId, Id, client);
        AddEvent(new MessageCreatedEvent(Id, client, message, messageId));
    }

    public void ReceiveAnswer(string message, string client, string messageId) => 
        LogMessage(message, messageId, client, Id);

    private void LogMessage(string message, string messageId, string sender, string receiver) =>
        MessageLog.Add($"[{messageId}] {sender} --> {receiver}: {message}");

}
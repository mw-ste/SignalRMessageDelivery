﻿namespace Shared;

public record MessageContext(string Sender, string Receiver, string MessageId)
{
    public virtual MessageContext Reverse() => this with { Sender = Receiver, Receiver = Sender };

    public override string ToString() => $"{Sender} --> {Receiver} ({MessageId})";
}
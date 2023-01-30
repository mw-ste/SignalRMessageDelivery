using Backend.Messaging;

namespace Backend;

public class Sender : Aggregate<string>
{
    public Sender(string id) : base(id)
    {
    }

    public void SendMessage(string message, string client)
    {
        //await hubContext
        //    .Clients
        //    .Groups(client)
        //    .SendAsync(
        //        "ReceiveClientMessage",
        //        new MessageContext(Id, client, Guid.NewGuid().ToString()),
        //        message);

        AddEvent(new SendMessageRequestedEvent(Id, client, message));
    }
}
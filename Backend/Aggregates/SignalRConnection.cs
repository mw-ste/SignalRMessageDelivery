namespace Backend.Aggregates;

public class SignalRConnection : Aggregate<string>
{
    public string ConnectionId { get; set; }

    public SignalRConnection(
        string clientName,
        string connectionId) 
        : base(clientName)
    {
        ConnectionId = connectionId;
    }
}
using Backend.Aggregates;

namespace Backend.Database;

public interface ISignalRConnectionDatabase : IDatabase<SignalRConnection, string>
{
}

public class SignalRConnectionInMemoryDatabase : InMemoryDatabase<SignalRConnection, string>, ISignalRConnectionDatabase
{
}
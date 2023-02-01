using Backend.Aggregates;

namespace Backend.Database;

public interface IPendingMessageDatabase : IDatabase<PendingMessage, string>
{
}

public class PendingMessageInMemoryDatabase : InMemoryDatabase<PendingMessage, string>, IPendingMessageDatabase
{
}
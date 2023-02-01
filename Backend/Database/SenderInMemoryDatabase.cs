using Backend.Aggregates;

namespace Backend.Database;

public interface ISenderDatabase : IDatabase<Sender, string>
{
}

public class SenderInMemoryDatabase : InMemoryDatabase<Sender, string>, ISenderDatabase
{
}
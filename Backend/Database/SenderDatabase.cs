using Backend.Aggregates;
using MediatR;

namespace Backend.Database;

public interface ISenderDatabase : IDatabase<Sender, string>
{
}

public class SenderInMemoryDatabase : InMemoryDatabase<Sender, string>, ISenderDatabase
{
    public SenderInMemoryDatabase(IMediator mediator) : base(mediator)
    {
    }
}
using Backend.Aggregates;
using MediatR;

namespace Backend.Database;

public interface IPendingMessageDatabase : IDatabase<PendingMessage, string>
{
}

public class PendingMessageInMemoryDatabase : InMemoryDatabase<PendingMessage, string>, IPendingMessageDatabase
{
    public PendingMessageInMemoryDatabase(IMediator mediator) : base(mediator)
    {
    }
}
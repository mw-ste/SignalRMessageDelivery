using Backend.Aggregates;
using MediatR;

namespace Backend.Database;

public interface IPendingMessageRepository : IRepository<PendingMessage, string>
{
}

public class PendingMessageRepository : Repository<PendingMessage, string>, IPendingMessageRepository
{
    public PendingMessageRepository(
        IPendingMessageDatabase database, 
        IMediator mediator,
        ILogger<PendingMessageRepository> logger) 
        : base(database, mediator, logger)
    {
    }
}
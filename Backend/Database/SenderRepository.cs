using Backend.Aggregates;
using MediatR;

namespace Backend.Database;

public interface ISenderRepository : IRepository<Sender, string>
{
}

public class SenderRepository : Repository<Sender, string>, ISenderRepository
{
    public SenderRepository(
        ISenderDatabase database, 
        IMediator mediator, 
        ILogger<SenderRepository> logger) 
        : base(database, mediator, logger)
    {
    }
}
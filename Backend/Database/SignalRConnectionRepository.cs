using Backend.Aggregates;
using MediatR;

namespace Backend.Database;

public interface ISignalRConnectionRepository : IRepository<SignalRConnection, string>
{
}

public class SignalRConnectionRepository : Repository<SignalRConnection, string>, ISignalRConnectionRepository
{
    public SignalRConnectionRepository(
        ISignalRConnectionDatabase database,
        IMediator mediator,
        ILogger<SignalRConnectionRepository> logger)
        : base(database, mediator, logger)
    {
    }
}
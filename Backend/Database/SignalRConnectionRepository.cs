using Backend.Aggregates;
using MediatR;

namespace Backend.Database;

public interface ISignalRConnectionRepository : IRepository<SignalRConnection, string>
{
    Task<SignalRConnection?> TryFindByConnectionId(string connectionId);
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

    public async Task<SignalRConnection?> TryFindByConnectionId(string connectionId) => 
        (await List()).SingleOrDefault(c => c.ConnectionId == connectionId);
}
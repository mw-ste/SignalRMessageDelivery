using Backend.Aggregates;
using MediatR;
using IMediator = MediatR.IMediator;

namespace Backend.Database;

public interface IRepository<T, in TId> where T : Aggregate<TId>
{
    Task<IEnumerable<T>> List();
    Task<T> Find(TId id);
    Task<T?> TryFind(TId id);
    Task Save(T entity);
    Task Delete(TId id);
}

public abstract class Repository<T, TId> : IRepository<T, TId>
    where T : Aggregate<TId>
    where TId : notnull
{
    private readonly IDatabase<T, TId> _database;
    private readonly IMediator _mediator;
    private readonly ILogger<Repository<T, TId>> _logger;

    protected Repository(
        IDatabase<T, TId> database,
        IMediator mediator,
        ILogger<Repository<T, TId>> logger)
    {
        _database = database;
        _mediator = mediator;
        _logger = logger;
    }

    public Task<IEnumerable<T>> List() =>
        _database.List();

    public async Task<T> Find(TId id)
    {
        var aggregate = await TryFind(id);
        if (aggregate == null)
        {
            throw new Exception($"Not aggregate of type {typeof(T)} with id {id} found!");
        }
        return aggregate;
    }

    public Task<T?> TryFind(TId id)
        => _database.TryFind(id);
    
    public Task Save(T entity)
    {
        var events = entity.ClearEvents();
        _database.Save(entity);
        return SendEvents(events);
    }

    private Task SendEvents(IEnumerable<INotification> events)
    {
        var tasks = events.Select(SendEvent).ToArray();
        return Task.WhenAll(tasks);
    }

    private async Task SendEvent(INotification notification) => 
        await _mediator.PublishWithRetryOnException<ReTryableException>(notification);

    public Task Delete(TId id) =>
        _database.Delete(id);
}
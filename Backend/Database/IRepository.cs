using Backend.Aggregates;
using MediatR;

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

    protected Repository(
        IDatabase<T, TId> database,
        IMediator mediator)
    {
        _database = database;
        _mediator = mediator;
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
        _database.Save(entity);
        return entity.SendEvents(_mediator);
    }

    public Task Delete(TId id) =>
        _database.Delete(id);
}
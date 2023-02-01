using Backend.Aggregates;
using MediatR;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace Backend.Database;

public interface IDatabase<T, TId> where T : Aggregate<TId>
{
    Task<IEnumerable<T>> List();
    Task<T> Find(TId id);
    Task<T?> TryFind(TId id);
    Task Save(T entity);
    Task Delete(TId id);
}

public abstract class InMemoryDatabase<T, TId> : IDatabase<T, TId>
    where T : Aggregate<TId>
    where TId : notnull
{
    private readonly IMediator _mediator;

    protected InMemoryDatabase(IMediator mediator)
    {
        _mediator = mediator;
    }

    private readonly ConcurrentDictionary<TId, string> _database =
        new ConcurrentDictionary<TId, string>();

    public Task<IEnumerable<T>> List() =>
        Task.FromResult(_database.Select(kvp => TryFindInternal(kvp.Key)!));

    public Task<T> Find(TId id)
    {
        var aggregate = TryFindInternal(id);
        if (aggregate == null)
        {
            throw new Exception($"Not aggregate of type {typeof(T)} with id {id} found!");
        }
        return Task.FromResult(aggregate);
    }

    public Task<T?> TryFind(TId id)
    {
        var aggregate = TryFindInternal(id);
        return Task.FromResult(aggregate);
    }

    private T? TryFindInternal(TId id)
    {
        return _database.TryGetValue(id, out var value)
            ? JsonConvert.DeserializeObject<T?>(value)
            : null;
    }

    public Task Save(T entity)
    {
        _database[entity.Id]= JsonConvert.SerializeObject(entity);
        return entity.SendEvents(_mediator);
    }

    public Task Delete(TId id)
    {
        _database.Remove(id, out _);
        return Task.CompletedTask;
    }
}
using MediatR;
using System.Collections.Concurrent;
using System.Text.Json;

namespace Backend;

public interface IDatabase<T, TId> where T : Aggregate<TId>
{
    Task<IEnumerable<T>> List();

    Task<T> Find(TId id);

    Task Save(T entity, IMediator mediator);

    Task Delete(TId id);
}

public abstract class InMemoryDatabase<T, TId> : IDatabase<T, TId>
    where T : Aggregate<TId>
    where TId : notnull
{
    private readonly ConcurrentDictionary<TId, string> _database =
        new ConcurrentDictionary<TId, string>();

    public Task<IEnumerable<T>> List() =>
        Task.FromResult(_database.Select(kvp => kvp.Value).Select(s => JsonSerializer.Deserialize<T>(s)))!;

    public Task<T> Find(TId id)
    {
        var aggregate = JsonSerializer.Deserialize<T>(_database[id]);
        if (aggregate == null)
        {
            throw new Exception($"Not aggregate of type {typeof(T)} with id {id} found!");
        }
        return Task.FromResult(aggregate);
    }

    public Task Save(T entity, IMediator mediator)
    {
        _database[entity.Id]= JsonSerializer.Serialize(entity);
        return entity.SendEvents(mediator);
    }

    public Task Delete(TId id)
    {
        _database.Remove(id, out _);
        return Task.CompletedTask;
    }
}
using System.Collections.Concurrent;
using Backend.Aggregates;
using Newtonsoft.Json;

namespace Backend.Database;

public interface IDatabase<T, in TId> where T : Aggregate<TId> where TId : notnull
{
    Task<IEnumerable<T>> List();
    Task<T?> TryFind(TId id);
    Task Save(T entity);
    Task Delete(TId id);
}

public abstract class InMemoryDatabase<T, TId> : IDatabase<T, TId> where T : Aggregate<TId> where TId : notnull
{
    private readonly ConcurrentDictionary<TId, string> _database =
        new ConcurrentDictionary<TId, string>();

    private readonly ConcurrentDictionary<TId, Guid> _tags =
        new ConcurrentDictionary<TId, Guid>();

    public Task<IEnumerable<T>> List() =>
        Task.FromResult(_database.Select(kvp => TryFindInternal(kvp.Key)!));

    public Task<T?> TryFind(TId id)
    {
        var aggregate = TryFindInternal(id);
        return Task.FromResult(aggregate);
    }

    private T? TryFindInternal(TId id)
    {
        if (!_database.TryGetValue(id, out var value))
        {
            return null;
        }

        var aggregate = JsonConvert.DeserializeObject<T?>(value);


        if (aggregate == null)
        {
            return null;
        }

        var tag = Guid.NewGuid();
        aggregate.DatabaseTag = tag;
        _tags[id] = tag;
        return aggregate;

    }

    public Task Save(T entity)
    {
        var entityId = entity.Id;

        if (_tags.ContainsKey(entityId) && entity.DatabaseTag != _tags[entityId])
        {
            throw new DatabaseTagMismatchException();
        }

        _database[entityId] = JsonConvert.SerializeObject(entity);
        return Task.CompletedTask;
    }

    public Task Delete(TId id)
    {
        _database.Remove(id, out _);
        _tags.Remove(id, out _);
        return Task.CompletedTask;
    }
}
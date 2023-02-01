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

    public Task<IEnumerable<T>> List() =>
        Task.FromResult(_database.Select(kvp => TryFindInternal(kvp.Key)!));

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
        _database[entity.Id] = JsonConvert.SerializeObject(entity);
        return Task.CompletedTask;
    }

    public Task Delete(TId id)
    {
        _database.Remove(id, out _);
        return Task.CompletedTask;
    }
}
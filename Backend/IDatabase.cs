using System.Collections.Concurrent;
using MediatR;

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
    private readonly ConcurrentDictionary<TId, T> _objects =
        new ConcurrentDictionary<TId, T>();

    public Task<IEnumerable<T>> List() =>
        Task.FromResult(_objects.Select(kvp => kvp.Value));

    public Task<T> Find(TId id) =>
        Task.FromResult(_objects[id]);

    public Task Save(T entity, IMediator mediator)
    {
        _objects[entity.Id] = entity;
        return entity.SendEvents(mediator);
    }

    public Task Delete(TId id)
    {
        _objects.Remove(id, out _);
        return Task.CompletedTask;
    }
}
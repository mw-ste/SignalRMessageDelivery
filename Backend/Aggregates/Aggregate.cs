using MediatR;

namespace Backend.Aggregates;

public abstract class Aggregate<TId>
{
    public Guid DatabaseTag { get; set; }
    private readonly List<INotification> _events = new List<INotification>();

    protected Aggregate(TId id)
    {
        Id = id;
    }

    public TId Id { get; init; }

    public void AddEvent(INotification notification) => _events.Add(notification);

    public IEnumerable<INotification> ClearEvents()
    {
        var events = _events.ToList();
        _events.Clear();
        return events;
    }
}
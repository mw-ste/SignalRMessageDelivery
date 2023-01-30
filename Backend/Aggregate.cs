using MediatR;

namespace Backend;

public abstract class Aggregate<TId>
{
    private readonly List<INotification> _events = new List<INotification>();

    protected Aggregate(TId id)
    {
        Id = id;
    }

    public TId Id { get; private set; }

    public void AddEvent(INotification notification) => _events.Add(notification);

    public async Task SendEvents(IMediator mediator)
    {
        foreach (var notification in _events)
        {
            await mediator.Publish(notification);
        }
        _events.Clear();
    }
}
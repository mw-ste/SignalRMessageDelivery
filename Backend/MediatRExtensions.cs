using MediatR;

namespace Backend;

public static class MediatRExtensions
{
    public static async Task PublishWithRetryOnException<T>(
        this IMediator mediator, 
        INotification notification, 
        int retries = 3, 
        double retryDelayInSeconds = 3,
        CancellationToken cancellationToken = default) 
        where T : Exception
    {
        await DoWithRetryOnException<T>(
            notification, 
            retries, 
            retryDelayInSeconds, 
            cancellationToken, 
            mediator.Publish);
    }

    public static async Task SendWithRetryOnException<T>(
        this IMediator mediator,
        INotification notification,
        int retries = 3,
        double retryDelayInSeconds = 3,
        CancellationToken cancellationToken = default)
        where T : Exception
    {
        await DoWithRetryOnException<T>(
            notification,
            retries,
            retryDelayInSeconds,
            cancellationToken,
            mediator.Send);
    }

    private static async Task DoWithRetryOnException<T>(
        INotification notification, 
        int retries, 
        double retryDelayInSeconds,
        CancellationToken cancellationToken, 
        Func<INotification, CancellationToken, Task> publish) 
        where T : Exception
    {
        while (true)
        {
            try
            {
                await publish(notification, cancellationToken);
                return;
            }
            catch (T)
            {
                if (retries-- <= 0)
                {
                    throw;
                }

                await Task.Delay(TimeSpan.FromSeconds(retryDelayInSeconds), cancellationToken);
            }
        }
    }
}
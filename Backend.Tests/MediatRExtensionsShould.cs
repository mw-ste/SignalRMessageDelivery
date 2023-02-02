using MediatR;
using NSubstitute;
using Xunit;

namespace Backend.Tests;

public class MediatRExtensionsShould
{
    private readonly IMediator _mediator;
    private readonly INotification _notification;

    public MediatRExtensionsShould()
    {
        _mediator = Substitute.For<IMediator>();
        _notification = Substitute.For<INotification>();
    }

    [Fact]
    public async Task ReSendMessages()
    {
        _mediator
            .Send(Arg.Any<INotification>())
            .Returns(
                _ => throw new Exception(),
                _ => throw new Exception(),
                _ => Task.FromResult<object?>(null));

        // ReSharper disable once InvokeAsExtensionMethod
        await MediatRExtensions.SendWithRetryOnException<Exception>(
            _mediator, 
            _notification,
            3, 
            0);

        await _mediator
            .Received(3)
            .Send(_notification);
    }

    [Fact]
    public async Task RePublishMessages()
    {
        _mediator
            .Publish(Arg.Any<INotification>())
            .Returns(
                _ => throw new Exception(),
                _ => throw new Exception(),
                _ => Task.FromResult<object?>(null));

        // ReSharper disable once InvokeAsExtensionMethod
        await MediatRExtensions.PublishWithRetryOnException<Exception>(
            _mediator,
            _notification,
            3,
            0);

        await _mediator
            .Received(3)
            .Publish(_notification);
    }

    [Fact]
    public async Task FailAfterRunningOutOfRetries()
    {
        _mediator
            .Publish(Arg.Any<INotification>())
            .Returns(_ => throw new Exception());

        // ReSharper disable once InvokeAsExtensionMethod
        await Assert.ThrowsAsync<Exception>(() =>
            MediatRExtensions.PublishWithRetryOnException<Exception>(
                _mediator,
                _notification,
                3,
                0));

        await _mediator
            .Received(4)
            .Publish(_notification);
    }
}
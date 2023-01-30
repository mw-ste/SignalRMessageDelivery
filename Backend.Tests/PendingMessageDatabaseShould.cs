using MediatR;
using NSubstitute;
using Shared;
using Xunit;

namespace Backend.Tests;

public class PendingMessageDatabaseShould
{
    private readonly PendingMessageInMemoryDatabase _sut;

    public PendingMessageDatabaseShould()
    {
        _sut = new PendingMessageInMemoryDatabase();
    }

    private const string MessageId = "messageId";

    [Fact]
    public async Task SerializeAndDeserialize()
    {
        var pendingMessage = new PendingMessage(
            "Method",
            new MessageContext("Sender", "Receiver", MessageId),
            "Argument1",
            "Argument2")
        {
            RetryDelay = TimeSpan.FromSeconds(1),
            TimeToLive = 42
        };

        await _sut.Save(pendingMessage, Substitute.For<IMediator>());
        var result = await _sut.Find(MessageId);

        Assert.NotNull(result);
        Assert.Equal(MessageId, result.Id);
        Assert.Equal("Method", result.Method);
        Assert.Equal(TimeSpan.FromSeconds(1), result.RetryDelay);
        Assert.Equal(42, result.TimeToLive);
        Assert.Equal(2, result.Arguments.Length);
        Assert.Equal("Argument1", result.Arguments.First());
        Assert.Equal("Argument2", result.Arguments.Last());
    }
}
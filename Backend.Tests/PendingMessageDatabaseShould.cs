using Backend.Aggregates;
using Backend.Database;
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
            "Message",
            new MessageContext("Sender", "Receiver", MessageId))
        {
            RetryDelay = TimeSpan.FromSeconds(1),
            TimeToLive = 42
        };

        await _sut.Save(pendingMessage);
        var result = await _sut.TryFind(MessageId);

        Assert.NotNull(result);
        Assert.Equal(MessageId, result.Id);
        Assert.Equal("Message", result.Message);
        Assert.Equal(TimeSpan.FromSeconds(1), result.RetryDelay);
        Assert.Equal(42, result.TimeToLive);
    }
}
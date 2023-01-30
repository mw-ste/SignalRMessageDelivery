using MediatR;
using NSubstitute;
using Xunit;

namespace Backend.Tests;

public class SenderDatabaseShould
{
    private readonly SenderInMemoryDatabase _sut;

    public SenderDatabaseShould()
    {
        _sut = new SenderInMemoryDatabase();
    }

    private const string SenderId = "senderId";

    [Fact]
    public async Task SerializeAndDeserialize()
    {
        var sender = new Sender(SenderId);
        sender.SendMessage("Message", "Client");
        sender.ReceiveAnswer("Answer", "Client", "MessageId");

        await _sut.Save(sender, Substitute.For<IMediator>());
        var result = await _sut.Find(SenderId);

        Assert.NotNull(result);
        Assert.Equal(2, result.MessageLog.Count);
    }
}
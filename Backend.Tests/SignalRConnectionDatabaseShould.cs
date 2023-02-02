using Backend.Aggregates;
using Backend.Database;
using Xunit;

namespace Backend.Tests;

public class SignalRConnectionDatabaseShould
{
    private const string ClientName = "ClientName";
    private const string ConnectionId = "ConnectionId";
    private readonly SignalRConnectionInMemoryDatabase _sut;

    public SignalRConnectionDatabaseShould()
    {
        _sut = new SignalRConnectionInMemoryDatabase();
    }

    [Fact]
    public async Task SerializeAndDeserialize()
    {
        var connection = new SignalRConnection(ClientName, ConnectionId);

        await _sut.Save(connection);
        var result = await _sut.TryFind(ClientName);

        Assert.NotNull(result);
        Assert.Equal(ClientName, result!.Id);
        Assert.Equal(ConnectionId, result.ConnectionId);
    }
}
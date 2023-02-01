using Backend.Database;
using MediatR;

namespace Backend.Messaging;

public record SendMessageCommand(string Sender, string Client, string Message) : IRequest;

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand>
{
    private readonly ISenderDatabase _database;

    public SendMessageCommandHandler(ISenderDatabase database)
    {
        _database = database;
    }

    public async Task<Unit> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var sender = await _database.Find(request.Sender);
        sender.SendMessage(request.Message, request.Client);
        await _database.Save(sender);
        return Unit.Value;
    }
}
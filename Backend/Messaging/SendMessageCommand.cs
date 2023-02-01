using Backend.Database;
using MediatR;

namespace Backend.Messaging;

public record SendMessageCommand(string Sender, string Client, string Message) : IRequest;

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand>
{
    private readonly ISenderRepository _repository;

    public SendMessageCommandHandler(ISenderRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var sender = await _repository.Find(request.Sender);
        sender.SendMessage(request.Message, request.Client);
        await _repository.Save(sender);
        return Unit.Value;
    }
}
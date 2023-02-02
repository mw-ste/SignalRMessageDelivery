using Backend.Aggregates;
using Backend.Database;
using MediatR;

namespace Backend.Messaging;

public record AddSignalRConnectionCommand(string ClientName, string ConnectionId) : IRequest;

public class AddSignalRConnectionCommandHandler : IRequestHandler<AddSignalRConnectionCommand>
{
   private readonly ISignalRConnectionRepository _connectionRepository;

   public AddSignalRConnectionCommandHandler(ISignalRConnectionRepository connectionRepository)
   {
       _connectionRepository = connectionRepository;
   }

   public async Task<Unit> Handle(AddSignalRConnectionCommand request, CancellationToken cancellationToken)
   {
       var signalRConnection = new SignalRConnection(request.ClientName, request.ConnectionId);
       await _connectionRepository.Save(signalRConnection);
       return Unit.Value;
   }
}
using Backend.Database;
using MediatR;

namespace Backend.Messaging;

public record RemoveSignalRConnectionCommand(string ConnectionId) : IRequest;

public class RemoveSignalRConnectionCommandHandler : IRequestHandler<RemoveSignalRConnectionCommand>
{
   private readonly ISignalRConnectionRepository _connectionRepository;

   public RemoveSignalRConnectionCommandHandler(ISignalRConnectionRepository connectionRepository)
   {
       _connectionRepository = connectionRepository;
   }

   public async Task<Unit> Handle(RemoveSignalRConnectionCommand request, CancellationToken cancellationToken)
   {
       var connection = await _connectionRepository.TryFindByConnectionId(request.ConnectionId);
       if (connection == null)
       {
           return Unit.Value;
       }
        
       await _connectionRepository.Delete(connection.ConnectionId);
       return Unit.Value;
   }
}
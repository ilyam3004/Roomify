using ErrorOr;
using MediatR;

namespace Roomify.Application.Messages.Commands.RemoveMessage;

public record RemoveMessageCommand(
    string MessageId, 
    string ConnectionId
) : IRequest<ErrorOr<Deleted>>;
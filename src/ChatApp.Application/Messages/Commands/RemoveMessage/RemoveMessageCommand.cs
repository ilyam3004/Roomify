using ErrorOr;
using MediatR;

namespace ChatApp.Application.Messages.Commands.RemoveMessage;

public record RemoveMessageCommand(
    string MessageId, 
    string ConnectionId
) : IRequest<ErrorOr<Deleted>>;
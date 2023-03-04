using ChatApp.Application.Common.Interfaces;
using ChatApp.Domain.Common.Errors;
using ErrorOr;
using MediatR;

namespace ChatApp.Application.Messages.Commands.RemoveMessage;

public class RemoveMessageCommandHandler : IRequestHandler<RemoveMessageCommand, ErrorOr<Deleted>>
{
    private readonly IUnitOfWork _unitOfWork;

    public RemoveMessageCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Deleted>> Handle(
        RemoveMessageCommand command,
        CancellationToken cancellationToken)
    {
        var message = await _unitOfWork
            .Messages.GetMessageByIdOrNullIfNotExists(command.MessageId);
        if (message is null)
        {
            return Errors.Message.MessageNotFound;
        }

        var user = await _unitOfWork
            .Users.GetUserByConnectionIdOrNull(command.ConnectionId);
        if (user is null)
        {
            return Errors.User.UserNotFound;
        }

        if (user.UserId == message.UserId)
        {
            await _unitOfWork.Messages.RemoveMessageById(message.MessageId);
            return Result.Deleted;
        }

        return Errors.Message.MessageIsNotRemoved;
    }
}

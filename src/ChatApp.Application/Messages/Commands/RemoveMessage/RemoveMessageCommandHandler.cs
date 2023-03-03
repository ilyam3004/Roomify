using ChatApp.Application.Common.Interfaces.Persistence;
using ChatApp.Domain.Common.Errors;
using ErrorOr;
using MediatR;

namespace ChatApp.Application.Messages.Commands.RemoveMessage;

public class RemoveMessageCommandHandler 
    : IRequestHandler<RemoveMessageCommand, ErrorOr<Deleted>>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;
    public RemoveMessageCommandHandler(
        IMessageRepository messageRepository, 
        IUserRepository userRepository)
    {
        _messageRepository = messageRepository;
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<Deleted>> Handle(
        RemoveMessageCommand command, 
        CancellationToken cancellationToken)
    {
        var message = await _messageRepository.GetMessageByIdOrNullIfNotExists(command.MessageId);
        if (message is null)
        {
            return Errors.Message.MessageNotFound;
        }

        var user = await _userRepository.GetUserByConnectionIdOrNull(command.ConnectionId);
        if (user is null)
        {
            return Errors.User.UserNotFound;
        }

        if (user.UserId == message.UserId)
        {
            await _messageRepository.RemoveMessageById(message.MessageId);
            return Result.Deleted;
            
        }

        return Errors.Message.MessageIsNotRemoved;
    }
}
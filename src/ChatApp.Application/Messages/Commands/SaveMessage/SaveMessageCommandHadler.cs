using ChatApp.Application.Common.Interfaces.Persistence;
using ChatApp.Application.Models.Responses;
using ChatApp.Application.Common.Errors;
using ChatApp.Domain.Common.Errors;
using ChatApp.Domain.Entities;
using FluentValidation;
using MapsterMapper;
using ErrorOr;
using MediatR;

namespace ChatApp.Application.Messages.Commands.SaveMessage;

public class SaveMessageCommandHandler : 
    IRequestHandler<SaveMessageCommand, ErrorOr<MessageResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IValidator<SaveMessageCommand> _textMessageValidator;
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;

    public SaveMessageCommandHandler(
        IUserRepository userRepository,
        IValidator<SaveMessageCommand> textMessageValidator,
        IMessageRepository messageRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _textMessageValidator = textMessageValidator;
        _messageRepository = messageRepository;
        _mapper = mapper;
    }

    public async Task<ErrorOr<MessageResponse>> Handle(
        SaveMessageCommand command, 
        CancellationToken cancellationToken)
    {
        if (!await _userRepository.UserExists(command.UserId))
        {
            return Errors.User.UserNotFound;
        }
        
        var validateResult = await _textMessageValidator.ValidateAsync(command);

        if (!validateResult.IsValid)
        {
            return ErrorConverter.ConvertValidationErrors(validateResult.Errors);
        }
        
        var dbMessage = await _messageRepository.SaveMessage(new Message
        {
            MessageId = Guid.NewGuid().ToString(),
            UserId = command.UserId,
            RoomId = command.RoomId,
            Text = command.Text,
            Date = DateTime.UtcNow,
            FromUser = command.FromUser,
            IsImage = false,
            ImageUrl = ""
        });

        var user = await _userRepository.GetUserById(dbMessage.UserId);

        return _mapper.Map<MessageResponse>((dbMessage, user));
    }
}
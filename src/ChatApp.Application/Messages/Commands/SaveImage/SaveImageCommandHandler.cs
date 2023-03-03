using ChatApp.Application.Common.Interfaces.Persistence;
using ChatApp.Application.Models.Responses;
using ChatApp.Application.Common.Errors;
using ChatApp.Domain.Common.Errors;
using ChatApp.Domain.Entities;
using MapsterMapper;
using ErrorOr;
using MediatR;
using FluentValidation;

namespace ChatApp.Application.Messages.Commands.SaveImage;

public class SaveImageCommandHandler : 
    IRequestHandler<SaveImageCommand, ErrorOr<MessageResponse>>
{
    private readonly IValidator<SaveImageCommand> _imageMessageValidator;
    private readonly IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public SaveImageCommandHandler(
        IUserRepository userRepository,
        IMessageRepository messageRepository,
        IValidator<SaveImageCommand> imageMessageValidator,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _messageRepository = messageRepository;
        _imageMessageValidator = imageMessageValidator;
        _mapper = mapper;
    }

    public async Task<ErrorOr<MessageResponse>> Handle(
        SaveImageCommand command, 
        CancellationToken cancellationToken)
    {
        if (!await _userRepository.UserExists(command.UserId))
        {
            return Errors.User.UserNotFound;
        }
        
        var validateResult = await _imageMessageValidator.ValidateAsync(command);

        if (!validateResult.IsValid)
        {
            return ErrorConverter.ConvertValidationErrors(validateResult.Errors);
        }

        var dbMessage = await _messageRepository.SaveMessage(new Message
        {
            MessageId = Guid.NewGuid().ToString(),
            RoomId = command.RoomId,
            UserId = command.UserId,
            Text = "",
            Date = DateTime.UtcNow,
            FromUser = true,
            IsImage = true,
            ImageUrl = command.ImageUrl
        });

        var user = await _userRepository.GetUserById(dbMessage.UserId);
            
        return _mapper.Map<MessageResponse>((dbMessage, user));
    }
}
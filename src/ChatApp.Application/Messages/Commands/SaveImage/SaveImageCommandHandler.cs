using ChatApp.Application.Common.Interfaces.Persistence;
using ChatApp.Application.Models.Responses;
using ChatApp.Application.Common.Errors;
using ChatApp.Domain.Common.Errors;
using ChatApp.Domain.Entities;
using MapsterMapper;
using ErrorOr;
using MediatR;

namespace ChatApp.Application.Messages.Commands.SaveImage;

public class SaveImageCommandHandler : 
    IRequestHandler<SaveImageCommand, ErrorOr<MessageResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly SaveImageCommandValidator _imageMessageValidator;
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;

    public SaveImageCommandHandler(
        IUserRepository userRepository,
        IMessageRepository messageRepository,
        SaveImageCommandValidator imageMessageValidator,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _messageRepository = messageRepository;
        _imageMessageValidator = imageMessageValidator;
        _mapper = mapper;
    }

    public async Task<ErrorOr<MessageResponse>> Handle(
        SaveImageCommand request, 
        CancellationToken cancellationToken)
    {
        if (!await _userRepository.UserExists(request.UserId))
        {
            return Errors.User.UserNotFound;
        }
        
        var validateResult = await _imageMessageValidator.ValidateAsync(request);

        if (!validateResult.IsValid)
        {
            return ErrorConverter.ConvertValidationErrors(validateResult.Errors);
        }

        var dbMessage = await _messageRepository.SaveMessage(new Message
        {
            MessageId = Guid.NewGuid().ToString(),
            RoomId = request.RoomId,
            UserId = request.UserId,
            Text = "",
            Date = DateTime.UtcNow,
            FromUser = true,
            IsImage = true,
            ImageUrl = request.ImageUrl
        });

        var user = await _userRepository.GetUserById(dbMessage.UserId);
            
        return _mapper.Map<MessageResponse>((dbMessage, user));
    }
}
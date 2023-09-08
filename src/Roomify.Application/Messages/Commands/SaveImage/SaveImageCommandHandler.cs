using ErrorOr;
using FluentValidation;
using MapsterMapper;
using MediatR;
using Roomify.Application.Common.Errors;
using Roomify.Application.Common.Interfaces;
using Roomify.Application.Models.Responses;
using Roomify.Domain.Common.Errors;
using Roomify.Domain.Entities;

namespace Roomify.Application.Messages.Commands.SaveImage;

public class SaveImageCommandHandler : 
    IRequestHandler<SaveImageCommand, ErrorOr<MessageResponse>>
{
    private readonly IValidator<SaveImageCommand> _imageMessageValidator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SaveImageCommandHandler(
        IUnitOfWork unitOfWork,
        IValidator<SaveImageCommand> imageMessageValidator,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _imageMessageValidator = imageMessageValidator;
        _mapper = mapper;
    }

    public async Task<ErrorOr<MessageResponse>> Handle(
        SaveImageCommand command, 
        CancellationToken cancellationToken)
    {
        if (!await _unitOfWork.Users.UserExists(command.UserId))
        {
            return Errors.User.UserNotFound;
        }
        
        var validateResult = await _imageMessageValidator
            .ValidateAsync(command);

        if (!validateResult.IsValid)
        {
            return ErrorConverter.ConvertValidationErrors(validateResult.Errors);
        }

        var dbMessage = await _unitOfWork.Messages
            .SaveMessage(new Message
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

        var user = await _unitOfWork.Users
            .GetUserById(dbMessage.UserId);
            
        return _mapper.Map<MessageResponse>((dbMessage, user));
    }
}
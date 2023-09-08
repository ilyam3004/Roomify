using ErrorOr;
using FluentValidation;
using MapsterMapper;
using MediatR;
using Roomify.Application.Common.Errors;
using Roomify.Application.Common.Interfaces;
using Roomify.Application.Models.Responses;
using Roomify.Domain.Common.Errors;
using Roomify.Domain.Entities;

namespace Roomify.Application.Messages.Commands.SaveMessage;

public class SaveMessageCommandHandler : 
    IRequestHandler<SaveMessageCommand, ErrorOr<MessageResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<SaveMessageCommand> _textMessageValidator;
    private readonly IMapper _mapper;

    public SaveMessageCommandHandler(
        IUnitOfWork unitOfWork,
        IValidator<SaveMessageCommand> textMessageValidator,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _textMessageValidator = textMessageValidator;
        _mapper = mapper;
    }

    public async Task<ErrorOr<MessageResponse>> Handle(
        SaveMessageCommand command, 
        CancellationToken cancellationToken)
    {
        if (!await _unitOfWork.Users.UserExists(command.UserId))
        {
            return Errors.User.UserNotFound;
        }
        
        var validateResult = await _textMessageValidator.ValidateAsync(command);

        if (!validateResult.IsValid)
        {
            return ErrorConverter.ConvertValidationErrors(validateResult.Errors);
        }

        var messageToSave = new Message()
        {
            MessageId = Guid.NewGuid().ToString(),
            UserId = command.UserId,
            RoomId = command.RoomId,
            Text = command.Text,
            Date = DateTime.UtcNow,
            FromUser = command.FromUser,
            IsImage = false,
            ImageUrl = ""
        };
        
        var dbMessage = await _unitOfWork.Messages.SaveMessage(messageToSave);

        var user = await _unitOfWork.Users
            .GetUserById(dbMessage.UserId);

        return _mapper.Map<MessageResponse>((dbMessage, user));
    }
}
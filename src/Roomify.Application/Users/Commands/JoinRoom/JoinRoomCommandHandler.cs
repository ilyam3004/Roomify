using ErrorOr;
using FluentValidation;
using MapsterMapper;
using MediatR;
using Roomify.Application.Common.Errors;
using Roomify.Application.Common.Interfaces;
using Roomify.Application.Models.Responses;
using Roomify.Domain.Common.Errors;
using Roomify.Domain.Entities;

namespace Roomify.Application.Users.Commands.JoinRoom;

public class JoinUserCommandHandler :
    IRequestHandler<JoinRoomCommand, ErrorOr<UserResponse>>
{
    private readonly IValidator<JoinRoomCommand> _commandValidator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public JoinUserCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper, 
        IValidator<JoinRoomCommand> commandValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _commandValidator = commandValidator;
    }

    public async Task<ErrorOr<UserResponse>> Handle(
        JoinRoomCommand command, 
        CancellationToken cancellationToken)
    {

        var validateResult = await _commandValidator.ValidateAsync(command);

        if (!validateResult.IsValid)
        {
            return ErrorConverter.ConvertValidationErrors(validateResult.Errors);
        } 
        
        Room room = await _unitOfWork.Users
            .CreateRoomIfNotExists(command.RoomName);

        if (await _unitOfWork.Users
                .UserExists(command.Username, room.RoomId))
        {
            return Errors.User.DuplicateUsername;
        }

        var userToAdd = new User()
        {   
            UserId = Guid.NewGuid().ToString(),
            Username = command.Username,
            ConnectionId = command.ConnectionId,
            RoomId = room.RoomId,
            Avatar = command.Avatar,
            HasLeft = false
        };

        var dbUser = await _unitOfWork.Users.AddUser(userToAdd);
    
        return _mapper.Map<UserResponse>((dbUser, room));
    }
}
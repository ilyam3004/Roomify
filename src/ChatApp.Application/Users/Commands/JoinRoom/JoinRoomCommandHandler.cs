using ChatApp.Application.Models.Responses;
using ChatApp.Application.Common.Errors;
using ChatApp.Application.Common.Interfaces;
using ChatApp.Domain.Common.Errors;
using ChatApp.Domain.Entities;
using FluentValidation;
using MapsterMapper;
using MediatR;
using ErrorOr;

namespace ChatApp.Application.Users.Commands.JoinRoom;

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
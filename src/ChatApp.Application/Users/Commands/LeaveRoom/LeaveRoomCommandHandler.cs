using ChatApp.Application.Common.Interfaces.Persistence;
using ChatApp.Application.Models.Responses;
using ChatApp.Domain.Common.Errors;
using ChatApp.Domain.Entities;
using MapsterMapper;
using ErrorOr;
using MediatR;

namespace ChatApp.Application.Users.Commands.LeaveRoom;

public class LeaveRoomCommandHandler : 
    IRequestHandler<LeaveRoomCommand, ErrorOr<UserResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public LeaveRoomCommandHandler(
        IUserRepository userRepository, 
        IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<ErrorOr<UserResponse>> Handle(
        LeaveRoomCommand command, 
        CancellationToken cancellationToken)
    {
        User? user = await _userRepository
                .GetUserByConnectionIdOrNull(command.ConnectionId);
        if (user is null)
        {
            return Errors.User.UserNotFound;
        }

        Room room = await _userRepository.GetRoomById(user.RoomId);
        
        bool isRoomEmpty = await _userRepository
            .RemoveRoomDataIfEmpty(user.RoomId, user.UserId);

        if (isRoomEmpty) 
        {
            return Errors.Room.RoomIsEmpty;
        }

        return _mapper.Map<UserResponse>((user, room));
    }
}
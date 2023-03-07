using ChatApp.Application.Common.Interfaces;
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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public LeaveRoomCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ErrorOr<UserResponse>> Handle(
        LeaveRoomCommand command, 
        CancellationToken cancellationToken)
    {
        User? user = await _unitOfWork.Users
                .GetUserByConnectionIdOrNull(command.ConnectionId);
        if (user is null)
        {
            return Errors.User.UserNotFound;
        }

        Room room = await _unitOfWork.Users
            .GetRoomById(user.RoomId);
        
        bool isRoomEmpty = await _unitOfWork.Users
            .RemoveRoomDataIfEmpty(user.RoomId, user.UserId);

        if (isRoomEmpty) 
        {
            return Errors.Room.RoomIsEmpty;
        }

        return _mapper.Map<UserResponse>((user, room));
    }
}
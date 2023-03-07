using ChatApp.Application.Common.Interfaces;
using ChatApp.Application.Models.Responses;
using ChatApp.Domain.Common.Errors;
using ChatApp.Domain.Entities;
using MapsterMapper;
using ErrorOr;
using MediatR;

namespace ChatApp.Application.Users.Queries.GetUserByConnId;

public class GetUserByConnIdQueryHandler : 
    IRequestHandler<GetUserByConnIdQuery, ErrorOr<UserResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUserByConnIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ErrorOr<UserResponse>> Handle(
        GetUserByConnIdQuery query, 
        CancellationToken cancellationToken)
    {
        User? user = await _unitOfWork.Users
            .GetUserByConnectionIdOrNull(query.ConnectionId);

        if (user is null)
        {
            return Errors.User.UserNotFound;
        }

        Room room = await _unitOfWork.Users
            .GetRoomById(user.RoomId);

        return _mapper.Map<UserResponse>((user, room));
    }
}
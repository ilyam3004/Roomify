using ChatApp.Application.Common.Interfaces.Persistence;
using ChatApp.Application.Models.Responses;
using ChatApp.Domain.Common.Errors;
using ChatApp.Domain.Entities;
using MapsterMapper;
using ErrorOr;
using MediatR;

namespace ChatApp.Application.Users.Queries.GetUserByConnId;

public class GetUserByConnIdHandler : 
    IRequestHandler<GetUserByConnIdQuery, ErrorOr<UserResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUserByConnIdHandler(
        IUserRepository userRepository, 
        IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<ErrorOr<UserResponse>> Handle(
        GetUserByConnIdQuery query, 
        CancellationToken cancellationToken)
    {
        User? user = await _userRepository
            .GetUserByConnectionIdOrNull(query.ConnectionId);

        if (user is null)
        {
            return Errors.User.UserNotFound;
        }

        Room room = await _userRepository.GetRoomById(user.RoomId);

        return _mapper.Map<UserResponse>((user, room));
    }
}
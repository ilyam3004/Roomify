using ChatApp.Application.Common.Interfaces.Persistence;
using ChatApp.Application.Models.Responses;
using ChatApp.Domain.Entities;
using MapsterMapper;
using MediatR;

namespace ChatApp.Application.Users.Queries.GetUserList;

public class GetUserListQueryHandler 
    : IRequestHandler<GetUserListQuery, List<UserResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUserListQueryHandler(
        IUserRepository userRepository, 
        IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<List<UserResponse>> Handle(
        GetUserListQuery query, 
        CancellationToken cancellationToken)
    {
        Room room = await _userRepository.GetRoomById(query.RoomId);

        List<User> dbUsers = await _userRepository.GetRoomUsers(query.RoomId);

        return dbUsers.Select(user => _mapper.Map<UserResponse>((user, room))).ToList();
    }
}
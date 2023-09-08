using MapsterMapper;
using MediatR;
using Roomify.Application.Common.Interfaces;
using Roomify.Application.Models.Responses;
using Roomify.Domain.Entities;

namespace Roomify.Application.Users.Queries.GetUserList;

public class GetUserListQueryHandler 
    : IRequestHandler<GetUserListQuery, List<UserResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUserListQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<UserResponse>> Handle(
        GetUserListQuery query, 
        CancellationToken cancellationToken)
    {
        Room room = await _unitOfWork.Users
            .GetRoomById(query.RoomId);

        List<User> dbUsers = await _unitOfWork.Users
            .GetRoomUsers(query.RoomId);

        return dbUsers.Select(user => _mapper.Map<UserResponse>((user, room))).ToList();
    }
}
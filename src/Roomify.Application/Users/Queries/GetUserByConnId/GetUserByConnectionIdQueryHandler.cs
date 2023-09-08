using ErrorOr;
using MapsterMapper;
using MediatR;
using Roomify.Application.Common.Interfaces;
using Roomify.Application.Models.Responses;
using Roomify.Domain.Common.Errors;
using Roomify.Domain.Entities;

namespace Roomify.Application.Users.Queries.GetUserByConnId;

public class GetUserByConnectionIdQueryHandler : 
    IRequestHandler<GetUserByConnectionIdQuery, ErrorOr<UserResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUserByConnectionIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ErrorOr<UserResponse>> Handle(
        GetUserByConnectionIdQuery query, 
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
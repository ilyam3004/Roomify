using MediatR;
using Roomify.Application.Models.Responses;

namespace Roomify.Application.Users.Queries.GetUserList;

public record GetUserListQuery(
    string RoomId) : IRequest<List<UserResponse>>;
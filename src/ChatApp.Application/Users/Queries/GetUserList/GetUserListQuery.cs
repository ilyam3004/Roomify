using ChatApp.Application.Models.Responses;
using ErrorOr;
using MediatR;

namespace ChatApp.Application.Users.Queries.GetUserList;

public record GetUserListQuery(
    string RoomId) : IRequest<List<UserResponse>>;
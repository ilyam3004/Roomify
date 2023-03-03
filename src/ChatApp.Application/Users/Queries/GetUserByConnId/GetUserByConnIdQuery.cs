using ChatApp.Application.Models.Responses;
using ErrorOr;
using MediatR;

namespace ChatApp.Application.Users.Queries.GetUserByConnId;

public record GetUserByConnIdQuery(
    string ConnectionId) : IRequest<ErrorOr<UserResponse>>;
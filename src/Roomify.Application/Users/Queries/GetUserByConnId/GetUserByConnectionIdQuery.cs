using ErrorOr;
using MediatR;
using Roomify.Application.Models.Responses;

namespace Roomify.Application.Users.Queries.GetUserByConnId;

public record GetUserByConnectionIdQuery(
    string ConnectionId) : IRequest<ErrorOr<UserResponse>>;
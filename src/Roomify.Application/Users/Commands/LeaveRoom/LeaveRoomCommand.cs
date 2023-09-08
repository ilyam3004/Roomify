using ErrorOr;
using MediatR;
using Roomify.Application.Models.Responses;

namespace Roomify.Application.Users.Commands.LeaveRoom;

public record LeaveRoomCommand(
    string ConnectionId) : IRequest<ErrorOr<UserResponse>>;
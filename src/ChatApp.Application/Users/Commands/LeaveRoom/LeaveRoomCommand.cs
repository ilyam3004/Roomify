using ChatApp.Application.Models.Responses;
using ErrorOr;
using MediatR;

namespace ChatApp.Application.Users.Commands.LeaveRoom;

public record LeaveRoomCommand(
    string ConnectionId) : IRequest<ErrorOr<UserResponse>>;
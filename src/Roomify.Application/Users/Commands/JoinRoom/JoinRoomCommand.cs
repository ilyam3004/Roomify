using ErrorOr;
using MediatR;
using Roomify.Application.Models.Responses;

namespace Roomify.Application.Users.Commands.JoinRoom;

public record JoinRoomCommand(
    string Username,
    string RoomName,
    string ConnectionId,
    string Avatar) : IRequest<ErrorOr<UserResponse>>;
using ChatApp.Application.Models.Responses;
using ErrorOr;
using MediatR;

namespace ChatApp.Application.Users.JoinRoom;

public record JoinRoomCommand(
    string Username,
    string RoomName,
    string ConnectionId,
    string Avatar) : IRequest<ErrorOr<UserResponse>>;
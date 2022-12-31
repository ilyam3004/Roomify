namespace ChatApp.Application.Models;

public record UserResponse(
    string UserId,
    string Username,
    string ConnectionId,
    string RoomId);
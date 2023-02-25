namespace ChatApp.Application.Models.Requests;

public record CreateUserRequest(
    string Username,
    string ConnectionId,
    string RoomName,
    string Avatar);
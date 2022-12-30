namespace ChatApp.Contracts.Rooms;

public record UserRequest(
    string Username,
    string ConnectionId,
    string RoomName);
namespace ChatApp.Contracts.Rooms;

public record JoinUserRequest(
    string Username,
    string RoomName);
namespace ChatApp.Contracts.Rooms;

public record JoinRoomRequest(
    string Username,
    string RoomName,
    string Avatar);
namespace Roomify.Contracts.Rooms.Requests;

public record JoinRoomRequest(
    string Username,
    string RoomName,
    string Avatar);
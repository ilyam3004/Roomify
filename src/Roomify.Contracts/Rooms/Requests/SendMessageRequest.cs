namespace Roomify.Contracts.Rooms.Requests;

public record SendMessageRequest(
    string UserId,
    string Username,
    string RoomId,
    string Text,
    bool FromUser);


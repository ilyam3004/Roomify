namespace ChatApp.Contracts.Rooms;

public record SendMessageRequest(
    string UserId,
    string Username,
    string RoomId,
    string Text,
    DateTime Date,
    bool FromUser);


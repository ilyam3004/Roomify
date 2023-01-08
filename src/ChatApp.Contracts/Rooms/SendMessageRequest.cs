namespace ChatApp.Contracts.Rooms;

public record SendMessageRequest(
    string UserId,
    string RoomId,
    string Text,
    DateTime Date,
    bool FromUser);


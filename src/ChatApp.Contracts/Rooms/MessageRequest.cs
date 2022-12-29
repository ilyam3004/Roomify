namespace ChatApp.Contracts.Rooms;

public record MessageRequest(
    string UserId,
    string RoomId,
    string Text,
    DateTime Date,
    bool FromUser);


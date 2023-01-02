namespace ChatApp.Contracts.Rooms;

public record MessageRequest(
    string UserId,
    string RoomName,
    string Text,
    DateTime Date,
    bool FromUser);


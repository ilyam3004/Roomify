namespace ChatApp.Application.Models;

public record MessageResponse(
    string MessageId,
    string UserId,
    string RoomId,
    string Text,
    DateTime Date,
    bool FromUser);


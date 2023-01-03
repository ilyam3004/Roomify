namespace ChatApp.Application.Models.Requests;

public record SaveMessageRequest(
    string UserId,
    string RoomId,
    string Text,
    DateTime Date,
    bool FromUser);
namespace ChatApp.Application.Models.Requests;

public record SaveMessageRequest(
    string UserId,
    string Username,
    string RoomId,
    string Text,
    DateTime Date,
    bool FromUser);
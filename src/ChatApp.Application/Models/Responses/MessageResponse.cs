namespace ChatApp.Application.Models.Responses;

public record MessageResponse(
    string MessageId,
    string Username,
    string RoomName,
    string Text,
    DateTime Date,
    bool FromUser);

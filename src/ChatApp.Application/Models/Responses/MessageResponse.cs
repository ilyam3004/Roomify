namespace ChatApp.Application.Models.Responses;

public record MessageResponse(
    string MessageId,
    string Username,
    string UserId,
    string UserAvatar,
    string RoomId,
    string Text,
    DateTime Date,
    bool FromUser,
    bool IsImage,
    string ImageUrl);

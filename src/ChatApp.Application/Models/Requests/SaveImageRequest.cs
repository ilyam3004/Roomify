namespace ChatApp.Application.Models.Requests;

public record SaveImageRequest(
    string UserId,
    string Username,
    string RoomId,
    string ImageUrl);

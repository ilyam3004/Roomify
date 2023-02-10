namespace ChatApp.Application.Models.Requests;

public record SaveImageRequest(
    string UserId,
    string RoomId,
    string ImageUrl);

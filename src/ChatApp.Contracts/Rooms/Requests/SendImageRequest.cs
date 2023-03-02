namespace ChatApp.Contracts.Rooms;

public record SendImageRequest(
    string UserId,
    string RoomId,
    string ImageUrl);
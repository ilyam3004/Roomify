namespace Roomify.Contracts.Rooms.Requests;

public record SendImageRequest(
    string UserId,
    string RoomId,
    string ImageUrl);
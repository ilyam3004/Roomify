namespace Roomify.Contracts.Rooms.Responses;

public record UploadResultResponse(
    string PublicId,
    string ImgUrl);
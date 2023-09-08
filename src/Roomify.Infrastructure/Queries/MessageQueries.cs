namespace Roomify.Infrastructure.Queries;

public static class MessageQueries
{
    public static readonly string RemoveMessage = "DELETE FROM Message " + 
        "WHERE MessageId = @MessageId";

    public static readonly string SaveMessage = "INSERT INTO Message " +
        "(MessageId, UserId, RoomId, Text, Date, FromUser, isImage, ImageUrl) " +
        "VALUES (@MessageId, @UserId, @RoomId, @Text, @Date, @FromUser, @isImage, @ImageUrl)";
    
    public static readonly string GetAllRoomMessages = "SELECT * FROM Message " + 
        "WHERE RoomId = @RoomId";
    
    public static readonly string GetMessageById = "SELECT * FROM Message " + 
        "WHERE MessageId = @MessageId";

    public static readonly string RemoveAllMessagesFromRoom = "DELETE FROM Message " + 
        "WHERE RoomId = @RoomId";
}
namespace ChatApp.Infrastructure.Queries;

public static class UserQueries
{
    public static readonly string GetUserById = "SELECT * FROM [ChatUser] " +
                                                "WHERE UserId = @UserId";
    
    public static readonly string GetRoomUsers = "SELECT * FROM [ChatUser] " +
                                                 "WHERE RoomId = @RoomId AND HasLeft = 'FALSE'";

    public static readonly string GetRoomById = "SELECT * FROM Room " +
                                                "WHERE RoomId = @RoomId";

    public static readonly string GetUserByConnectionId = "SELECT * FROM [ChatUser] " +
                                                          "WHERE ConnectionId = @ConnectionId";

    public static readonly string CountOfUsersByUserId = "SELECT COUNT(*) FROM [ChatUser] " +
                                                         "WHERE UserId = @UserId";

    public static readonly string CountOfUsersByUsername = "SELECT COUNT(*) FROM [ChatUser] " +
                                                           "WHERE RoomId = @RoomId AND Username = @Username AND HasLeft = 'FALSE'";

    public static readonly string UpdateUserStatus = "UPDATE [ChatUser] " +
                                                     "SET HasLeft = 'TRUE' WHERE UserId = @UserId";

    public static readonly string CountOfUsersInRoom = "SELECT COUNT(*) FROM [ChatUser] " +
                                                       "WHERE RoomId = @RoomId AND HasLeft = 'FALSE'";

    public static readonly string CountOfRooms = "SELECT COUNT(*) " +
                                                 "FROM Room WHERE RoomName = @RoomName";

    public static readonly string GetRoomByRoomName = "SELECT * FROM Room " +
                                                      "WHERE RoomName = @RoomName";

    public static readonly string RemoveAllUsersFromRoom = "DELETE FROM [ChatUser] " +
                                                           "WHERE RoomId = @RoomId";

    public static readonly string RemoveRoom = "DELETE FROM Room " +
                                               "WHERE RoomId = @RoomId";

    public static string AddUser =
        "INSERT INTO [ChatUser] "
        + "(UserId, Username, ConnectionId, RoomId, HasLeft, Avatar) "
        + "VALUES (@UserId, @Username, @ConnectionId, @RoomId, @HasLeft, @Avatar)";

    public static string CreateRoom = "INSERT INTO Room " +
                                      "(RoomId, RoomName) VALUES (@RoomId, @RoomName)";
}
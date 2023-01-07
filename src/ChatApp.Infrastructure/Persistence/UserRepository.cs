using Dapper;
using System.Data;
using ChatApp.Domain.Entities;
using ChatApp.Infrastructure.Config;
using ChatApp.Application.Common.Interfaces.Persistence;

namespace ChatApp.Infrastructure.Persistence;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;
    private readonly IMessageRepository _messageRepository;

    public UserRepository(AppDbContext dbContext, IMessageRepository messageRepository)
    {
        _dbContext = dbContext;
        _messageRepository = messageRepository;
    }

    public async Task<User> AddUser(User user)
    {
        var query = "INSERT INTO [User] (UserId, Username, ConnectionId, RoomId) VALUES (@UserId, @Username, @ConnectionId, @RoomId)";

        using var connection = _dbContext.CreateConnection();
        await connection.ExecuteAsync(query, user);
        var dbUser = await GetUserById(user.UserId);
        
        return dbUser;
    }

    public async Task<User> GetUserById(string userId)
    {
        var query = "SELECT * FROM [User] WHERE UserId = @userId";

        using var connection = _dbContext.CreateConnection();
        var user = await connection.QueryFirstOrDefaultAsync<User>(query, new { userId });
        
        return user;
    }

    public async Task<Room> CreateRoomIfNotExists(string roomName)
    {
        if (await RoomExistsByRoomName(roomName))
        {
            return await GetRoomByRoomName(roomName);
        }

        string query = "INSERT INTO Room (RoomId, RoomName) VALUES (@RoomId, @RoomName)";
        
        var roomId = Guid.NewGuid();

        using var connection = _dbContext.CreateConnection();
        await connection.ExecuteAsync(query, 
            GetRoomInsertionQueryParameters(roomId.ToString(), roomName));
        
        return await GetRoomById(roomId.ToString());
    }

    public async Task<List<User>> GetRoomUsers(string roomId)
    {
        var query = "SELECT * FROM [User] WHERE RoomId = @RoomId";

        using var connection = _dbContext.CreateConnection();
        List<User> users = await connection.QueryFirstOrDefaultAsync<List<User>>(query, new { RoomId = roomId });

        return users;
    }

    public async Task<bool> UserExists(string username, string roomId)
    {
        var query = "SELECT COUNT(*) FROM [User] WHERE Username = @Username AND RoomId = @RoomId";

        using var connection = _dbContext.CreateConnection();
        int count = await connection
            .QueryFirstOrDefaultAsync<int>(query, 
                new { Username = username, RoomId = roomId });
            
        return count != 0;
    }

    public async Task<bool> UserExists(string userId)
    {
        var query = "SELECT COUNT(*) FROM [User] WHERE UserId = @UserId";

        using var connection = _dbContext.CreateConnection();
        int count = await connection.QueryFirstOrDefaultAsync<int>(query, 
            new { UserId = userId });

        return count != 0;
    }

    public async Task<bool> RemoveUserFromRoom(string userId)
    {
        string removeUserQuery = "DELETE FROM [User] WHERE UserId = @UserId";

        using var connection = _dbContext.CreateConnection();
        
        await connection.ExecuteAsync(removeUserQuery, 
            new { UserId = userId });

        await RemoveRoomIfEmpty(await GetRoomIdByUserId(userId));

        return !(await UserExists(userId));
    }

    private async Task<string> GetRoomIdByUserId(string userId)
    {
        string query = "SELECT RoomId FROM [User] WHERE UserId = @UserId";

        var connection = _dbContext.CreateConnection();

        string roomId = await connection.QueryFirstOrDefaultAsync<string>(query, 
            new { UserId = userId });

        return roomId;
    }

    private async Task<Room> GetRoomById(string roomId)
    {
        var query = "SELECT * FROM Room WHERE RoomId = @RoomId";

        using (var connection = _dbContext.CreateConnection())
        {
            Room room = await connection.QueryFirstOrDefaultAsync<Room>(query, new { roomId });
            return room;
        }
    }

    public async Task<string> GetRoomIdByConnectionId(string connectionId)
    {
        string query = "SELECT RoomId FROM [User] WHERE ConnectionId = @ConnectionId";
        
        using var connection = _dbContext.CreateConnection();
        string roomId =  await connection.QueryFirstOrDefaultAsync<string>(query, new { ConnectionId = connectionId });

        return roomId;
    }

    public async Task<bool> RoomExists(string roomId)
    {
        string query = "SELECT COUNT(*) FROM Room WHERE RoomId = @RoomId";

        using var connection = _dbContext.CreateConnection();
        int count = await connection.QueryFirstOrDefaultAsync<int>(query,
            new { RoomId = roomId });

        return count != 0;
    }

    private async Task<Room> GetRoomByRoomName(string roomName)
    {
        var query = "SELECT * FROM Room WHERE RoomName = @RoomName";

        using var connection = _dbContext.CreateConnection();
        Room room = await connection.QueryFirstOrDefaultAsync<Room>(
            query, new { RoomName = roomName });
        
        return room;
    }
    
    private async Task RemoveRoomIfEmpty(string roomId)
    {
        var messagesDeleted = await _messageRepository.RemoveAllMessagesFromRoom(roomId);
        
        string query = "SELECT COUNT(*) FROM [User] WHERE RoomId = @RoomId";

        using var connection = _dbContext.CreateConnection();
        int count = await connection.QueryFirstOrDefaultAsync<int>(query, 
            new { RoomId = roomId });

        if (count == 0 && messagesDeleted)
            await RemoveRoom(roomId);
    }

    private async Task RemoveRoom(string roomId)
    {
        string query = "DELETE FROM Room WHERE RoomId = @RoomId";

        using var connection = _dbContext.CreateConnection();
        await connection.ExecuteAsync(query, new { RoomId = roomId });
    }
    
    private async Task<bool> RoomExistsByRoomName(string roomName)
    {
        var query = "SELECT COUNT(*) FROM Room WHERE RoomName = @RoomName";

        using var connection = _dbContext.CreateConnection();
        int count = await connection
            .QueryFirstOrDefaultAsync<int>(query, new { RoomName = roomName  });
        return count != 0;
    }

    private DynamicParameters GetRoomInsertionQueryParameters(string roomId, string roomName)
    {
        DynamicParameters parameters = new();
        {
            parameters.Add("RoomId", roomId, DbType.String);
            parameters.Add("RoomName", roomName, DbType.String);
        }
        
        return parameters;
    }

    private DynamicParameters GetUserInsertionParameters(User user)
    {
        DynamicParameters parameters = new();
        {
            parameters.Add("UserId", user.UserId, DbType.String);
            parameters.Add("Username", user.Username, DbType.String);
            parameters.Add("ConnectionId", user.ConnectionId, DbType.String);
            parameters.Add("RoomId", user.RoomId, DbType.String);
        }

        return new DynamicParameters();
    }
}
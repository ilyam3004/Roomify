using ChatApp.Application.Common.Interfaces.Persistence;
using ChatApp.Domain.Entities;
using System.Data;
using Dapper;
using Microsoft.Extensions.Options;
using ChatApp.Infrastructure.Config;

namespace ChatApp.Infrastructure.Interfaces.Persistence;

public class UserRepository : IUserRepository
{
    private readonly IDbConnection  _connection;
    private readonly IDbTransaction _transaction;
    
    private readonly IMessageRepository _messageRepository;

    public UserRepository(
        IDbTransaction transaction, 
        IDbConnection connection)
    {
        _messageRepository = new MessageRepository(
            new IOptions<CloudinarySettings>(), 
            connection, 
            transaction);
        _transaction = transaction;
        _connection = connection;
    }

    public async Task<User> AddUser(User user)
    {
        var query = "INSERT INTO [ChatUser] (UserId, Username, ConnectionId, RoomId, HasLeft, Avatar) " +
                    "VALUES (@UserId, @Username, @ConnectionId, @RoomId, @HasLeft, @Avatar)";

        using var connection = _dbContext.CreateConnection();
        await connection.ExecuteAsync(query, user);

        return user;
    }

    public async Task<User> GetUserById(string userId)
    {
        var query = "SELECT * FROM [ChatUser] WHERE UserId = @userId";

        var user = await _connection
            .QueryFirstOrDefaultAsync<User>(query, new {userId}, _transaction);

        return user;
    }

    public async Task<Room> CreateRoomIfNotExists(string roomName)
    {
        if (await RoomExistsByRoomName(roomName))
        {
            return await GetRoomByRoomName(roomName);
        }

        string query = "INSERT INTO Room (RoomId, RoomName) VALUES (@RoomId, @RoomName)";

        var room = new Room
        {
            RoomId = Guid.NewGuid().ToString(),
            RoomName = roomName
        };

        using var connection = _dbContext.CreateConnection();
        await connection.ExecuteAsync(query, room);

        return room;
    }

    public async Task<List<User>> GetRoomUsers(string roomId)
    {
        var query = "SELECT * FROM [ChatUser] WHERE RoomId = @RoomId AND HasLeft = 'FALSE'";

        using var connection = _dbContext.CreateConnection();
        IEnumerable<User> users = await connection.QueryAsync<User>(query, new {RoomId = roomId});

        return users.ToList();
    }

    public async Task<Room> GetRoomById(string roomId)
    {
        var query = "SELECT * FROM Room WHERE RoomId = @RoomId";

        using var connection = _dbContext.CreateConnection();
        Room room = await connection.QueryFirstOrDefaultAsync<Room>(query, new {roomId});
        return room;
    }

    public async Task<bool> UserExists(string username, string roomId)
    {
        var query = "SELECT COUNT(*) FROM [ChatUser] WHERE Username = @Username AND RoomId = @RoomId AND HasLeft = 'FALSE'";

        using var connection = _dbContext.CreateConnection();
        int count = await connection
            .QueryFirstOrDefaultAsync<int>(query,
                new {Username = username, RoomId = roomId});

        return count != 0;
    }

    public async Task<User?> GetUserByConnectionIdOrNull(string connectionId)
    {
        string query = "SELECT * FROM [ChatUser] WHERE ConnectionId = @ConnectionId";

        using var connection = _dbContext.CreateConnection();
        User? user = await connection.QueryFirstOrDefaultAsync<User>(query, new {ConnectionId = connectionId});
        return user;
    }


    public async Task<bool> UserExists(string userId)
    {
        var query = "SELECT COUNT(*) FROM [ChatUser] WHERE UserId = @UserId";

        using var connection = _dbContext.CreateConnection();
        int count = await connection.QueryFirstOrDefaultAsync<int>(query,
            new { UserId = userId });

        return count != 0;
    }

    public async Task<bool> RoomExists(string roomId)
    {
        string query = "SELECT COUNT(*) FROM Room WHERE RoomId = @RoomId";

        using var connection = _dbContext.CreateConnection();
        int count = await connection.QueryFirstOrDefaultAsync<int>(query,
            new {RoomId = roomId});

        return count != 0;
    }

    private async Task<Room> GetRoomByRoomName(string roomName)
    {
        var query = "SELECT * FROM Room WHERE RoomName = @RoomName";

        using var connection = _dbContext.CreateConnection();
        Room room = await connection.QueryFirstOrDefaultAsync<Room>(
            query, new {RoomName = roomName});

        return room;
    }

    public async Task<bool> RemoveRoomDataIfEmpty(string roomId, string userId)
    {
        string query = "SELECT COUNT(*) FROM [ChatUser] WHERE RoomId = @RoomId AND HasLeft = 'FALSE'";

        using var connection = _dbContext.CreateConnection();
        int count = await connection.QueryFirstOrDefaultAsync<int>(query,
            new {RoomId = roomId});

        if (count == 1)
        {
            await _messageRepository.RemoveAllMessagesFromRoom(roomId);
            await RemoveAllUsersFromRoom(roomId);
            await RemoveRoom(roomId);
            return true;
        }
        
        await UpdateUserStatusToHasLeft(userId);
        return false;
    }

    private async Task RemoveAllUsersFromRoom(string roomId)
    {
        var query = "DELETE FROM [ChatUser] WHERE RoomId = @RoomId";

        var connection = _dbContext.CreateConnection();
        await connection.ExecuteAsync(query, new { RoomId = roomId });
    }

    public async Task UpdateUserStatusToHasLeft(string userId)
    {
        string query = "UPDATE [ChatUser] SET HasLeft = 'TRUE' WHERE UserId = @UserId";

        using var connection = _dbContext.CreateConnection();
        await connection.ExecuteAsync(query, new {UserId = userId});
    }

    private async Task RemoveRoom(string roomId)
    {
        string query = "DELETE FROM Room WHERE RoomId = @RoomId";

        using var connection = _dbContext.CreateConnection();
        await connection.ExecuteAsync(query, new {RoomId = roomId});
    }

    private async Task<bool> RoomExistsByRoomName(string roomName)
    {
        var query = "SELECT COUNT(*) FROM Room WHERE RoomName = @RoomName";

        using var connection = _dbContext.CreateConnection();
        int count = await connection
            .QueryFirstOrDefaultAsync<int>(query, new {RoomName = roomName});
        
        return count != 0;
    }
}
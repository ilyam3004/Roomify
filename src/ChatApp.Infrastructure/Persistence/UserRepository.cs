using Dapper;
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
        var query = "INSERT INTO [User] (UserId, Username, ConnectionId, RoomId, HasLeft) VALUES (@UserId, @Username, @ConnectionId, @RoomId, @HasLeft)";

        using var connection = _dbContext.CreateConnection();
        await connection.ExecuteAsync(query, user);

        return user;
    }

    public async Task<User> GetUserById(string userId)
    {
        var query = "SELECT * FROM [User] WHERE UserId = @userId";

        using var connection = _dbContext.CreateConnection();
        var user = await connection.QueryFirstOrDefaultAsync<User>(query, new {userId});

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
        var query = "SELECT * FROM [User] WHERE RoomId = @RoomId AND HasLeft = 'FALSE'";

        using var connection = _dbContext.CreateConnection();
        IEnumerable<User> users = await connection.QueryAsync<User>(query, new {RoomId = roomId});

        return users.ToList();
    }

    public async Task<Room?> GetRoomById(string roomId)
    {
        var query = "SELECT * FROM Room WHERE RoomId = @RoomId";

        using var connection = _dbContext.CreateConnection();
        Room? room = await connection.QueryFirstOrDefaultAsync<Room>(query, new {roomId});
        return room;
    }

    public async Task<bool> UserExists(string username, string roomId)
    {
        var query = "SELECT COUNT(*) FROM [User] WHERE Username = @Username AND RoomId = @RoomId AND HasLeft = 'FALSE'";

        using var connection = _dbContext.CreateConnection();
        int count = await connection
            .QueryFirstOrDefaultAsync<int>(query,
                new {Username = username, RoomId = roomId});

        return count != 0;
    }

    public async Task<User?> GetUserByConnectionIdOrNull(string connectionId)
    {
        string query = "SELECT * FROM [User] WHERE ConnectionId = @ConnectionId";

        using var connection = _dbContext.CreateConnection();
        User? user = await connection.QueryFirstOrDefaultAsync<User>(query, new {ConnectionId = connectionId});
        return user;
    }


    public async Task<bool> UserExists(string userId)
    {
        var query = "SELECT COUNT(*) FROM [User] WHERE UserId = @UserId";

        using var connection = _dbContext.CreateConnection();
        int count = await connection.QueryFirstOrDefaultAsync<int>(query,
            new {UserId = userId});

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

    public async Task RemoveRoomDataIfEmpty(string roomId, string userId)
    {
        string query = "SELECT COUNT(*) FROM [User] WHERE RoomId = @RoomId AND HasLeft = 'FALSE'";

        using var connection = _dbContext.CreateConnection();
        int count = await connection.QueryFirstOrDefaultAsync<int>(query,
            new {RoomId = roomId});

        if (count == 1)
        {
            await _messageRepository.RemoveAllMessagesFromRoom(roomId);
            await RemoveAllUsersFromRoom(roomId);
            await RemoveRoom(roomId);
        }
        
        await UpdateUserStatusToHasLeft(userId);
    }

    private async Task RemoveAllUsersFromRoom(string roomId)
    {
        var query = "DELETE FROM [User] WHERE RoomId = @RoomId";

        var connection = _dbContext.CreateConnection();
        await connection.ExecuteAsync(query, new { RoomId = roomId });
    }

    public async Task UpdateUserStatusToHasLeft(string userId)
    {
        string query = "UPDATE [User] SET HasLeft = 'TRUE' WHERE UserId = @UserId";

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
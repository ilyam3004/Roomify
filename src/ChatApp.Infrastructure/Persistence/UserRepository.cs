using Dapper;
using System.Data;
using ChatApp.Domain.Entities;
using ChatApp.Infrastructure.Config;
using ChatApp.Application.Common.Interfaces.Persistence;

namespace ChatApp.Infrastructure.Persistence;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User> AddUser(User user)
    {
        var query = "INSERT INTO User (UserId, Username, ConnectionId, RoomId) VALUES (@UserId, @Username, @ConnectionId, @RoomId)";

        var parameters = new DynamicParameters();
        parameters.Add("UserId", user.UserId, DbType.String);
        parameters.Add("Username", user.Username, DbType.String);
        parameters.Add("ConnectionId", user.ConnectionId, DbType.String);
        parameters.Add("RoomId", user.RoomId, DbType.String);

        using (var connection = _dbContext.CreateConnection())
        {
            await connection.ExecuteAsync(query, parameters);
        }

        var dbUser = await GetUserById(user.UserId);

        return dbUser;
    }

    public async Task<User> GetUserById(string userId)
    {
        var query = "SELECT * FROM [User] WHERE UserId = @userId";

        using (var connection = _dbContext.CreateConnection())
        {
            var user = await connection.QueryFirstOrDefaultAsync<User>(query, new { userId });
            if (user is null)
            {
                throw new NullReferenceException();
            }
            return user;
        }
    }

    public async Task<Room> AddRoom(string roomName)
    {
        string query = "INSERT INTO Room (RoomId, RoomName) VALUES (@RoomId, @RoomName)";

        var parameters = new DynamicParameters();
        
        var roomId = Guid.NewGuid();
        
        parameters.Add("RoomId", roomId.ToString(), DbType.String);
        parameters.Add("RoomId", roomName, DbType.String);

        using (var connection = _dbContext.CreateConnection())
        {
            await connection.ExecuteAsync(query, parameters);
            return await GetRoomById(roomId.ToString());
        }
    }

    public async Task<Room> GetRoomById(string roomId)
    {
        var query = "SELECT * FROM Room WHERE RoomId = @RoomId";

        using (var connection = _dbContext.CreateConnection())
        {
            Room room = await connection.QueryFirstOrDefaultAsync<Room>(query, new { roomId });
            return room;
        }
    }

    public Task<List<User>> GetRoomUsers(string roomName)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UserExists(string username)
    {
        var query = "SELECT COUNT(*) FROM [User] WHERE Username = @Username";

        using (var connection = _dbContext.CreateConnection())
        {
            int count = await connection.QueryFirstOrDefaultAsync<int>(query, new { username });
            return count != 0;
        }
    }

    public async Task<bool> RoomExists(string roomName)
    {
        var query = "SELECT COUNT(*) FROM Room WHERE RoomName = @RoomName";

        using (var connection = _dbContext.CreateConnection())
        {
            int count = await connection.QueryFirstOrDefaultAsync<int>(query, new { roomName });
            return count != 0;
        }
    }
}
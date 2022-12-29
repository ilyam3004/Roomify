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

        //ADD ERROR HANDLING
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
}
using ChatApp.Domain.Entities;
using ChatApp.Infrastructure.Config;
using ChatApp.Application.Common.Interfaces.Persistence;
using Dapper;
using System.Data;

namespace ChatApp.Infrastructure.Persistence;

public class MessageRepository : IMessageRepository
{
    private readonly AppDbContext _dbContext;
    
    public MessageRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Message> SaveMessage(Message message)
    {
        var query = "INSERT INTO Message (MessageId, UserId, RoomId, Text, Date, FromUser) " +
                    "VALUES (@MessageId, @UserId, @RoomId, @Text, @Date, @FromUser)";

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

    public Task RemoveMessageById(string messageId)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAllMessagesFromRoom(string roomId)
    {
        throw new NotImplementedException();
    }
}
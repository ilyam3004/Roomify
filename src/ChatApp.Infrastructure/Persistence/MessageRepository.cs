using Dapper;
using System.Data;
using ChatApp.Domain.Entities;
using ChatApp.Infrastructure.Config;
using ChatApp.Application.Common.Interfaces.Persistence;

namespace ChatApp.Infrastructure.Persistence;

public class MessageRepository : IMessageRepository
{
    private readonly AppDbContext _dbContext;
    
    public MessageRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Message> SaveMessage(Message message)
    {
        string query = "INSERT INTO Message (MessageId, UserId, RoomId, Text, Date, FromUser) " +
                    "VALUES (@MessageId, @UserId, @RoomId, @Text, @Date, @FromUser)";

        using var connection = _dbContext.CreateConnection();
        await connection.ExecuteAsync(query, GetInsertionQueryParameters(message));

        return await GetMessageById(message.MessageId);
    }

    public async Task<bool> RemoveMessageById(string messageId)
    {
        string query = "DELETE FROM Message WHERE MessageId = @MessageId";

        using var connection = _dbContext.CreateConnection();
        await connection.ExecuteAsync(query, new { MessageId = messageId });
        
        return !(await MessageExists(messageId));
    }

    public async Task<bool> RemoveAllMessagesFromRoom(string roomId)
    {
        string query = "DELETE FROM Message WHERE RoomId = @RoomId";
        
        using var connection = _dbContext.CreateConnection();
        await connection.ExecuteAsync(query, new { RoomId = roomId });

        return !(await MessagesExistsInRoom(roomId));
    }

    private async Task<bool> MessagesExistsInRoom(string roomId)
    {
        string query = "SELECT COUNT(*) FROM Message WHERE RoomId = @RoomId";

        using var connection = _dbContext.CreateConnection();
        int count = await connection.QueryFirstOrDefaultAsync<int>(query, new { roomId });

        return count != 0;
    }
    
    private async Task<bool> MessageExists(string messageId)
    {
        string query = "SELECT COUNT(*) FROM Message WHERE MessageId = @MessageId";

        using var connection = _dbContext.CreateConnection();
        int count = await connection.QueryFirstOrDefaultAsync<int>(query, new { messageId });
        
        return count != 0;
    }
    
    private async Task<Message> GetMessageById(string messageId)
    {
        string query = "SELECT * FROM Message WHERE MessageId = @MessageId";

        using var connection = _dbContext.CreateConnection();
        var message = await connection.QueryFirstOrDefaultAsync<Message>(query, new { messageId });
        
        return message;
    }
    
    private DynamicParameters GetInsertionQueryParameters(Message message)
    {
        DynamicParameters parameters = new();
        {
            parameters.Add("MessageId", message.MessageId, DbType.String);
            parameters.Add("UserId", message.UserId, DbType.String);
            parameters.Add("RoomId", message.RoomId, DbType.String);
            parameters.Add("Text", message.Text, DbType.String);
            parameters.Add("Date", message.Date, DbType.DateTime);
            parameters.Add("FromUser", message.FromUser, DbType.Boolean);
        }
        
        return parameters;
    }
}
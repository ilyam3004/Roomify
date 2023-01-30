using Dapper;
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
        await connection.ExecuteAsync(query, message);

        return await GetMessageByIdOrNullIfNotExists(message.MessageId);
    }

    public async Task RemoveMessageById(string messageId)
    {
        string query = "DELETE FROM Message WHERE MessageId = @MessageId";

        using var connection = _dbContext.CreateConnection();
        await connection.ExecuteAsync(query, new {MessageId = messageId});
    }

    public async Task RemoveAllMessagesFromRoom(string roomId)
    {
        string query = "DELETE FROM Message WHERE RoomId = @RoomId";
        
        using var connection = _dbContext.CreateConnection();
        await connection.ExecuteAsync(query, new {RoomId = roomId});
    }

    public async Task<List<Message>> GetAllRoomMessages(string roomId)
    {
        string query = "SELECT * FROM Message WHERE RoomId = @RoomId";
        
        using var connection = _dbContext.CreateConnection();
        IEnumerable<Message> messages = await connection
            .QueryAsync<Message>(query, new {RoomId = roomId});
        
        return messages.ToList();
    }

    private async Task<bool> MessageExists(string messageId)
    {
        string query = "SELECT COUNT(*) FROM Message WHERE MessageId = @MessageId";

        using var connection = _dbContext.CreateConnection();
        int count = await connection.QueryFirstOrDefaultAsync<int>(query, new { messageId });
        
        return count != 0;
    }
    
    public async Task<Message> GetMessageByIdOrNullIfNotExists(string messageId)
    {
        string query = "SELECT * FROM Message WHERE MessageId = @MessageId";

        using var connection = _dbContext.CreateConnection();
        var message = await connection.QueryFirstOrDefaultAsync<Message>(query, new {MessageId = messageId});
        
        return message;
    }
}
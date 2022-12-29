using ChatApp.Domain.Entities;
using ChatApp.Infrastructure.Config;
using ChatApp.Application.Common.Interfaces.Persistence;
using Dapper;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using ChatApp.Application.Services;

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
        var query = "INSERT INTO Message (MessageId, UserId, RoomId, Text, Date, FromUser) " +
                    "VALUES (@MessageId, @UserId, @RoomId, @Text, @Date, @FromUser)";

        var parameters = new DynamicParameters();
        parameters.Add("MessageId", message.MessageId, DbType.String);
        parameters.Add("UserId", message.UserId, DbType.String);
        parameters.Add("RoomId", message.RoomId, DbType.String);
        parameters.Add("Text", message.Text, DbType.String);
        parameters.Add("Date", message.Date, DbType.DateTime);
        parameters.Add("FromUser", message.FromUser, DbType.Boolean);

        using (var connection = _dbContext.CreateConnection())
        {
            await connection.ExecuteAsync(query, parameters);
        }

        var dbMessage = await GetMessageById(message.MessageId);

        //ADD ERROR HANDLING
        return dbMessage;
    }

    public async Task RemoveMessageById(string messageId)
    {
        throw new NotImplementedException();
    }

    public async Task<Message> GetMessageById(string messageId)
    {
        var query = "SELECT * FROM Message WHERE MessageId = @MessageId";

        using (var connection = _dbContext.CreateConnection())
        {
            var message = await connection.QueryFirstOrDefaultAsync<Message>(query, new { messageId });
            return message;
        }
    }

    public Task RemoveAllMessagesFromRoom(string roomId)
    {
        throw new NotImplementedException();
    }
}
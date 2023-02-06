using ChatApp.Application.Common.Interfaces.Persistence;
using ChatApp.Infrastructure.Config;
using Microsoft.Extensions.Options;
using ChatApp.Domain.Entities;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Dapper;
using Microsoft.AspNetCore.Http;

namespace ChatApp.Infrastructure.Persistence;

public class MessageRepository : IMessageRepository
{
    private readonly AppDbContext _dbContext;
    private readonly Cloudinary _cloudinary;

    public MessageRepository(IOptions<CloudinarySettings> config, AppDbContext dbContext)
    {
        var account = new Account(
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret);

        _cloudinary = new Cloudinary(account);

        _dbContext = dbContext;
    }

    public async Task<ImageUploadResult?> UploadImageToCloudinary(IFormFile image)
    {
        await using var stream = image.OpenReadStream();
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(image.FileName, stream)
        };
        var uploadResult = _cloudinary.Upload(uploadParams);
        var upload = new ImageUploadResult();
        
        if (uploadResult.Error is not null)
        {
            return null;
        }

        return new ImageUploadResult()
        {
            PublicId = uploadResult.PublicId,
            Url = uploadResult.Url
        };
    }

    public async Task<Message> SaveMessage(Message message)
    {
        string query = "INSERT INTO Message (MessageId, UserId, RoomId, Text, Date, FromUser, isImage, Url) " +
                       "VALUES (@MessageId, @UserId, @RoomId, @Text, @Date, @FromUser, @isImage, @Url)";

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

    public async Task<Message> GetMessageByIdOrNullIfNotExists(string messageId)
    {
        string query = "SELECT * FROM Message WHERE MessageId = @MessageId";

        using var connection = _dbContext.CreateConnection();
        var message = await connection.QueryFirstOrDefaultAsync<Message>(query, new {MessageId = messageId});

        return message;
    }
}
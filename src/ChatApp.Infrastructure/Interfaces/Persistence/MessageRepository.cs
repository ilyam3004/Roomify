using ChatApp.Application.Common.Interfaces.Persistence;
using ChatApp.Infrastructure.Config;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using CloudinaryDotNet.Actions;
using ChatApp.Domain.Entities;
using CloudinaryDotNet;
using System.Data;
using Dapper;

namespace ChatApp.Infrastructure.Interfaces.Persistence;

public class MessageRepository : IMessageRepository
{
    private readonly IDbConnection _connection;
    private readonly Cloudinary _cloudinary;

    public MessageRepository(
        IDbConnection connection,
        IOptions<CloudinarySettings> config)
    {
        var account = new Account(
             config.Value.CloudName,
             config.Value.ApiKey,
             config.Value.ApiSecret);

        _cloudinary = new Cloudinary(account);

        _connection = connection;
    }

    public async Task<ImageUploadResult?> UploadImageToCloudinary(IFormFile image, bool isAvatar)
    {
        await using var stream = image.OpenReadStream();
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(image.FileName, stream)
        };

        if (isAvatar)
        {
            uploadParams.Transformation = new Transformation()
                .Width(200)
                .Height(200)
                .Gravity("faces")
                .Crop("fill");
        }
        
        
        var uploadResult = _cloudinary.Upload(uploadParams);

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
        string query = "INSERT INTO Message (MessageId, UserId, RoomId, Text, Date, FromUser, isImage, ImageUrl) " +
                       "VALUES (@MessageId, @UserId, @RoomId, @Text, @Date, @FromUser, @isImage, @ImageUrl)";

        await _connection.ExecuteAsync(query, message);

        return message;
    }

    public async Task RemoveMessageById(string messageId)
    {
        string query = "DELETE FROM Message WHERE MessageId = @MessageId";

        await _connection.ExecuteAsync(query, new {MessageId = messageId});
    }

    public async Task<List<Message>> GetAllRoomMessages(string roomId)
    {
        string query = "SELECT * FROM Message WHERE RoomId = @RoomId";

        IEnumerable<Message> messages = await _connection
            .QueryAsync<Message>(query, new {RoomId = roomId});

        return messages.ToList();
    }

    public async Task<Message?> GetMessageByIdOrNullIfNotExists(string messageId)
    {
        string query = "SELECT * FROM Message WHERE MessageId = @MessageId";

        var message = await _connection.QueryFirstOrDefaultAsync<Message>(
            query, new {MessageId = messageId});

        return message;
    }
}
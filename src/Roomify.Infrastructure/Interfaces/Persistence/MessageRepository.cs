using System.Data;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Roomify.Application.Common.Interfaces.Persistence;
using Roomify.Domain.Entities;
using Roomify.Infrastructure.Config;
using Roomify.Infrastructure.Queries;

namespace Roomify.Infrastructure.Interfaces.Persistence;

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

    public async Task<ImageUploadResult?> UploadImageToCloudinary(
        IFormFile image, 
        bool isAvatar)
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
        await _connection.ExecuteAsync(
            MessageQueries.SaveMessage, 
            message);

        return message;
    }

    public async Task RemoveMessageById(string messageId)
    {
        await _connection.ExecuteAsync(
            MessageQueries.RemoveMessage, 
            new {MessageId = messageId});
    }

    public async Task<List<Message>> GetAllRoomMessages(string roomId)
    {
        IEnumerable<Message> messages = await _connection
            .QueryAsync<Message>(
                MessageQueries.GetAllRoomMessages, 
                new {RoomId = roomId});

        return messages.ToList();
    }

    public async Task<Message?> GetMessageByIdOrNullIfNotExists(string messageId)
    {
        var message = await _connection.QueryFirstOrDefaultAsync<Message>(
            MessageQueries.GetMessageById, 
            new {MessageId = messageId});

        return message;
    }
}
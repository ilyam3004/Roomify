using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Roomify.Domain.Entities;

namespace Roomify.Application.Common.Interfaces.Persistence;

public interface IMessageRepository
{
    Task<Message> SaveMessage(Message message);
    Task RemoveMessageById(string messageId);
    Task<List<Message>> GetAllRoomMessages(string roomId);
    Task<ImageUploadResult?> UploadImageToCloudinary(IFormFile image, bool isAvatar);
    Task<Message?> GetMessageByIdOrNullIfNotExists(string messageId);
}
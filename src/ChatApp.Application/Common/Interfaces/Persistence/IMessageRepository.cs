using Microsoft.AspNetCore.Http;
using CloudinaryDotNet.Actions;
using ChatApp.Domain.Entities;

namespace ChatApp.Application.Common.Interfaces.Persistence;

public interface IMessageRepository
{
    Task<Message> SaveMessage(Message message);
    Task RemoveMessageById(string messageId);
    Task RemoveAllMessagesFromRoom(string roomId);
    Task<List<Message>> GetAllRoomMessages(string roomId);
    Task<ImageUploadResult?> UploadImageToCloudinary(IFormFile image, bool isAvatar);
    Task<Message?> GetMessageByIdOrNullIfNotExists(string messageId);
}
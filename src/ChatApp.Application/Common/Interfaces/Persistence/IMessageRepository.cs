using ChatApp.Domain.Entities;

namespace ChatApp.Application.Common.Interfaces.Persistence;

public interface IMessageRepository
{
    Task<Message> SaveMessage(Message message);
    Task<bool> RemoveMessageById(string messageId);
    Task RemoveAllMessagesFromRoom(string roomId);
    Task<List<Message>> GetAllRoomMessages(string roomId);
}
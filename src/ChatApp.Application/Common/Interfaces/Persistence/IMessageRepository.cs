using ChatApp.Domain.Entities;

namespace ChatApp.Application.Common.Interfaces.Persistence;

public interface IMessageRepository
{
    Task<Message> SaveMessage(Message message);
    Task<Message> GetMessageById(string messageId);
    Task RemoveMessageById(string messageId);
    Task RemoveAllMessagesFromRoom(string roomId);
}
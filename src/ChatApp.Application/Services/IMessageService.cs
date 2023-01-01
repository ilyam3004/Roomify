using ChatApp.Application.Models.Responses;

namespace ChatApp.Application.Services;
public interface IMessageService
{
    Task<MessageResponse> SaveMessage(string userId, string roomId, string text, DateTime date, bool fromUser);
    Task RemoveMessage(string messageId);
    Task RemoveAllRoomMessages(string roomId);
}
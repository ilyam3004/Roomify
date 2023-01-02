using ChatApp.Application.Models.Responses;
using ErrorOr;

namespace ChatApp.Application.Services;
public interface IMessageService
{
    Task<ErrorOr<MessageResponse>> SaveMessage(string userId, string roomId, string text, DateTime date, bool fromUser);
    Task<ErrorOr<string>> RemoveMessage(string messageId);
    Task<ErrorOr<string>> RemoveAllRoomMessages(string roomId);
}
using ChatApp.Application.Models.Requests;
using ChatApp.Application.Models.Responses;
using ErrorOr;

namespace ChatApp.Application.Services;
public interface IMessageService
{
    Task<ErrorOr<MessageResponse>> SaveMessage(SaveMessageRequest request);
    Task<ErrorOr<string>> RemoveMessage(string messageId);
    Task<ErrorOr<string>> RemoveAllMessagesFromRoom(string roomId);
    Task<ErrorOr<List<MessageResponse>>> GetAllRoomMessages(string roomId);
}
using ChatApp.Application.Models.Requests;
using ChatApp.Application.Models.Responses;
using ErrorOr;

namespace ChatApp.Application.Services;
public interface IMessageService
{
    Task<ErrorOr<MessageResponse>> SaveMessage(SaveMessageRequest request);
    Task<ErrorOr<Deleted>> RemoveMessage(RemoveMessageRequest request, string messageId);
    Task<ErrorOr<List<MessageResponse>>> GetAllRoomMessages(string roomId);
}
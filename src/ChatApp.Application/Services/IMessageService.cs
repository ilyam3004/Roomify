using ChatApp.Application.Models.Requests;
using ChatApp.Application.Models.Responses;
using ErrorOr;

namespace ChatApp.Application.Services;
public interface IMessageService
{
    Task<ErrorOr<MessageResponse>> SaveMessage(SaveMessageRequest request);
    Task<ErrorOr<Deleted>> RemoveMessage(RemoveMessageRequest request);
    Task<ErrorOr<MessageResponse>> SaveImage(SaveImageRequest request);
    Task<List<MessageResponse>> GetAllRoomMessages(string roomId);
}
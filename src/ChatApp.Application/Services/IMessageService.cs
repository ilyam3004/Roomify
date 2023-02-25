using ChatApp.Application.Models.Responses;
using ChatApp.Application.Models.Requests;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using ErrorOr;

namespace ChatApp.Application.Services;
public interface IMessageService
{
    Task<ErrorOr<MessageResponse>> SaveMessage(SaveMessageRequest request);
    Task<ErrorOr<Deleted>> RemoveMessage(RemoveMessageRequest request);
    Task<ErrorOr<MessageResponse>> SaveImage(SaveImageRequest request);
    Task<ErrorOr<ImageUploadResult>> UploadImage(IFormFile image, bool isAvatar);
    Task<List<MessageResponse>> GetAllRoomMessages(string roomId);
}
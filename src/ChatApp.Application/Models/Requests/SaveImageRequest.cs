using Microsoft.AspNetCore.Http;

namespace ChatApp.Application.Models.Requests;

public record SaveImageRequest(
    string UserId,
    string Username,
    string RoomId,
    IFormFile Image,
    bool FromUser);

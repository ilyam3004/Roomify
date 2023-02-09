using ChatApp.Contracts.Rooms.Responses;
using ChatApp.Application.Services;
using Microsoft.AspNetCore.Mvc;
using CloudinaryDotNet.Actions;
using ErrorOr;

namespace ChatApp.Api.Hubs;

[ApiController]
[Route("img")]
public class ImageController : ApiController
{
    private readonly IMessageService _messageService;

    public ImageController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    [HttpPost("uploadImage")]
    public async Task<IActionResult> UploadImage([FromForm]IFormFile image)
    {
        ErrorOr<ImageUploadResult> result = await _messageService.UploadImage(image);

        return result.Match(
            onValue => Ok(MapUploadResponse(onValue)), 
            onError => Problem(onError));
    }

    private static UploadResultResponse MapUploadResponse(ImageUploadResult result)
    {
        return new UploadResultResponse(
            result.PublicId,
            result.Url.ToString());
    }
}
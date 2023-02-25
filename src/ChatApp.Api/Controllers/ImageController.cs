using ChatApp.Contracts.Rooms.Responses;
using ChatApp.Application.Services;
using Microsoft.AspNetCore.Mvc;
using CloudinaryDotNet.Actions;
using ErrorOr;
using MapsterMapper;

namespace ChatApp.Api.Controllers;

[ApiController]
[Route("img")]
public class ImageController : ApiController
{
    private readonly IMessageService _messageService;
    private readonly IMapper _mapper;
    
    public ImageController(IMessageService messageService, IMapper mapper)
    {
        _messageService = messageService;
        _mapper = mapper;
    }

    [HttpPost("uploadImage")]
    public async Task<IActionResult> UploadImage([FromForm]IFormFile image)
    {
        bool isAvatar = false;
        ErrorOr<ImageUploadResult> result = await _messageService.UploadImage(image, isAvatar);

        return result.Match(
            onValue => Ok(_mapper.Map<UploadResultResponse>(onValue)), 
            onError => Problem(onError));
    }

    [HttpPost("uploadAvatar")]
    public async Task<IActionResult> UploadAvatar([FromForm]IFormFile avatar)
    {
        bool isAvatar = true;
        ErrorOr<ImageUploadResult> result = await _messageService.UploadImage(avatar, isAvatar);

        return result.Match(
            onValue => Ok(_mapper.Map<UploadResultResponse>(onValue)),
            onError => Problem(onError));
    }
}
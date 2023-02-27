using ChatApp.Application.Images.Commands.UploadImage;
using ChatApp.Contracts.Rooms.Responses;
using Microsoft.AspNetCore.Mvc;
using CloudinaryDotNet.Actions;
using MapsterMapper;
using MediatR;
using ErrorOr;

namespace ChatApp.Api.Controllers;

[ApiController]
[Route("img")]
public class ImageController : ApiController
{
    private readonly ISender _mediator;
    private readonly IMapper _mapper;

    public ImageController(
        IMapper mapper, 
        ISender mediator)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpPost("uploadImage")]
    public async Task<IActionResult> UploadImage([FromForm]IFormFile image, bool isAvatar)
    {
        var command = new UploadImageCommand(image, isAvatar);
        ErrorOr<ImageUploadResult> result = await _mediator.Send(command);

        return result.Match(
            onValue => Ok(_mapper.Map<UploadResultResponse>(onValue)), 
            onError => Problem(onError));
    }

    [HttpPost("uploadAvatar")]
    public async Task<IActionResult> UploadAvatar([FromForm]IFormFile avatar)
    {
        bool isAvatar = true;
        var command = new UploadImageCommand(avatar, isAvatar);

        ErrorOr<ImageUploadResult> result = await _mediator.Send(command);

        return result.Match(
            onValue => Ok(_mapper.Map<UploadResultResponse>(onValue)),
            onError => Problem(onError));
    }
}
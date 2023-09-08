using CloudinaryDotNet.Actions;
using ErrorOr;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Roomify.Application.Messages.Commands.UploadImage;
using Roomify.Contracts.Rooms.Responses;

namespace Roomify.Api.Controllers;

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
    public async Task<IActionResult> UploadImage([FromForm]IFormFile image, [FromForm]string isAvatar)
    {
        var command = new UploadImageCommand(image, bool.Parse(isAvatar));
        ErrorOr<ImageUploadResult> result = await _mediator.Send(command);

        return result.Match(
            onValue => Ok(_mapper.Map<UploadResultResponse>(onValue)), 
            onError => Problem(onError));
    }
}
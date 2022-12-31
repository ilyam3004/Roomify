using FluentValidation;
using ChatApp.Application.Models.Requests;
using ChatApp.Application.Services;
using ChatApp.Contracts.Rooms;
using Microsoft.AspNetCore.Mvc;
using BadHttpRequestException = Microsoft.AspNetCore.Server.Kestrel.Core.BadHttpRequestException;

namespace ChatApp.Api.Controllers;

[ApiController]
[Route("users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("createUser")]
    public async Task<IActionResult> CreateUser(UserRequest request, [FromServices] IValidator<CreateUserRequest> validator)
    {
        var createUserRequest = new CreateUserRequest(request.Username, request.ConnectionId, request.RoomName);
        var result = await _userService.AddUser(createUserRequest);

        return result.Match<IActionResult>(b =>
        {
            return Ok();
        }, exception =>
        {
            // if (exception is ValidationException validationException)
            // {
            //     return BadRequest(validationException);
            // }

            if (exception is Exception badHttpRequestException)
            {
                return BadRequest(badHttpRequestException);
            }

            return StatusCode(500);
        });
    }
}
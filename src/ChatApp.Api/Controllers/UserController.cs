using ChatApp.Application.Models.Requests;
using ChatApp.Application.Models.Responses;
using ChatApp.Application.Services;
using ChatApp.Contracts.Rooms;
using Microsoft.AspNetCore.Mvc;
using ErrorOr;

namespace ChatApp.Api.Controllers;

[Route("users")]
public class UserController : ApiController
{
    private readonly IUserService _userService;
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("createUser")]
    public async Task<IActionResult> CreateUser(UserRequest request)
    {
        ErrorOr<UserResponse> result = await _userService.AddUserToRoom(
            new CreateUserRequest(
                request.Username, 
                request.ConnectionId, 
                request.RoomName));
        return result.Match(
            result => Ok(result),
            errors => Problem(errors)
        );
    }
}
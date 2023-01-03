using ChatApp.Application.Models.Requests;
using ChatApp.Application.Models.Responses;
using ChatApp.Application.Services;
using ChatApp.Contracts.Rooms;
using Microsoft.AspNetCore.Mvc;
using ErrorOr;

namespace ChatApp.Api.Controllers;

[Route("room")]
public class RoomController : ApiController
{
    private readonly IUserService _userService;
    private readonly IMessageService _messageService;
    public RoomController(IUserService userService, 
        IMessageService messageService)
    {
        _userService = userService;
        _messageService = messageService;
    }

    [HttpPost("addUser")]
    public async Task<IActionResult> AddUser([FromBody]UserRequest request)
    {
        ErrorOr<UserResponse> result = await _userService.AddUserToRoom(
            new CreateUserRequest(
                request.Username, 
                request.ConnectionId, 
                request.RoomName));

        return result.Match(
            result => Ok(result),
        errors => Problem(errors));
    }
    
    [HttpPost("sendMessage")]
    public async Task<IActionResult> SaveMessage([FromBody]MessageRequest request)
    {
        ErrorOr<MessageResponse> saveMessageResult = await _messageService
            .SaveMessage(new SaveMessageRequest(
                request.UserId, 
                request.RoomId, 
                request.Text, 
                request.Date, 
                request.FromUser));

        return saveMessageResult.Match(
            result => Ok(result),
            errors => Problem(errors));
    }
    
    [HttpPost("removeMessage/{messageId}")]
    public async Task<IActionResult> RemoveMessage(string messageId)
    {
        ErrorOr<string> removeMessageResult = await _messageService
            .RemoveMessage(messageId);

        return removeMessageResult.Match(
            result => Ok(result),
            errors => Problem(errors));
    }
}
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

    // [HttpPost("addUser")]
    // public async Task<IActionResult> AddUser([FromBody]JoinUserRequest request)
    // {
    //     ErrorOr<UserResponse> result = await _userService.AddUserToRoom(
    //         new CreateUserRequest(
    //             request.Username, 
    //             request.RoomName, 
    //             request.RoomName));
    //
    //     return result.Match(
    //         result => Ok(result),
    //     errors => Problem(errors));
    // }
    
    [HttpPost("sendMessage")]
    public async Task<IActionResult> SaveMessage([FromBody]SendMessageRequest request)
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
    
    [HttpDelete("removeMessage/{messageId}")]
    public async Task<IActionResult> RemoveMessage(string messageId)
    {
        ErrorOr<string> removeMessageResult = await _messageService
            .RemoveMessage(messageId);

        return removeMessageResult.Match(
            result => Ok(result),
            errors => Problem(errors));
    }

    [HttpDelete("leaveRoom/{userId}")]
    public async Task<IActionResult> LeaveRoom(string userId)
    {
        ErrorOr<string> result = await _userService.RemoveUserFromRoom(userId);

        return result.Match(
            result => Ok(result),
            errors => Problem(errors));
    }
}
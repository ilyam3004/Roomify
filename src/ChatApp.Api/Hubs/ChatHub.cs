using ChatApp.Application.Services;
using ChatApp.Contracts.Rooms;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Api.Hubs;

public class ChatHub : Hub
{
    private readonly IUserService _userService;
    private readonly IMessageService _messageService;
    public ChatHub(IUserService userService, IMessageService messageService)
    {
        _userService = userService;
        _messageService = messageService;
    }

    public async Task JoinRoom(UserRequest request)
    {
        var user = await _userService.AddUser(
            request.Username,
            request.RoomName,
            request.ConnectionId);

        await SendMessageToRoom(new MessageRequest(
            user.UserId,
            user.RoomId,
            $"User {user.Username} has joined the room",
            DateTime.UtcNow,
            false));
    }
    public async Task SendMessageToRoom(MessageRequest request) 
    {
        var message = await _messageService.SaveMessage(
            request.UserId,
            request.RoomId,
            request.Text,
            request.Date,
            request.FromUser);
        
        await Clients.Group(message.RoomName).SendAsync("ReceiveMessage", message, CancellationToken.None);
    }
}
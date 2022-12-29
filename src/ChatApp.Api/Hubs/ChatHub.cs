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
            request.RoomId,
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
        
        await Clients.Groups().SendAsync("ReceiveMessage", message, CancellationToken.None);
        Console.WriteLine($"Message sent to from connection: {Context.ConnectionId}" +
                            $"\nMessage: {text}");
    }

// public async Task JoinRoom()
    // {
    //     Groups.AddToGroupAsync(Context.ConnectionId, connection.Room, CancellationToken.None);
    //     var message = JsonConvert.SerializeObject(new Message
    //     {
    //         Username = C,
    //         Text = $ has joined the room",
    //         ConnectionId = Context.ConnectionId
    //     });
    //
    //     Clients.Group(connection.Room).SendAsync("ReceiveMessage", message, CancellationToken.None);
    //     Console.WriteLine($"-----------------------------------------------------------------");
    //     Console.WriteLine($"Connection {Context.ConnectionId} established");
    //     Console.WriteLine($"User {connection.Username} has joined the room: {connection.Room}");
    //     Console.WriteLine($"-----------------------------------------------------------------");
    //     SendConnectedUsersList(connection.Room);
    // }
    //
    // public override Task OnDisconnectedAsync(Exception? exception)
    // {
    //     if ()
    //     {
    //         _connections.Remove(Context.ConnectionId);
    //
    //         var message = JsonConvert.SerializeObject(new Message
    //         {
    //             Username = _roomBot,
    //             Text = $"User {connection.Username} has left the room",
    //             ConnectionId = Context.ConnectionId
    //         });
    //
    //         Clients.Group(connection.Room)
    //             .SendAsync("ReceiveMessage", message, CancellationToken.None);
    //         Console.WriteLine($"-----------------------------------------------------------------");
    //         Console.WriteLine($"Connection {Context.ConnectionId} closed");
    //         Console.WriteLine($"User {connection.Username} has left the room: {connection.Room}");
    //         Console.WriteLine($"-----------------------------------------------------------------");
    //         SendConnectedUsersList(connection.Room);
    //     }
    //
    //     return base.OnDisconnectedAsync(exception);
    // }
    //
    // public async Task SendMessageToRoom(string text) 
    // { 
    //     Console.WriteLine($"Message received from connection: {Context.ConnectionId}");
    //     if(_connections.TryGetValue(Context.ConnectionId, out UserConnection connection)) 
    //     {
    //         var message = JsonConvert.SerializeObject(new Message
    //         {
    //             ConnectionId = Context.ConnectionId,
    //             Text = text,
    //             Username = connection.Username
    //         });
    //         await Clients.Groups(connection.Room).SendAsync("ReceiveMessage", message, CancellationToken.None);
    //         Console.WriteLine($"Message sent to from connection: {Context.ConnectionId}" +
    //                             $"\nMessage: {text}");
    //     }
    // }
    //
    // public async Task SendImageToRoom(IFormFile file)
    // {
    //     Console.WriteLine($"File {file.FileName} receive from connection: {Context.ConnectionId}");
    //     await Clients.All.SendAsync("ReceiveFile", file);
    // }
    //
    // public Task SendConnectedUsersList(string room) 
    // {
    //     var connectedUsers = _connections.Values
    //         .Where(c => c.Room == room)
    //         .Select(c => c.Username);
    //     var usersInRoom = JsonConvert.SerializeObject(connectedUsers);
    //     return Clients.Group(room).SendAsync("UsersInRoom", usersInRoom);
    // }
    //
    // public async Task SendImage(string json)
    // {
    //     var image = JsonConvert.DeserializeObject<UploadImageResult>(json);
    //     Console.WriteLine($"Image received:{image!.Url}"); 
    // }
}
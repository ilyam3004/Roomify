using ChatApp.Contracts.Rooms;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Api.Hubs;

public class ChatHub : Hub
{
 
     public ChatHub()
     {
        
     }
     
     public async Task JoinRoom(UserRequest request)
     {
         
     }
     
     public override Task OnDisconnectedAsync(Exception? exception)
     {
         return base.OnDisconnectedAsync(exception);
     }
     
     public async Task SendMessageToRoom(MessageRequest messageRequest) 
     {
         await Clients.Group(messageRequest.RoomName).SendAsync("ReceiveMessage", messageRequest.Text, CancellationToken.None);
     }
}
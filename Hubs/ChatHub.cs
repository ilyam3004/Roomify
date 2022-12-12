using ChatAppServer.Models;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace ChatAppServer.Hubs
{
    public class ChatHub : Hub
    {
        private readonly string _roomBot;
        private readonly Dictionary<string, UserConnection> _connections;

        public ChatHub(Dictionary<string, UserConnection> connections)
        {
            _connections = connections;
            _roomBot = "RoomBot";
        }

        public async Task JoinRoom(UserConnection connection)
        {
            _connections[Context.ConnectionId] = connection;
            await Groups.AddToGroupAsync(Context.ConnectionId, connection.Room, CancellationToken.None);

            var message = JsonConvert.SerializeObject(new Message
            {
                Username = _roomBot,
                Text = $"User {connection.Username} has joined the room",
                ConnectionId = Context.ConnectionId
            });

            await Clients.Group(connection.Room).SendAsync("ReceiveMessage", message, CancellationToken.None);
            Console.WriteLine($"-----------------------------------------------------------------");
            Console.WriteLine($"Connection {Context.ConnectionId} established");
            Console.WriteLine($"User {connection.Username} has joined the room: {connection.Room}");
            Console.WriteLine($"-----------------------------------------------------------------");
            await SendConnectedUsersList(connection.Room);
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection connection))
            {
                _connections.Remove(Context.ConnectionId);

                var message = JsonConvert.SerializeObject(new Message
                {
                    Username = _roomBot,
                    Text = $"User {connection.Username} has left the room",
                    ConnectionId = Context.ConnectionId
                });

                Clients.Group(connection.Room)
                    .SendAsync("ReceiveMessage", message, CancellationToken.None);
                Console.WriteLine($"-----------------------------------------------------------------");
                Console.WriteLine($"Connection {Context.ConnectionId} closed");
                Console.WriteLine($"User {connection.Username} has left the room: {connection.Room}");
                Console.WriteLine($"-----------------------------------------------------------------");
                SendConnectedUsersList(connection.Room);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessageToRoom(string text) 
        { 
            if(_connections.TryGetValue(Context.ConnectionId, out UserConnection connection)) 
            {
                var message = JsonConvert.SerializeObject(new Message
                {
                    ConnectionId = Context.ConnectionId,
                    Text = text,
                    Username = connection.Username
                });
                await Clients.Groups(connection.Room).SendAsync("ReceiveMessage", message, CancellationToken.None);
            }
        }

        public Task SendConnectedUsersList(string room) 
        {
            var connectedUsers = _connections.Values
                .Where(c => c.Room == room)
                .Select(c => c.Username);
            var usersInRoom = JsonConvert.SerializeObject(connectedUsers);
            return Clients.Group(room).SendAsync("UsersInRoom", usersInRoom);
        }
    }
}

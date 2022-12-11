using ChatAppServer.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChatAppServer.Hubs
{
    public class ChatHub : Hub
    {
        private readonly string _chatBot;
        private readonly Dictionary<string, UserConnection> _connections;

        public ChatHub(Dictionary<string, UserConnection> connections)
        {
            _connections = connections;
            _chatBot = "RoomBot";
        }

        public async Task JoinRoom(UserConnection connection)
        {
            _connections[Context.ConnectionId] = connection;
            await Groups.AddToGroupAsync(Context.ConnectionId, connection.Room, CancellationToken.None);
            await Clients.Group(connection.Room).SendAsync("ReceiveMessage", _chatBot, $"User {connection.Username} has joined the room {connection.Room}", CancellationToken.None);
            Console.WriteLine($"User {connection.Username} has joined the room: {connection.Room}");
            await SendConnectedUsersList(connection.Room);
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                _connections.Remove(Context.ConnectionId);
                Console.WriteLine($"Connection {Context.ConnectionId} closed");
                Clients.Group(userConnection.Room)
                    .SendAsync("ReceiveMessage",
                    _chatBot,
                    $"User {userConnection.Username} has left the room {userConnection.Room}",
                    CancellationToken.None);
                SendConnectedUsersList(userConnection.Room);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessageToRoom(string message) 
        { 
            if(_connections.TryGetValue(Context.ConnectionId, out UserConnection connection)) 
            {
                await Clients.Groups(connection.Room).SendAsync("ReceiveMessage", connection.Username, message, CancellationToken.None);
            }
        }

        public Task SendConnectedUsersList(string room) 
        {
            var connectedUsers = _connections.Values
                .Where(c => c.Room == room)
                .Select(c => c.Username);

            return Clients.Group(room).SendAsync("UsersInRoom", connectedUsers);
        }

        public void ShowConnections() 
        {
            foreach (KeyValuePair<string, UserConnection> connection in _connections)
            {
                Console.WriteLine($"{connection.Key} -> " +
                                  $"{connection.Value.Username} -> " +
                                  $"{connection.Value.Room}");
            }
        }
    }
}

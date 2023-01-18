namespace ChatApp.Domain.Entities;

public class User
{
    public string UserId { get; set; }
    public string Username { get; set; }
    public string ConnectionId { get; set; }
    public string RoomId { get; set; }
    public bool HasLeft { get; set; }
}
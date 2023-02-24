namespace ChatApp.Domain.Entities;

public class User
{
    public string UserId { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string ConnectionId { get; set; } = null!;
    public string RoomId { get; set; } = null!;
    public bool HasLeft { get; set; }
    public string Avatar { get; set; } = null!;
}
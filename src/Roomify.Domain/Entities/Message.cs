namespace  Roomify.Domain.Entities;

public class Message
{
    public string MessageId { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string RoomId { get; set; } = null!;
    public string Text { get; set; } = null!;
    public DateTime Date { get; set; }
    public bool FromUser { get; set; }
    public bool IsImage { get; set; }
    public string ImageUrl { get; set; } = null!;
}


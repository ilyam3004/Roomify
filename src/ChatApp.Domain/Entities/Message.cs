namespace  ChatApp.Domain.Entities;

public class Message
{
    public string MessageId { get; set; }
    public string UserId { get; set; }
    public string RoomId { get; set; }
    public string Text { get; set; }
    public DateTime Date { get; set; }
    public bool FromUser { get; set; }
    public bool IsImage { get; set; }
    public string ImageUrl { get; set; }
}


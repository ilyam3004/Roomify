namespace ChatApp.Application.Models.Requests;

public record RemoveMessageRequest(
    string MessageId, 
    string ConnectionId);
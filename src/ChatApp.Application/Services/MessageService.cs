using ChatApp.Application.Common.Interfaces.Persistence;
using ChatApp.Application.Models;
using ChatApp.Domain.Entities;

namespace ChatApp.Application.Services;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;
    
    public MessageService(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task<MessageResponse> SaveMessage(string userId, string roomId, string text, DateTime date, bool fromUser)
    {
        var dbMessage = await _messageRepository.SaveMessage(new Message
        {
            MessageId = Guid.NewGuid().ToString(),
            UserId = userId,
            RoomId = roomId,
            Text = text,
            Date = date,
            FromUser = fromUser
        }); 
        
        return new MessageResponse(
            dbMessage.MessageId,
            dbMessage.UserId,
            dbMessage.RoomId,
            dbMessage.Text,
            dbMessage.Date,
            dbMessage.FromUser);
    }

    public async Task RemoveMessage(string messageId)
    {
        throw new NotImplementedException();
    }

    public async Task RemoveAllRoomMessages(string roomId)
    {
        throw new NotImplementedException();
    }
}
using ChatApp.Application.Common.Interfaces.Persistence;
using ChatApp.Application.Models.Responses;
using ChatApp.Domain.Common.Errors;
using ChatApp.Domain.Entities;
using ErrorOr;

namespace ChatApp.Application.Services;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;
    
    public MessageService(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task<ErrorOr<MessageResponse>> SaveMessage(string userId, string roomId, string text, DateTime date, bool fromUser)
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

    public async Task<ErrorOr<string>> RemoveMessage(string messageId)
    {
        bool deleted = await _messageRepository.RemoveMessageById(messageId);
        
        if (deleted)
            return "Message successfully deleted";
        
        return Errors.Message.MessageIsNotRemoved;
    }

    public async Task<ErrorOr<string>> RemoveAllRoomMessages(string roomName)
    {
        bool deleted = await _messageRepository.RemoveAllMessagesFromRoom(roomName);
    }
}
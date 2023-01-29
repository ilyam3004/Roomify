using ErrorOr;
using FluentValidation;
using ChatApp.Domain.Entities;
using FluentValidation.Results;
using ChatApp.Domain.Common.Errors;
using ChatApp.Application.Models.Requests;
using ChatApp.Application.Models.Responses;
using ChatApp.Application.Common.Interfaces.Persistence;

namespace ChatApp.Application.Services;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;
    private readonly IValidator<SaveMessageRequest> _messageValidator;

    public MessageService(IMessageRepository messageRepository, 
        IUserRepository userRepository,
        IValidator<SaveMessageRequest> messageValidator)
    {
        _messageRepository = messageRepository;
        _messageValidator = messageValidator;
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<MessageResponse>> SaveMessage(SaveMessageRequest request)
    {
        var validateResult = await _messageValidator.ValidateAsync(request);
        
        if (validateResult.IsValid)
        {
            if (await _userRepository.UserExists(request.UserId))
            {
                var user = await _userRepository.GetUserById(request.UserId);
                var dbMessage = await _messageRepository.SaveMessage(new Message
                {
                    MessageId = Guid.NewGuid().ToString(),
                    UserId = request.UserId,
                    RoomId = request.RoomId,
                    Text = request.Text,
                    Date = request.Date,
                    FromUser = request.FromUser
                });
                
                return MapMessageResponseResult(dbMessage, user);
            }

            return Errors.User.UserNotFound;
        }
        
        return ConvertValidationErrorToError(validateResult.Errors);
    }
    
    public async Task<ErrorOr<Deleted>> RemoveMessage(RemoveMessageRequest request, string connectionId)
    {
        if (connectionId == request.ConnectionId)
        {
            await _messageRepository.RemoveMessageById(request.MessageId);
            return Result.Deleted;
        }

        return Errors.Message.MessageIsNotRemoved;
    }

    public async Task<ErrorOr<List<MessageResponse>>> GetAllRoomMessages(string roomId)
    {
        if (await _userRepository.RoomExists(roomId))
        {
            List<Message> dbMessages = await _messageRepository.GetAllRoomMessages(roomId);   
            return await MapRoomMessagesResponseResult(dbMessages);
        }

        return Errors.Room.RoomNotFound;
    }

    private List<Error> ConvertValidationErrorToError(List<ValidationFailure> failures)
    {
        return failures.ConvertAll(
            validationFaliure => Error.Validation(
                validationFaliure.PropertyName,
                validationFaliure.ErrorMessage));
    }

    private async Task<List<MessageResponse>> MapRoomMessagesResponseResult(List<Message> dbMessages)
    {
        List<MessageResponse> messages = new();
        foreach (var dbMessage in dbMessages)
        {
            var user = await _userRepository.GetUserById(dbMessage.UserId);
            messages.Add(MapMessageResponseResult(dbMessage, user));
        }

        return messages;
    }

    private MessageResponse MapMessageResponseResult(Message message, User user)
    {
        return new MessageResponse(
                message.MessageId,
                user.Username,
                user.UserId,
                message.RoomId,
                message.Text,
                message.Date,
                message.FromUser);
    }
}
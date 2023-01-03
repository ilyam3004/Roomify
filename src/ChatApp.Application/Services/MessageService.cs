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
    private readonly IValidator<SaveMessageRequest> _messageValidator;

    public MessageService(IMessageRepository messageRepository,
        IValidator<SaveMessageRequest> messageValidator)
    {
        _messageRepository = messageRepository;
        _messageValidator = messageValidator;
    }

    public async Task<ErrorOr<MessageResponse>> SaveMessage(SaveMessageRequest request)
    {
        var validateResult = await _messageValidator.ValidateAsync(request);
        if (validateResult.IsValid)
        {
            var dbMessage = await _messageRepository.SaveMessage(new Message
            {
                MessageId = Guid.NewGuid().ToString(),
                UserId = request.UserId,
                RoomId = request.RoomId,
                Text = request.Text,
                Date = request.Date,
                FromUser = request.FromUser
            });

            return MapMessageResponseResult(dbMessage);
        }
        
        return ConvertValidationErrorToError(validateResult.Errors);
    }
    
    public async Task<ErrorOr<string>> RemoveMessage(string messageId)
    {
        bool deleted = await _messageRepository.RemoveMessageById(messageId);
        
        return deleted ? "Message successfully deleted" : Errors.Message.MessagesIsNotRemoved;
    }

    public async Task<ErrorOr<string>> RemoveAllMessagesFromRoom(string roomId)
    {
        bool deleted = await _messageRepository.RemoveAllMessagesFromRoom(roomId);
        return deleted ? "Chat successfully deleted" : Errors.Message.MessagesIsNotRemoved;
    }

    private List<Error> ConvertValidationErrorToError(List<ValidationFailure> failures)
    {
        return failures.ConvertAll(
            validationFaliure => Error.Validation(
                validationFaliure.PropertyName,
                validationFaliure.ErrorMessage));
    }

    private MessageResponse MapMessageResponseResult(Message message)
    {
        return new MessageResponse(
                message.MessageId,
                message.UserId,
                message.RoomId,
                message.Text,
                message.Date,
                message.FromUser);
    }
}
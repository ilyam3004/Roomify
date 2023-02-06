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
                
                return MapMessageResponseResult(dbMessage, user.Username, user.UserId);
            }

            return Errors.User.UserNotFound;
        }
        
        return ConvertValidationErrorToError(validateResult.Errors);
    }
    
    public async Task<ErrorOr<Deleted>> RemoveMessage(RemoveMessageRequest request)
    {
        var message = await _messageRepository.GetMessageByIdOrNullIfNotExists(request.MessageId);
        if (message is null)
        {
            return Errors.Message.MessageNotFound;
        }

        var user = await _userRepository.GetUserByConnectionIdOrNull(request.ConnectionId);
        if (user is null)
        {
            return Errors.User.UserNotFound;
        }

        if (user.UserId == message.UserId)
        {
            await _messageRepository.RemoveMessageById(message.MessageId);
            return Result.Deleted;
        }

        return Errors.Message.MessageIsNotRemoved;
    }

    public async Task<ErrorOr<MessageResponse>> SaveImage(SaveImageRequest request)
    {
        if (request.Image.Length <= 0)
        {
            return Errors.Message.ImageFileIsCorrupted;
        }

        var uploadResult = await _messageRepository.UploadImageToCloudinary(request.Image);

        if (uploadResult is null)
        {
            return Errors.Message.CantUploadImage;
        }

        var dbMessage = await _messageRepository.SaveMessage(new Message
        {
            MessageId = Guid.NewGuid().ToString(),
            RoomId = request.RoomId,
            UserId = request.UserId,
            Text = "",
            Date = DateTime.UtcNow,
            FromUser = true,
            IsImage = true,
            ImageUrl = uploadResult.Url.ToString()
        });

        return MapMessageResponseResult(dbMessage, request.Username, request.UserId);
    }

    public async Task<List<MessageResponse>> GetAllRoomMessages(string roomId)
    {
        List<Message> dbMessages = await _messageRepository.GetAllRoomMessages(roomId);   
        
        return await MapRoomMessagesResponseResult(dbMessages);
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
            messages.Add(MapMessageResponseResult(dbMessage, user.Username, user.UserId));
        }

        return messages;
    }

    private MessageResponse MapMessageResponseResult(Message message, string senderName, string senderId)
    {
        return new MessageResponse(
                message.MessageId,
                senderName,
                senderId,
                message.RoomId,
                message.Text,
                message.Date,
                message.FromUser,
                message.IsImage,
                message.ImageUrl);
    }
}
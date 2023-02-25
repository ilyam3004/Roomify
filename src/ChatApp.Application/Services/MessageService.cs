using ChatApp.Application.Common.Interfaces.Persistence;
using ChatApp.Application.Models.Responses;
using ChatApp.Application.Models.Requests;
using ChatApp.Domain.Common.Errors;
using FluentValidation.Results;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using ChatApp.Domain.Entities;
using Error = ErrorOr.Error;
using FluentValidation;
using MapsterMapper;
using ErrorOr;

namespace ChatApp.Application.Services;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;
    private readonly IValidator<SaveMessageRequest> _textMessageValidator;
    private readonly IValidator<SaveImageRequest> _imageMessageValidator;
    private readonly IMapper _mapper;

    public MessageService(IMessageRepository messageRepository, 
        IUserRepository userRepository,
        IValidator<SaveMessageRequest> textMessageValidator,
        IValidator<SaveImageRequest> imageMessageValidator,
        IMapper mapper)
    {
        _messageRepository = messageRepository;
        _textMessageValidator = textMessageValidator;
        _userRepository = userRepository;
        _imageMessageValidator = imageMessageValidator;
        _mapper = mapper;
    }

    public async Task<ErrorOr<MessageResponse>> SaveMessage(SaveMessageRequest request)
    {
        if (!await _userRepository.UserExists(request.UserId))
        {
            return Errors.User.UserNotFound;
        }
        
        var validateResult = await _textMessageValidator.ValidateAsync(request);

        if (!validateResult.IsValid)
        {
            return ConvertValidationErrorToError(validateResult.Errors);
        }
        
        var dbMessage = await _messageRepository.SaveMessage(new Message
        {
            MessageId = Guid.NewGuid().ToString(),
            UserId = request.UserId,
            RoomId = request.RoomId,
            Text = request.Text,
            Date = DateTime.UtcNow,
            FromUser = request.FromUser,
            IsImage = false,
            ImageUrl = ""
        });

        var user = await _userRepository.GetUserById(dbMessage.UserId);

        return _mapper.Map<MessageResponse>((dbMessage, user));
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
        if (!await _userRepository.UserExists(request.UserId))
        {
            return Errors.User.UserNotFound;
        }
        
        var validateResult = await _imageMessageValidator.ValidateAsync(request);

        if (!validateResult.IsValid)
        {
            return ConvertValidationErrorToError(validateResult.Errors);
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
            ImageUrl = request.ImageUrl
        });

        var user = await _userRepository.GetUserById(dbMessage.UserId);
            
        return _mapper.Map<MessageResponse>((dbMessage, user));

    }

    public async Task<ErrorOr<ImageUploadResult>> UploadImage(IFormFile image)
    {
        if (image.Length <= 0)
        {
            return Errors.Message.ImageFileIsCorrupted;
        }

        var uploadResult = await _messageRepository.UploadImageToCloudinary(image);

        return uploadResult is null ? Errors.Message.CantUploadImage : uploadResult;
    }

    public async Task<List<MessageResponse>> GetAllRoomMessages(string roomId)
    {
        List<Message> dbMessages = await _messageRepository.GetAllRoomMessages(roomId);   
        
        return await MapRoomMessagesResponseResult(dbMessages);
    }

    private async Task<List<MessageResponse>> MapRoomMessagesResponseResult(List<Message> dbMessages)
    {
        List<MessageResponse> messages = new();
        foreach (var dbMessage in dbMessages)
        {
            var user = await _userRepository.GetUserById(dbMessage.UserId);
            messages.Add(_mapper.Map<MessageResponse>((dbMessage, user)));
        }

        return messages;
    }

    private static List<Error> ConvertValidationErrorToError(List<ValidationFailure> failures)
    {
        return failures.ConvertAll(
            validationFaliure => Error.Validation(
                validationFaliure.PropertyName,
                validationFaliure.ErrorMessage));
    }
}
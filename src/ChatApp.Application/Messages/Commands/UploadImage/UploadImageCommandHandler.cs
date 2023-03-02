using ChatApp.Application.Common.Interfaces.Persistence;
using ChatApp.Domain.Common.Errors;
using CloudinaryDotNet.Actions;
using ErrorOr;
using MediatR;

namespace ChatApp.Application.Messages.Commands.UploadImage;

public class UploadImageCommandHandler : 
    IRequestHandler<UploadImageCommand, ErrorOr<ImageUploadResult>>
{
    private readonly IMessageRepository _messageRepository;

    public UploadImageCommandHandler(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task<ErrorOr<ImageUploadResult>> Handle(
        UploadImageCommand command, 
        CancellationToken cancellationToken)
    {
        if (command.image.Length <= 0)
        {
            return Errors.Message.ImageFileIsCorrupted;
        }

        var uploadResult = await _messageRepository.UploadImageToCloudinary(command.image, command.isAvatar);

        return uploadResult is null ? Errors.Message.CantUploadImage : uploadResult;
    }
}
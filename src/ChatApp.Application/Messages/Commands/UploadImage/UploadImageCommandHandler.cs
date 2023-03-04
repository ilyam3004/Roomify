using ChatApp.Application.Common.Interfaces;
using ChatApp.Domain.Common.Errors;
using CloudinaryDotNet.Actions;
using ErrorOr;
using MediatR;

namespace ChatApp.Application.Messages.Commands.UploadImage;

public class UploadImageCommandHandler : 
    IRequestHandler<UploadImageCommand, ErrorOr<ImageUploadResult>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UploadImageCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<ImageUploadResult>> Handle(
        UploadImageCommand command, 
        CancellationToken cancellationToken)
    {
        if (command.image.Length <= 0)
        {
            return Errors.Message.ImageFileIsCorrupted;
        }

        var uploadResult = await _unitOfWork.Messages
            .UploadImageToCloudinary(command.image, command.isAvatar);

        return uploadResult is null ? Errors.Message.CantUploadImage : uploadResult;
    }
}
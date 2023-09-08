using CloudinaryDotNet.Actions;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Roomify.Application.Messages.Commands.UploadImage;

public record UploadImageCommand(
    IFormFile image,
    bool isAvatar): IRequest<ErrorOr<ImageUploadResult>>;
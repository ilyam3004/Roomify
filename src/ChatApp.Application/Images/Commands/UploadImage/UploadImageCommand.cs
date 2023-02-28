using Microsoft.AspNetCore.Http;
using CloudinaryDotNet.Actions;
using ErrorOr;
using MediatR;

namespace ChatApp.Application.Images.Commands.UploadImage;

public record UploadImageCommand(
    IFormFile image,
    bool isAvatar): IRequest<ErrorOr<ImageUploadResult>>;
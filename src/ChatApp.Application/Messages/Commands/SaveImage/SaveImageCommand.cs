using ChatApp.Application.Models.Responses;
using ErrorOr;
using MediatR;

namespace ChatApp.Application.Messages.Commands.SaveImage;

public record SaveImageCommand(
    string UserId,
    string RoomId,
    string ImageUrl
) : IRequest<ErrorOr<MessageResponse>>;
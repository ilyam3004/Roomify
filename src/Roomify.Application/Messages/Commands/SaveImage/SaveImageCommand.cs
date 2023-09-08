using ErrorOr;
using MediatR;
using Roomify.Application.Models.Responses;

namespace Roomify.Application.Messages.Commands.SaveImage;

public record SaveImageCommand(
    string UserId,
    string RoomId,
    string ImageUrl
) : IRequest<ErrorOr<MessageResponse>>;
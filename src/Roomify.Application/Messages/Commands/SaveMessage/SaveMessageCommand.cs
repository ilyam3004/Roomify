using ErrorOr;
using MediatR;
using Roomify.Application.Models.Responses;

namespace Roomify.Application.Messages.Commands.SaveMessage;

public record SaveMessageCommand(
    string UserId,
    string Username,
    string RoomId,
    string Text,
    bool FromUser
) : IRequest<ErrorOr<MessageResponse>>;
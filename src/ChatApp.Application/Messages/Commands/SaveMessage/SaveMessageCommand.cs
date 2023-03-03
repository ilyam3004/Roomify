using ChatApp.Application.Models.Responses;
using ErrorOr;
using MediatR;

namespace ChatApp.Application.Messages.Commands.SaveMessage;

public record SaveMessageCommand(
    string UserId,
    string Username,
    string RoomId,
    string Text,
    bool FromUser
) : IRequest<ErrorOr<MessageResponse>>;
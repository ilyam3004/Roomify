using MediatR;
using Roomify.Application.Models.Responses;

namespace Roomify.Application.Messages.Queries.GetRoomMessages;

public record GetRoomMessagesQuery(
    string RoomId) : IRequest<List<MessageResponse>>;
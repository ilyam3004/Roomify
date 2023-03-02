using ChatApp.Application.Models.Responses;
using MediatR;

namespace ChatApp.Application.Messages.Queries.GetRoomMessages;

public record GetRoomMessagesQuery(
    string RoomId) : IRequest<List<MessageResponse>>;
using MapsterMapper;
using MediatR;
using Roomify.Application.Common.Interfaces;
using Roomify.Application.Models.Responses;
using Roomify.Domain.Entities;

namespace Roomify.Application.Messages.Queries.GetRoomMessages;

public class GetRoomMessagesQueryHandler : 
    IRequestHandler<GetRoomMessagesQuery, List<MessageResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetRoomMessagesQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<MessageResponse>> Handle(
        GetRoomMessagesQuery query, 
        CancellationToken cancellationToken)
    {
        List<Message> dbMessages = await _unitOfWork.Messages
            .GetAllRoomMessages(query.RoomId);   
        
        return await MapRoomMessagesResponseResult(dbMessages);
    }

    private async Task<List<MessageResponse>> MapRoomMessagesResponseResult(
        List<Message> dbMessages)
    {
        List<MessageResponse> messages = new();
        foreach (var dbMessage in dbMessages)
        {
            var user = await _unitOfWork.Users
                .GetUserById(dbMessage.UserId);
            messages.Add(_mapper.Map<MessageResponse>((dbMessage, user)));
        }

        return messages;
    }
}
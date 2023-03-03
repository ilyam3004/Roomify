using ChatApp.Application.Common.Interfaces.Persistence;
using ChatApp.Application.Models.Responses;
using ChatApp.Domain.Entities;
using MapsterMapper;
using MediatR;

namespace ChatApp.Application.Messages.Queries.GetRoomMessages;

public class GetRoomMessagesQueryHandler : 
    IRequestHandler<GetRoomMessagesQuery, List<MessageResponse>>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetRoomMessagesQueryHandler(
        IMessageRepository messageRepository,
        IUserRepository userRepository,
        IMapper mapper)
    {
        _messageRepository = messageRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<List<MessageResponse>> Handle(
        GetRoomMessagesQuery query, 
        CancellationToken cancellationToken)
    {
        List<Message> dbMessages = await _messageRepository.GetAllRoomMessages(query.RoomId);   
        
        return await MapRoomMessagesResponseResult(dbMessages);
    }

    private async Task<List<MessageResponse>> MapRoomMessagesResponseResult(
        List<Message> dbMessages)
    {
        List<MessageResponse> messages = new();
        foreach (var dbMessage in dbMessages)
        {
            var user = await _userRepository.GetUserById(dbMessage.UserId);
            messages.Add(_mapper.Map<MessageResponse>((dbMessage, user)));
        }

        return messages;
    }
}
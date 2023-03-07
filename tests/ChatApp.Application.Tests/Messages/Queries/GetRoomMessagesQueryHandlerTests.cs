using ChatApp.Application.Messages.Queries.GetRoomMessages;
using ChatApp.Application.Common.Interfaces;
using ChatApp.Application.Tests.Config;
using ChatApp.Domain.Entities;
using MapsterMapper;
using AutoFixture;
using Moq;

namespace ChatApp.Application.Tests.Messages.Queries;

public class GetRoomMessagesQueryHandlerTests
{ 
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly IMapper _mapper = MapsterConfigForTesting.GetMapper();
    private readonly Fixture _fixture;
    private readonly GetRoomMessagesQueryHandler _sut;

    public GetRoomMessagesQueryHandlerTests()
    {   
        _fixture = new Fixture();
        _sut = new GetRoomMessagesQueryHandler(
            _unitOfWorkMock.Object,
            _mapper);
    }

    [Fact]
    public async Task Handler_ShouldReturnListOfMessageResponse()
    {
        //Arrange
        var user = _fixture.Create<User>();

        var query = new GetRoomMessagesQuery(user.RoomId);

        _unitOfWorkMock
            .Setup(x => 
                x.Users.GetUserById(user.UserId))
            .ReturnsAsync(user);

        var messageList = _fixture.Build<Message>()
            .With(m => m.UserId, user.UserId)
            .With(m => m.RoomId, user.RoomId)
            .CreateMany(2)
            .ToList();

        _unitOfWorkMock
            .Setup(u => 
                u.Messages.GetAllRoomMessages(user.RoomId))
            .ReturnsAsync(messageList);
        
        //Act
        var messageResponse = await _sut.Handle(query, CancellationToken.None);

        //Assert
        Assert.Equal(messageResponse.Count, messageList.Count);
        Assert.Equal(messageResponse[0].UserId, messageList[1].UserId);
        Assert.Equal(messageResponse[0].MessageId, messageList[0].MessageId);
    }
}
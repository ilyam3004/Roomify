using ChatApp.Application.Common.Interfaces.Persistence;
using ChatApp.Application.Tests.Config;
using ChatApp.Domain.Common.Errors;
using ChatApp.Domain.Entities;
using MapsterMapper;
using AutoFixture;
using ErrorOr;
using Moq;
using ChatApp.Application.Messages.Commands.RemoveMessage;

namespace ChatApp.Application.Tests.Messages.Commands;

public class RemoveMessageCommandHandlerTests
{
    private readonly Mock<IMessageRepository> _messageRepositoryMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly IMapper _mapper = MapsterConfigForTesting.GetMapper();
    private readonly Fixture _fixture;
    private readonly RemoveMessageCommandHandler _sut;

    public RemoveMessageCommandHandlerTests()
    {
        _sut = new RemoveMessageCommandHandler(
            _messageRepositoryMock.Object,
            _userRepositoryMock.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task Handler_ShouldReturnDeleted() 
    {
        //Arrange
        var user = _fixture.Create<User>();

        _userRepositoryMock
            .Setup(x => x.GetUserByConnectionIdOrNull(user.ConnectionId))
            .ReturnsAsync(user);

        var message = _fixture.Build<Message>()
            .With(m => m.UserId, user.UserId)
            .With(m => m.RoomId, user.RoomId)
            .Create();

        _messageRepositoryMock
            .Setup(x => x.GetMessageByIdOrNullIfNotExists(message.MessageId))
            .ReturnsAsync(message);

        var command = new RemoveMessageCommand(
            message.MessageId, 
            user.ConnectionId);

        //Act
        var response = await _sut.Handle(command, CancellationToken.None);
        
        //Assert 
        Assert.Equal(response.Value, Result.Deleted);
    }

    [Fact]
    public async Task Handler_ShouldReturnError_WhenUserAreNotOwnerOfMessage()
    {
        //Arrange
        var user = _fixture.Create<User>();

        _userRepositoryMock
            .Setup(x => x.GetUserByConnectionIdOrNull(user.ConnectionId))
            .ReturnsAsync(user);

        var message = _fixture.Build<Message>()
            .With(m => m.RoomId, user.RoomId)
            .Create();

        _messageRepositoryMock
            .Setup(x => x.GetMessageByIdOrNullIfNotExists(message.MessageId))
            .ReturnsAsync(message);

        var command = new RemoveMessageCommand(message.MessageId, user.ConnectionId);

        //Act
        var response = await _sut.Handle(command, CancellationToken.None);
        
        //Assert 
        Assert.Equal(Errors.Message.MessageIsNotRemoved, response.FirstError);
    }

    [Fact]
    public async Task Handler_ShouldReturnError_WhenMessageNotExists()
    {
        //Arrange
        var command = _fixture.Create<RemoveMessageCommand>();

        _messageRepositoryMock
            .Setup(x => x.GetMessageByIdOrNullIfNotExists(command.MessageId))
            .ReturnsAsync(() => null);

        //Act
        var response = await _sut.Handle(command, CancellationToken.None);
        
        //Assert 
        Assert.Equal(Errors.Message.MessageNotFound, response.FirstError);
    }

    [Fact]
    public async Task Handler_ShouldReturnError_WhenUserNotExists()
    {
        //Arrange
        var connectionId = Guid.NewGuid().ToString();

        var message = _fixture.Create<Message>();

        var command = new RemoveMessageCommand(message.MessageId, connectionId);

        _messageRepositoryMock
            .Setup(x => x.GetMessageByIdOrNullIfNotExists(message.MessageId))
            .ReturnsAsync(message);

        _userRepositoryMock
            .Setup(x => x.GetUserByConnectionIdOrNull(command.ConnectionId))
            .ReturnsAsync(() => null);
        
        //Act
        var response = await _sut.Handle(command, CancellationToken.None);
        
        //Assert 
        Assert.Equal(Errors.User.UserNotFound, response.FirstError);
    }

}
using AutoFixture;
using ErrorOr;
using Moq;
using Roomify.Application.Common.Interfaces;
using Roomify.Application.Messages.Commands.RemoveMessage;
using Roomify.Domain.Common.Errors;
using Roomify.Domain.Entities;

namespace Roomify.Application.Tests.Messages.Commands;

public class RemoveMessageCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Fixture _fixture;
    private readonly RemoveMessageCommandHandler _sut;

    public RemoveMessageCommandHandlerTests()
    {
        _sut = new RemoveMessageCommandHandler(
            _unitOfWorkMock.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task Handler_ShouldReturnDeleted() 
    {
        //Arrange
        var user = _fixture.Create<User>();

        _unitOfWorkMock
            .Setup(u => 
                u.Users.GetUserByConnectionIdOrNull(user.ConnectionId))
            .ReturnsAsync(user);

        var message = _fixture.Build<Message>()
            .With(m => m.UserId, user.UserId)
            .With(m => m.RoomId, user.RoomId)
            .Create();

        _unitOfWorkMock
            .Setup(u => 
                u.Messages.GetMessageByIdOrNullIfNotExists(message.MessageId))
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

        _unitOfWorkMock
            .Setup(u => 
                u.Users.GetUserByConnectionIdOrNull(user.ConnectionId))
            .ReturnsAsync(user);

        var message = _fixture.Build<Message>()
            .With(m => m.RoomId, user.RoomId)
            .Create();

        _unitOfWorkMock
            .Setup(u => 
                u.Messages.GetMessageByIdOrNullIfNotExists(message.MessageId))
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

        _unitOfWorkMock
            .Setup(u => 
                u.Messages.GetMessageByIdOrNullIfNotExists(command.MessageId))
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

        _unitOfWorkMock
            .Setup(u => 
                u.Messages.GetMessageByIdOrNullIfNotExists(message.MessageId))
            .ReturnsAsync(message);

        _unitOfWorkMock
            .Setup(u => 
                u.Users.GetUserByConnectionIdOrNull(command.ConnectionId))
            .ReturnsAsync(() => null);
        
        //Act
        var response = await _sut.Handle(command, CancellationToken.None);
        
        //Assert 
        Assert.Equal(Errors.User.UserNotFound, response.FirstError);
    }

}
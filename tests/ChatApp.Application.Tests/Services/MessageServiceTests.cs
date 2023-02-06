using ChatApp.Application.Common.Interfaces.Persistence;
using ChatApp.Application.Common.Validations;
using ChatApp.Application.Models.Requests;
using ChatApp.Application.Services;
using ChatApp.Domain.Common.Errors;
using ChatApp.Domain.Entities;
using FluentValidation;
using AutoFixture;
using ErrorOr;
using Moq;

namespace ChatApp.Application.Tests.Services;

public class MessageServiceTests
{
    private readonly MessageService _sut;
    private readonly Fixture _fixture;
    private readonly Mock<IMessageRepository> _messageRepositoryMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly IValidator<SaveMessageRequest> _messageValidator = new SaveMessageRequestValidator();

    public MessageServiceTests()
    {
        _sut = new MessageService(_messageRepositoryMock.Object, 
            _userRepositoryMock.Object, 
            _messageValidator);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task SaveMessage_ShouldReturnMessageResponse()
    {
        //Arrange
        var request = _fixture.Create<SaveMessageRequest>();
        var user = _fixture.Build<User>()
            .With(u => u.UserId, request.UserId)
            .Create();

        _userRepositoryMock
            .Setup(x => x.UserExists(user.UserId))
            .ReturnsAsync(true);

        _userRepositoryMock
            .Setup(x => x.GetUserById(user.UserId))
            .ReturnsAsync(user);

        _messageRepositoryMock
            .Setup(x => x.SaveMessage(It.IsAny<Message>()))
            .ReturnsAsync((Message message) => message);
        
        //Act
        var messageResponse = await _sut.SaveMessage(request);

        //Assert
        Assert.Equal(messageResponse.Value.Username, user.Username);
        Assert.Equal(messageResponse.Value.Date, request.Date);
    }
    
    [Fact]
    public async Task SaveMessage_ShouldReturnError_WhenUserNotExists()
    {
        //Arrange
        var request = _fixture.Create<SaveMessageRequest>();

        _userRepositoryMock
            .Setup(x => x.UserExists(request.UserId))
            .ReturnsAsync(false);
        
        //Act
        var messageResponse = await _sut.SaveMessage(request);

        //Assert
        Assert.Equal(Errors.User.UserNotFound, messageResponse.FirstError);
    }

    [Fact]
    public async Task SaveMessageRequest_ShouldReturnError_WhenRequestIsNotValid()
    {
        //Arrange
        var request = new SaveMessageRequest(
            "",
            "",
            "",
            "",
            DateTime.UtcNow,
            true);

        //Act
        var messageResponse = await _sut.SaveMessage(request);

        //Assert
        Assert.Equal(Error.Validation().Type, messageResponse.FirstError.Type);
    }

    [Fact]
    public async Task GetAllRoomMessages_ShouldReturnListOfMessageResponse()
    {
        //Arrange
        var user = _fixture.Create<User>();

        _userRepositoryMock
            .Setup(x => x.GetUserById(user.UserId))
            .ReturnsAsync(user);

        _userRepositoryMock
            .Setup(x => x.UserExists(user.UserId))
            .ReturnsAsync(true);

        _userRepositoryMock
            .Setup(x => x.RoomExists(user.RoomId))
            .ReturnsAsync(true);

        var messageList = _fixture.Build<Message>()
            .With(m => m.UserId, user.UserId)
            .With(m => m.RoomId, user.RoomId)
            .CreateMany(2)
            .ToList();

        _messageRepositoryMock
            .Setup(x => x.GetAllRoomMessages(user.RoomId))
            .ReturnsAsync(messageList);
        
        //Act
        var messageResponse = await _sut.GetAllRoomMessages(user.RoomId);

        //Assert
        Assert.Equal(messageResponse.Count, messageList.Count);
        Assert.Equal(messageResponse[0].UserId, messageList[1].UserId);
        Assert.Equal(messageResponse[0].MessageId, messageList[0].MessageId);
    }
    
    [Fact]
    public async Task RemoveMessage_ShouldReturnDeleted() 
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

        var request = new RemoveMessageRequest(message.MessageId, user.ConnectionId);

        //Act
        var response = await _sut.RemoveMessage(request);
        
        //Assert 
        Assert.Equal(response.Value, Result.Deleted);
    }

    [Fact]
    public async Task RemoveMessage_ShouldReturnError_WhenUserAreNotOwnerOfMessage()
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

        var request = new RemoveMessageRequest(message.MessageId, user.ConnectionId);

        //Act
        var response = await _sut.RemoveMessage(request);
        
        //Assert 
        Assert.Equal(Errors.Message.MessageIsNotRemoved, response.FirstError);
    }

    [Fact]
    public async Task RemoveMessage_ShouldReturnError_WhenMessageNotExists()
    {
        //Arrange
        var request = _fixture.Create<RemoveMessageRequest>();

        _messageRepositoryMock
            .Setup(x => x.GetMessageByIdOrNullIfNotExists(request.MessageId))
            .ReturnsAsync(() => null);

        //Act
        var response = await _sut.RemoveMessage(request);
        
        //Assert 
        Assert.Equal(Errors.Message.MessageNotFound, response.FirstError);
    }

    [Fact]
    public async Task RemoveMessage_ShouldReturnError_WhenUserNotExists()
    {
        //Arrange
        var connectionId = Guid.NewGuid().ToString();

        var message = _fixture.Create<Message>();

        var request = new RemoveMessageRequest(message.MessageId, connectionId);

        _messageRepositoryMock
            .Setup(x => x.GetMessageByIdOrNullIfNotExists(message.MessageId))
            .ReturnsAsync(message);

        _userRepositoryMock
            .Setup(x => x.GetUserByConnectionIdOrNull(request.ConnectionId))
            .ReturnsAsync(() => null);
        
        //Act
        var response = await _sut.RemoveMessage(request);
        
        //Assert 
        Assert.Equal(Errors.User.UserNotFound, response.FirstError);
    }

    // [Fact]
    // public async Task SaveImage_ShouldReturnMessageResponse() 
    // {
    //     
    //     // public record MessageResponse(
    //     //     string MessageId,
    //     //     string Username,
    //     //     string UserId,
    //     //     string RoomId,
    //     //     string Text,
    //     //     DateTime Date,
    //     //     bool FromUser,
    //     //     bool IsImage,
    //     //     string Url);
    //
    //     // Arrange
    //     var messageResponse = new MessageResponse(
    //         Guid.NewGuid().ToString(),
    //         "Username",
    //         Guid.NewGuid().ToString(),
    //         Guid.NewGuid().ToString(),
    //         "",
    //         DateTime.UtcNow,
    //         true,
    //         true,
    //         "imageUrl");        
    //
    //     // Act
    //     _sut.SaveImage();
    //
    //     // Assert
    //     Assert.Equal();
    // }
}
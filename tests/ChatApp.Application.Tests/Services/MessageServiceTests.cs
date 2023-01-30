using System.Security.Principal;
using ChatApp.Application.Common.Interfaces.Persistence;
using ChatApp.Application.Common.Validations;
using ChatApp.Application.Models.Requests;
using ChatApp.Application.Services;
using ChatApp.Domain.Common.Errors;
using ChatApp.Domain.Entities;
using ErrorOr;
using FluentValidation;
using Moq;

namespace ChatApp.Application.Tests.Services;

public class MessageServiceTests
{
    private readonly MessageService _messageService;
    private readonly Mock<IMessageRepository> _messageRepositoryMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly IValidator<SaveMessageRequest> _messageValidator = new SaveMessageRequestValidator();

    public MessageServiceTests()
    {
        _messageService = new MessageService(_messageRepositoryMock.Object, 
            _userRepositoryMock.Object, 
            _messageValidator);
    }

    [Fact]
    public async Task SaveMessage_ShouldReturnMessageResponse()
    {
        //Arrange
        var userId = Guid.NewGuid().ToString();
        var request = new SaveMessageRequest(
            userId, 
            Guid.NewGuid().ToString(),
            "Message text",
            DateTime.UtcNow, 
            true);

        _userRepositoryMock
            .Setup(x => x.UserExists(userId))
            .ReturnsAsync(true);

        var user = new User
        {
            UserId = userId,
            Username = "username",
            ConnectionId = Guid.NewGuid().ToString(),
            HasLeft = false,
            RoomId = Guid.NewGuid().ToString()
        };

        _userRepositoryMock
            .Setup(x => x.GetUserById(userId))
            .ReturnsAsync(user);

        _messageRepositoryMock
            .Setup(x => x.SaveMessage(It.IsAny<Message>()))
            .ReturnsAsync((Message message) => message);
        
        //Act
        var messageResponse = await _messageService.SaveMessage(request);

        //Assert
        Assert.Equal(messageResponse.Value.Username, user.Username);
        Assert.Equal(messageResponse.Value.Date, request.Date);
    }
    
    [Fact]
    public async Task SaveMessage_ShouldReturnError_WhenUserNotExists()
    {
        //Arrange
        var userId = Guid.NewGuid().ToString();
        var request = new SaveMessageRequest(
            userId, 
            Guid.NewGuid().ToString(),
            "Message text",
            DateTime.UtcNow, 
            true);

        _userRepositoryMock
            .Setup(x => x.UserExists(userId))
            .ReturnsAsync(false);
        
        //Act
        var messageResponse = await _messageService.SaveMessage(request);

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
            DateTime.UtcNow, 
            true);
        
        //Act
        var messageResponse = await _messageService.SaveMessage(request);

        //Assert
        Assert.Equal(Error.Validation().Type, messageResponse.FirstError.Type);
    }

    [Fact]
    public async Task GetAllRoomMessages_ShouldReturnListOfMessageResponse()
    {
        //Arrange
        var roomId = Guid.NewGuid().ToString();
        var userId = Guid.NewGuid().ToString();

        _userRepositoryMock
            .Setup(x => x.RoomExists(roomId))
            .ReturnsAsync(true);

        var messageList = new List<Message>()
        {
            new Message
            {
                UserId = userId,
                MessageId = Guid.NewGuid().ToString(),
                Date = DateTime.UtcNow,
                FromUser = true,
                RoomId = roomId,
                Text = "message text 1"
            },
            new Message
            {
                UserId = userId,
                MessageId = Guid.NewGuid().ToString(),
                Date = DateTime.UtcNow,
                FromUser = true,
                RoomId = roomId,
                Text = "message text 2"
            }
        };

        _messageRepositoryMock
            .Setup(x => x.GetAllRoomMessages(roomId))
            .ReturnsAsync(messageList);
            

        _userRepositoryMock
            .Setup(x => x.UserExists(userId))
            .ReturnsAsync(true);

        var user = new User
        {
            UserId = userId,
            Username = "username",
            ConnectionId = Guid.NewGuid().ToString(),
            HasLeft = false,
            RoomId = roomId
        };

        _userRepositoryMock
            .Setup(x => x.GetUserById(userId))
            .ReturnsAsync(user);
        
        //Act
        var messageResponse = await _messageService.GetAllRoomMessages(roomId);

        //Assert
        Assert.Equal(messageResponse.Value.Count, messageList.Count);
        Assert.Equal(messageResponse.Value[0].UserId, messageList[1].UserId);
        Assert.Equal(messageResponse.Value[0].MessageId, messageList[0].MessageId);
    }

    [Fact]
    public async Task GetAllRoomMessages_ShouldReturnError_WhenRoomNotExists()
    {
        //Arrange
        var roomId = Guid.NewGuid().ToString();
        
        _userRepositoryMock
            .Setup(x => x.RoomExists(roomId))
            .ReturnsAsync(false);

        //Act
        var messageResponse = await _messageService.GetAllRoomMessages(roomId);

        //Assert
        Assert.Equal(messageResponse.FirstError, Errors.Room.RoomNotFound);
    }
    
    [Fact]
    public async Task RemoveMessage_ShouldReturnDeleted() 
    {
        //Arrange
        var connectionId = Guid.NewGuid().ToString();
        var messageId = Guid.NewGuid().ToString();
        var userId = Guid.NewGuid().ToString();
        var roomId = Guid.NewGuid().ToString();
        
        var request = new RemoveMessageRequest(messageId, connectionId);

        var message = new Message
        {
            UserId = userId,
            MessageId = Guid.NewGuid().ToString(),
            Date = DateTime.UtcNow,
            FromUser = true,
            RoomId = roomId,
            Text = "message text 1"
        };

        _messageRepositoryMock
            .Setup(x => x.GetMessageByIdOrNullIfNotExists(messageId))
            .ReturnsAsync(message);
        
        var user = new User
        {
            UserId = userId,
            Username = "username",
            ConnectionId = connectionId,
            HasLeft = false,
            RoomId = roomId
        };

        _userRepositoryMock
            .Setup(x => x.GetUserByConnectionIdOrNull(connectionId))
            .ReturnsAsync(user);
        
        
        //Act
        var response = await _messageService.RemoveMessage(request);
        
        //Assert 
        Assert.Equal(response.Value, Result.Deleted);
    }

    [Fact]
    public async Task RemoveMessage_ShouldReturnError_WhenUserAreNotOwnerOfMessage()
    {
        //Arrange
        var connectionId = Guid.NewGuid().ToString();
        var messageId = Guid.NewGuid().ToString();
        var roomId = Guid.NewGuid().ToString();

        var request = new RemoveMessageRequest(messageId, connectionId);

        var message = new Message
        {
            UserId = Guid.NewGuid().ToString(),
            MessageId = messageId,
            Date = DateTime.UtcNow,
            FromUser = true,
            RoomId = roomId,
            Text = "message text 1"
        };

        _messageRepositoryMock
            .Setup(x => x.GetMessageByIdOrNullIfNotExists(messageId))
            .ReturnsAsync(message);
        
        var user = new User
        {
            UserId = Guid.NewGuid().ToString(),
            Username = "username",
            ConnectionId = connectionId,
            HasLeft = false,
            RoomId = roomId
        };

        _userRepositoryMock
            .Setup(x => x.GetUserByConnectionIdOrNull(connectionId))
            .ReturnsAsync(user);

        //Act
        var response = await _messageService.RemoveMessage(request);
        
        //Assert 
        Assert.Equal(Errors.Message.MessageIsNotRemoved, response.FirstError);
    }

    [Fact]
    public async Task RemoveMessage_ShouldReturnError_WhenMessageNotExists()
    {
        //Arrange
        var connectionId = Guid.NewGuid().ToString();
        var messageId = Guid.NewGuid().ToString();

        var request = new RemoveMessageRequest(messageId, connectionId);

        _messageRepositoryMock
            .Setup(x => x.GetMessageByIdOrNullIfNotExists(messageId))
            .ReturnsAsync(() => null);

        //Act
        var response = await _messageService.RemoveMessage(request);
        
        //Assert 
        Assert.Equal(Errors.Message.MessageNotFound, response.FirstError);
    }

    [Fact]
    public async Task RemoveMessage_ShouldReturnError_WhenUserNotExists()
    {
        //Arrange
        var connectionId = Guid.NewGuid().ToString();
        var messageId = Guid.NewGuid().ToString();
        
        var request = new RemoveMessageRequest(messageId, connectionId);

        var message = new Message
        {
            UserId = Guid.NewGuid().ToString(),
            MessageId = Guid.NewGuid().ToString(),
            Date = DateTime.UtcNow,
            FromUser = true,
            RoomId = Guid.NewGuid().ToString(),
            Text = "message text"
        };

        _messageRepositoryMock
            .Setup(x => x.GetMessageByIdOrNullIfNotExists(messageId))
            .ReturnsAsync(message);

        _userRepositoryMock
            .Setup(x => x.GetUserByConnectionIdOrNull(request.ConnectionId))
            .ReturnsAsync(() => null);
        
        //Act
        var response = await _messageService.RemoveMessage(request);
        
        //Assert 
        Assert.Equal(Errors.User.UserNotFound, response.FirstError);
    }
}
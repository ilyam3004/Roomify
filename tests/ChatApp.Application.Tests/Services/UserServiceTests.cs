using ChatApp.Application.Common.Interfaces.Persistence;
using ChatApp.Application.Common.Validations;
using ChatApp.Application.Models.Requests;
using ChatApp.Application.Services;
using ChatApp.Domain.Common.Errors;
using ChatApp.Domain.Entities;
using FluentResults;
using FluentValidation;
using Moq;
using Error = ErrorOr.Error;
using Result = ErrorOr.Result;

namespace ChatApp.Application.Tests.Services;

public class UserServiceTests
{
    private readonly UserService _userService;
    private readonly Mock<IUserRepository> _userRepositoryMock = new Mock<IUserRepository>();
    private readonly IValidator<CreateUserRequest> _userValidator = new CreateUserRequestValidator();

    public UserServiceTests()
    {
        _userService = new UserService(_userRepositoryMock.Object, _userValidator);
    }

    [Fact]
    public async Task GetUserByConnectionId_ShouldReturnUserResponse()
    {
        //Arrange
        var connectionId = Guid.NewGuid().ToString();
        var roomId = Guid.NewGuid().ToString();
        var userId = Guid.NewGuid().ToString();
        var username = "username";

        var user = new User
        {
            UserId = userId,
            Username = username,
            ConnectionId = connectionId,
            RoomId = roomId,
            HasLeft = false
        };

        _userRepositoryMock
            .Setup(x => x.GetUserByConnectionIdOrNull(connectionId))
            .ReturnsAsync(user);

        var roomName = "roomName";
        var room = new Room
        {
            RoomId = roomId,
            RoomName = roomName
        };

        _userRepositoryMock
            .Setup(x => x.GetRoomById(roomId))
            .ReturnsAsync(room);

        //Act
        var userResponse = await _userService
            .GetUserByConnectionId(connectionId);

        //Assert
        Assert.Equal(userResponse.Value.ConnectionId, user.ConnectionId);
        Assert.Equal(userResponse.Value.RoomName, room.RoomName);
    }

    [Fact]
    public async Task GetUserByConnectionId_ShouldReturnError_WhenUserNotExists()
    {
        //Arrange
        var connectionId = Guid.NewGuid().ToString();
        var roomId = Guid.NewGuid().ToString();

        _userRepositoryMock
            .Setup(x => x.GetUserByConnectionIdOrNull(connectionId))
            .ReturnsAsync(() => null);

        var roomName = "roomName";
        var room = new Room
        {
            RoomId = roomId,
            RoomName = roomName
        };

        _userRepositoryMock
            .Setup(x => x.GetRoomById(roomId))
            .ReturnsAsync(room);

        //Act
        var userResponse = await _userService
            .GetUserByConnectionId(connectionId);

        //Assert
        Assert.Equal(userResponse.FirstError, Errors.User.UserNotFound);
    }

    [Fact]
    public async Task GetUserByConnectionId_ShouldReturnError_WhenRoomNotExists()
    {
        //Arrange
        var connectionId = Guid.NewGuid().ToString();
        var roomId = Guid.NewGuid().ToString();
        var userId = Guid.NewGuid().ToString();
        var username = "username";

        var user = new User
        {
            UserId = userId,
            Username = username,
            ConnectionId = connectionId,
            RoomId = roomId,
            HasLeft = false
        };

        _userRepositoryMock
            .Setup(x => x.GetUserByConnectionIdOrNull(connectionId))
            .ReturnsAsync(user);

        _userRepositoryMock
            .Setup(x => x.GetRoomById(roomId))
            .ReturnsAsync(() => null);
        //Act
        var userResponse = await _userService.GetUserByConnectionId(connectionId);

        //Assert
        Assert.Equal(userResponse.FirstError, Errors.Room.RoomNotFound);
    }

    [Fact]
    public async Task AddUserToRoom_ShouldReturnUserResponse()
    {
        //Arrange
        var username = "username";
        var roomName = "roomName";
        var connectionId = Guid.NewGuid().ToString();
        var roomId = Guid.NewGuid().ToString();

        var request = new CreateUserRequest(
            username,
            connectionId,
            roomName);

        var room = new Room
        {
            RoomId = roomId,
            RoomName = roomName
        };

        _userRepositoryMock
            .Setup(x => x.CreateRoomIfNotExists(request.RoomName))
            .ReturnsAsync(room);

        _userRepositoryMock
            .Setup(x => x.UserExists(request.Username, room.RoomId))
            .ReturnsAsync(false);

        _userRepositoryMock
            .Setup(x => x
                .AddUser(It.IsAny<User>()))
            .ReturnsAsync((User user) => user);

        //Act
        var userResponse = await _userService.AddUserToRoom(request);

        //Assert   
        Assert.Equal(userResponse.Value.ConnectionId, connectionId);
        Assert.Equal(userResponse.Value.RoomId, roomId);
    }

    [Fact]
    public async Task AddUserToRoom_ShouldReturnError_WhenRequestIsNotValid()
    {
        //Arrange
        var username = "InvalidUsername";
        var roomName = "InvalidRoomName#$%%$##";
        var connectionId = Guid.NewGuid().ToString();

        var request = new CreateUserRequest(
            username,
            connectionId,
            roomName);

        //Act
        var userResponse = await _userService.AddUserToRoom(request);

        //Assert  
        Assert.Equal(userResponse.FirstError.Type, Error.Validation().Type);
    }
    
    [Fact]
    public async Task AddUserToRoom_ShouldReturnError_WhenUsernameAlreadyExistsInRoom()
    {
        //Arrange
        var username = "username";
        var roomName = "roomName";
        var connectionId = Guid.NewGuid().ToString();
        var roomId = Guid.NewGuid().ToString();

        var request = new CreateUserRequest(
            username,
            connectionId,
            roomName);

        var room = new Room
        {
            RoomId = roomId,
            RoomName = roomName
        };

        _userRepositoryMock
            .Setup(x => x.CreateRoomIfNotExists(request.RoomName))
            .ReturnsAsync(room);

        _userRepositoryMock
            .Setup(x => x.UserExists(request.Username, room.RoomId))
            .ReturnsAsync(true);

        //Act
        var userResponse = await _userService.AddUserToRoom(request);

        //Assert   
        Assert.Equal(userResponse.FirstError, Errors.User.DuplicateUsername);
    }

    [Fact]
    public async Task RemoveUserFromRoom_ShouldReturnDeleted()
    {
        //Arrange
        var connectionId = Guid.NewGuid().ToString();
        var userId = Guid.NewGuid().ToString();
        var roomId = Guid.NewGuid().ToString();
        var username = "username";
        var roomName = "roomName";
        
        var user = new User
        {
            UserId = userId,
            Username = username,
            ConnectionId = connectionId,
            RoomId = roomId,
            HasLeft = false
        };
        
        var room = new Room
        {
            RoomId = roomId,
            RoomName = roomName
        };

        _userRepositoryMock
            .Setup(x => x.GetUserByConnectionIdOrNull(connectionId))
            .ReturnsAsync(user);

        _userRepositoryMock
            .Setup(x => x.GetRoomById(roomId))
            .ReturnsAsync(room);
        
        //Act
        var userResponse = await _userService.RemoveUserFromRoom(connectionId);
        
        //Assert
        Assert.Equal(userResponse.Value.ConnectionId, connectionId);
        Assert.Equal(userResponse.Value.RoomId, roomId);
    }
}
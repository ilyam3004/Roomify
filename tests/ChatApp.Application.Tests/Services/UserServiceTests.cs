using ChatApp.Application.Common.Interfaces.Persistence;
using ChatApp.Application.Common.Validations;
using ChatApp.Application.Models.Requests;
using ChatApp.Application.Services;
using ChatApp.Domain.Common.Errors;
using ChatApp.Domain.Entities;
using FluentValidation;
using Moq;
using Error = ErrorOr.Error;

namespace ChatApp.Application.Tests.Services;

public class UserServiceTests
{
    private readonly UserService _userService;
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
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
        var roomId = Guid.NewGuid().ToString();

        var user = new User
        {
            UserId = Guid.NewGuid().ToString(),
            Username = "username",
            ConnectionId = connectionId,
            RoomId = roomId,
            HasLeft = false
        };

        var room = new Room
        {
            RoomId = roomId,
            RoomName = "roomName"
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

    [Fact]
    public async Task RemoveUserFromRoom_ShouldReturnError_WhenUserNotExists()
    {
        
        //Arrange
        var connectionId = Guid.NewGuid().ToString();
        var roomId = Guid.NewGuid().ToString();
        var room = new Room
        {
            RoomName = "roomName",
            RoomId = roomId
        };

        _userRepositoryMock
            .Setup(x => x.GetUserByConnectionIdOrNull(connectionId))
            .ReturnsAsync(() => null);

        _userRepositoryMock
            .Setup(x => x.GetRoomById(roomId))
            .ReturnsAsync(room);

        //Act
        var response = await _userService.RemoveUserFromRoom(connectionId);

        //Assert
        Assert.Equal(response.FirstError, Errors.User.UserNotFound);
    }

    [Fact]
    public async Task RemoveUserFromRoom_ShouldReturnError_WhenRoomNotExists()
    {
        //Arrange
        var connectionId = Guid.NewGuid().ToString();
        var roomId = Guid.NewGuid().ToString();

        var user = new User
        {
            UserId = Guid.NewGuid().ToString(),
            Username = "username",
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
        var response = await _userService.RemoveUserFromRoom(connectionId);

        //Assert
        Assert.Equal(response.FirstError, Errors.Room.RoomNotFound);
    }

    [Fact]
    public async Task GetUserList_ShouldReturnUserList()
    {
        //Arrange
        var roomId = Guid.NewGuid().ToString();

        var room = new Room
        {
            RoomId = roomId,
            RoomName = "RoomName"
        };
        
        _userRepositoryMock
            .Setup(x => x.GetRoomById(roomId))
            .ReturnsAsync(room);
        
        var userList = new List<User>
        {
            new User
            {
                UserId = Guid.NewGuid().ToString(),
                Username = "user1",
                ConnectionId = Guid.NewGuid().ToString(),
                HasLeft = false,
                RoomId = roomId
            },
            new User
            {
                UserId = Guid.NewGuid().ToString(),
                Username = "user2",
                ConnectionId = Guid.NewGuid().ToString(),
                HasLeft = false,
                RoomId = roomId
            }
        };

        _userRepositoryMock
            .Setup(x => x.GetRoomUsers(roomId))
            .ReturnsAsync(userList);
        
        //Act
        var response = await _userService.GetUserList(roomId);
        
        //Assert
        Assert.Equal(response.Value.Count, userList.Count);
        Assert.Equal(response.Value[1].RoomId, userList[0].RoomId);
    }

    [Fact]
    public async Task GetUserList_ShouldReturnError_WhenRoomNotExists()
    {
        //Arrange
        var roomId = Guid.NewGuid().ToString();

        _userRepositoryMock
            .Setup(x => x.GetRoomById(roomId))
            .ReturnsAsync(() => null);
        
        //Act
        var response = await _userService.GetUserList(roomId);
        //Assert
        Assert.Equal(Errors.Room.RoomNotFound, response.FirstError);
    }
}
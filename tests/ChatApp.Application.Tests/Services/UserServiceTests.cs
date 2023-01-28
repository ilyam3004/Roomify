using ChatApp.Application.Common.Interfaces.Persistence;
using ChatApp.Application.Models.Requests;
using ChatApp.Application.Services;
using ChatApp.Domain.Entities;
using ErrorOr;
using FluentValidation;
using Moq;

namespace ChatApp.Application.Tests.Services;

public class UserServiceTests
{
    private readonly UserService _userService;
    private readonly Mock<IUserRepository> _userRepositoryMock = new Mock<IUserRepository>();
    private readonly Mock<IValidator<CreateUserRequest>> _userValidation = new Mock<IValidator<CreateUserRequest>>();
                    
    public UserServiceTests()
    {
        _userService = new UserService(_userRepositoryMock.Object, _userValidation.Object);
    }

    [Fact]
    public async Task GetUserByConnectionId_ShouldReturn_WhenUserAndRoomExists()
    {
        //Arrange
        var connectionId = Guid.NewGuid().ToString();
        var user = new User
        {
            UserId = Guid.NewGuid().ToString(),
            Username = "username",
            ConnectionId = connectionId,
            RoomId = Guid.NewGuid().ToString(),
            HasLeft = false
        };
        _userRepositoryMock.Setup(x => x.GetUserByConnectionIdOrNull(connectionId))
            .ReturnsAsync(user);

        var roomId = user.RoomId;
        var room = new Room
        {
            RoomId = user.RoomId,
            RoomName = "RoomName"
        };
        _userRepositoryMock.Setup(x => x.GetRoomById(roomId))
            .ReturnsAsync(room);
        
        //Act
        var userResponse = await _userService
            .GetUserByConnectionId(connectionId);
        
        //Assert
        Assert.Equal(userResponse.Value.ConnectionId, user.ConnectionId);
    }

    [Fact]
    public async Task GetUserByConnectionId_ShouldReturnError_WhenUserNotExists()
    {
        //Arrange
        var connectionId = Guid.NewGuid().ToString();
        var user = new User
        {
            UserId = Guid.NewGuid().ToString(),
            Username = "username",
            ConnectionId = connectionId,
            RoomId = Guid.NewGuid().ToString(),
            HasLeft = false
        };
        _userRepositoryMock.Setup(x => x.GetUserByConnectionIdOrNull(connectionId))
            .ReturnsAsync(null as User);

        var roomId = user.RoomId;
        var room = new Room
        {
            RoomId = user.RoomId,
            RoomName = "RoomName"
        };
        _userRepositoryMock.Setup(x => x.GetRoomById(roomId))
            .ReturnsAsync(room);
        
        //Act
        var userResponse = await _userService
            .GetUserByConnectionId(connectionId);
        
        //Assert
        Assert.Equal(ErrorType.Conflict, userResponse.FirstError.Type);
    }

    public async Task JoinRoom_ShouldReturnObject_IfRequestValid()
    {
        //Arrange
        
        //Act
        
        //Assert
    }
}
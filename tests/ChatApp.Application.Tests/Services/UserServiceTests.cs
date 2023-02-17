using ChatApp.Application.Common.Interfaces.Persistence;
using ChatApp.Application.Common.Validations;
using ChatApp.Application.Models.Requests;
using ChatApp.Application.Services;
using ChatApp.Domain.Common.Errors;
using ChatApp.Domain.Entities;
using Error = ErrorOr.Error;
using FluentValidation;
using AutoFixture;
using MapsterMapper;
using Moq;

namespace ChatApp.Application.Tests.Services;

public class UserServiceTests
{
    private readonly UserService _sut;
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Fixture _fixture;
    private readonly IValidator<CreateUserRequest> _userValidator = new CreateUserRequestValidator();
    private readonly IMapper _mapper = new Mapper();
    
    public UserServiceTests()
    {
        _fixture = new Fixture();
        _sut = new UserService(_userRepositoryMock.Object, _userValidator, _mapper);
    }

    [Fact]
    public async Task GetUserByConnectionId_ShouldReturnUserResponse()
    {
        // Arrange
        var user = _fixture.Create<User>();

        _userRepositoryMock
            .Setup(x => x.GetUserByConnectionIdOrNull(user.ConnectionId))
            .ReturnsAsync(user);

        var room = _fixture.Build<Room>()
            .With(r => r.RoomId, user.RoomId)
            .Create();

        _userRepositoryMock
            .Setup(x => x.GetRoomById(room.RoomId))
            .ReturnsAsync(room);

        // Act
        var userResponse = await _sut
            .GetUserByConnectionId(user.ConnectionId);

        // Assert
        Assert.Equal(userResponse.Value.ConnectionId, user.ConnectionId);
        Assert.Equal(userResponse.Value.RoomName, room.RoomName);
    }

    [Fact]
    public async Task GetUserByConnectionId_ShouldReturnError_WhenUserNotExists()
    {
        //Arrange
        var connectionId = Guid.NewGuid().ToString();
        var room = _fixture.Create<Room>();

        _userRepositoryMock
            .Setup(x => x.GetUserByConnectionIdOrNull(connectionId))
            .ReturnsAsync(() => null);


        _userRepositoryMock
            .Setup(x => x.GetRoomById(room.RoomId))
            .ReturnsAsync(room);

        //Act
        var userResponse = await _sut
            .GetUserByConnectionId(connectionId);

        //Assert
        Assert.Equal(userResponse.FirstError, Errors.User.UserNotFound);
    }

    [Fact]
    public async Task AddUserToRoom_ShouldReturnUserResponse()
    {
        // Arrange
        var request = _fixture.Build<CreateUserRequest>()
            .With(r => r.Username, "Username")
            .With(r => r.RoomName, "Roomname")
            .Create();

        var room = _fixture.Build<Room>()
            .With(r => r.RoomName, request.RoomName)
            .Create();

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

        // Act
        var userResponse = await _sut.AddUserToRoom(request);

        // Assert   
        Assert.Equal(userResponse.Value.ConnectionId, request.ConnectionId);
        Assert.Equal(userResponse.Value.RoomId, room.RoomId);
    }

    [Fact]
    public async Task AddUserToRoom_ShouldReturnError_WhenRequestIsNotValid()
    {
        // Arrange
        var request = _fixture.Build<CreateUserRequest>()
            .With(r => r.Username, "InvalidUsername")
            .With(r => r.RoomName, "InvalidRoomName#$%%$##")
            .Create();

        // Act
        var userResponse = await _sut.AddUserToRoom(request);

        // Assert  
        Assert.Equal(userResponse.FirstError.Type, Error.Validation().Type);
    }

    [Fact]
    public async Task AddUserToRoom_ShouldReturnError_WhenUsernameAlreadyExistsInRoom()
    {
        // Arrange
        var request = _fixture.Build<CreateUserRequest>()
            .With(r => r.Username, "Username")
            .With(r => r.RoomName, "Roomname")
            .Create();

        var room = _fixture.Build<Room>()
          .With(r => r.RoomName, request.RoomName)
          .Create();

        _userRepositoryMock
            .Setup(x => x.CreateRoomIfNotExists(request.RoomName))
            .ReturnsAsync(room);

        _userRepositoryMock
            .Setup(x => x.UserExists(request.Username, room.RoomId))
            .ReturnsAsync(true);

        // Act
        var userResponse = await _sut.AddUserToRoom(request);

        // Assert   
        Assert.Equal(userResponse.FirstError, Errors.User.DuplicateUsername);
    }

    [Fact]
    public async Task RemoveUserFromRoom_ShouldReturnDeleted()
    {
        // Arrange
        var user = _fixture.Create<User>();

        var room = _fixture.Build<Room>()
            .With(r => r.RoomId, user.RoomId)
            .Create();

        _userRepositoryMock
            .Setup(x => x.GetUserByConnectionIdOrNull(user.ConnectionId))
            .ReturnsAsync(user);

        _userRepositoryMock
            .Setup(x => x.GetRoomById(room.RoomId))
            .ReturnsAsync(room);

        // Act
        var userResponse = await _sut.RemoveUserFromRoom(user.ConnectionId);

        // Assert
        Assert.Equal(userResponse.Value.ConnectionId, user.ConnectionId);
        Assert.Equal(userResponse.Value.RoomId, room.RoomId);
    }

    [Fact]
    public async Task RemoveUserFromRoom_ShouldReturnError_WhenUserNotExists()
    {
        
        // Arrange
        var connectionId = Guid.NewGuid().ToString();
        var room = _fixture.Create<Room>();

        _userRepositoryMock
            .Setup(x => x.GetUserByConnectionIdOrNull(connectionId))
            .ReturnsAsync(() => null);

        _userRepositoryMock
            .Setup(x => x.GetRoomById(room.RoomId))
            .ReturnsAsync(room);

        // Act
        var response = await _sut.RemoveUserFromRoom(connectionId);

        // Assert
        Assert.Equal(response.FirstError, Errors.User.UserNotFound);
    }
    
    [Fact]
    public async Task RemoveUserFromRoom_ShouldReturnError_WhenRoomIsEmpty()
    {
        // Arrange
        var user = _fixture.Create<User>();

        var room = _fixture.Build<Room>()
            .With(r => r.RoomId, user.RoomId)
            .Create();

        _userRepositoryMock
            .Setup(u => u.GetRoomById(user.RoomId))
            .ReturnsAsync(room);

        _userRepositoryMock
            .Setup(x => x.GetUserByConnectionIdOrNull(user.ConnectionId))
            .ReturnsAsync(user);

        _userRepositoryMock
            .Setup(x => x.RemoveRoomDataIfEmpty(user.RoomId, user.UserId))
            .ReturnsAsync(true);

        // Act
        var userResponse = await _sut.RemoveUserFromRoom(user.ConnectionId);

        // Assert
        Assert.Equal(userResponse.FirstError, Errors.Room.RoomIsEmpty);
    }

    [Fact]
    public async Task GetUserList_ShouldReturnUserList()
    {
        // Arrange
        var room = _fixture.Create<Room>();
        
        _userRepositoryMock
            .Setup(x => x.GetRoomById(room.RoomId))
            .ReturnsAsync(room);

        var userList = _fixture.Build<User>()
            .With(u => u.RoomId, room.RoomId)
            .CreateMany(2)
            .ToList();

        _userRepositoryMock
            .Setup(x => x.GetRoomUsers(room.RoomId))
            .ReturnsAsync(userList);
        
        //Act
        var response = await _sut.GetUserList(room.RoomId);
        
        //Assert
        Assert.Equal(response.Count, userList.Count);
        Assert.Equal(response[1].RoomId, userList[0].RoomId);
    }
}
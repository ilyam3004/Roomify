using ChatApp.Application.Common.Interfaces.Persistence;
using ChatApp.Application.Users.Commands.LeaveRoom;
using ChatApp.Application.Tests.Config;
using ChatApp.Domain.Entities;
using MapsterMapper;
using AutoFixture;
using Moq;
using ChatApp.Domain.Common.Errors;

namespace ChatApp.Application.Tests.Users.Commands;

public class LeaveRoomCommandHandlerTests
{
    private readonly IMapper _mapper = MapsterConfigForTesting.GetMapper();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly LeaveRoomCommandHandler _sut;
    private readonly Fixture _fixture;

    public LeaveRoomCommandHandlerTests()
    {
        _fixture = new Fixture();
        _sut = new LeaveRoomCommandHandler(
            _userRepositoryMock.Object,
            _mapper);
    }

    [Fact]
    public async Task Handler_ShouldReturnDeleted()
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

        var command = new LeaveRoomCommand(user.ConnectionId);

        // Act
        var userResponse = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(userResponse.Value.ConnectionId, user.ConnectionId);
        Assert.Equal(userResponse.Value.RoomId, room.RoomId);
    }

    [Fact]
    public async Task Handler_ShouldReturnError_WhenUserNotExists()
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

        var command = new LeaveRoomCommand(Guid.NewGuid().ToString());

        // Act
        var response = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(response.FirstError, Errors.User.UserNotFound);
    }

    [Fact]
    public async Task Handler_ShouldReturnError_WhenRoomIsEmpty()
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
        
        var command = new LeaveRoomCommand(user.ConnectionId);

        // Act
        var userResponse = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(userResponse.FirstError, Errors.Room.RoomIsEmpty);
    }
}
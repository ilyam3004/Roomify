using ChatApp.Application.Users.Commands.LeaveRoom;
using ChatApp.Application.Common.Interfaces;
using ChatApp.Application.Tests.Config;
using ChatApp.Domain.Common.Errors;
using ChatApp.Domain.Entities;
using MapsterMapper;
using AutoFixture;
using Moq;

namespace ChatApp.Application.Tests.Users.Commands;

public class LeaveRoomCommandHandlerTests
{
    private readonly IMapper _mapper = MapsterConfigForTesting.GetMapper();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly LeaveRoomCommandHandler _sut;
    private readonly Fixture _fixture;

    public LeaveRoomCommandHandlerTests()
    {
        _fixture = new Fixture();
        _sut = new LeaveRoomCommandHandler(
            _unitOfWorkMock.Object,
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

        _unitOfWorkMock
            .Setup(u => 
                u.Users.GetUserByConnectionIdOrNull(user.ConnectionId))
            .ReturnsAsync(user);

        _unitOfWorkMock
            .Setup(x => 
                x.Users.GetRoomById(room.RoomId))
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

        _unitOfWorkMock
            .Setup(u => 
                u.Users.GetUserByConnectionIdOrNull(connectionId))
            .ReturnsAsync(() => null);

        _unitOfWorkMock
            .Setup(u => 
                u.Users.GetRoomById(room.RoomId))
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

        _unitOfWorkMock
            .Setup(u => 
                u.Users.GetRoomById(user.RoomId))
            .ReturnsAsync(room);

        _unitOfWorkMock
            .Setup(u => 
                u.Users.GetUserByConnectionIdOrNull(user.ConnectionId))
            .ReturnsAsync(user);

        _unitOfWorkMock
            .Setup(u => 
                u.Users.RemoveRoomDataIfEmpty(user.RoomId, user.UserId))
            .ReturnsAsync(true);
        
        var command = new LeaveRoomCommand(user.ConnectionId);

        // Act
        var userResponse = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(userResponse.FirstError, Errors.Room.RoomIsEmpty);
    }
}
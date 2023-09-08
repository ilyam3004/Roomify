using ChatApp.Application.Tests.Config;
using FluentValidation;
using MapsterMapper;
using AutoFixture;
using ErrorOr;
using Moq;
using Roomify.Application.Common.Interfaces;
using Roomify.Application.Users.Commands.JoinRoom;
using Roomify.Domain.Common.Errors;
using Roomify.Domain.Entities;

namespace ChatApp.Application.Tests.Users.Commands;

public class JoinUserCommandHandlerTests
{
    private readonly IValidator<JoinRoomCommand> _commandValidator = new JoinRoomCommandValidator();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly IMapper _mapper = MapsterConfigForTesting.GetMapper();
    private readonly JoinUserCommandHandler _sut;
    private readonly Fixture _fixture;

    public JoinUserCommandHandlerTests()
    {
        _fixture = new Fixture();
        _sut = new JoinUserCommandHandler(
            _unitOfWorkMock.Object, 
            _mapper,
            _commandValidator);
    }

    [Fact]
    public async Task Handler_ShouldReturnUserResponse()
    {
        // Arrange
        var command = _fixture.Build<JoinRoomCommand>()
            .With(r => r.Username, "Username")
            .With(r => r.RoomName, "Roomname")
            .Create();

        var room = _fixture.Build<Room>()
            .With(r => r.RoomName, command.RoomName)
            .Create();

        _unitOfWorkMock
            .Setup(u => 
                u.Users.CreateRoomIfNotExists(command.RoomName))
            .ReturnsAsync(room);

        _unitOfWorkMock
            .Setup(u => 
                u.Users.UserExists(command.Username, room.RoomId))
            .ReturnsAsync(false);

        _unitOfWorkMock
            .Setup(u => 
                u.Users.AddUser(It.IsAny<User>()))
            .ReturnsAsync((User user) => user);

        // Act
        var userResponse = await _sut.Handle(command, CancellationToken.None);

        // Assert   
        Assert.Equal(userResponse.Value.ConnectionId, command.ConnectionId);
        Assert.Equal(userResponse.Value.RoomId, room.RoomId);
    }

    [Fact]
    public async Task Handler_ShouldReturnError_WhenRequestIsNotValid()
    {
        // Arrange
        var command = _fixture.Build<JoinRoomCommand>()
            .With(r => r.Username, "InvalidUsername")
            .With(r => r.RoomName, "InvalidRoomName#$%%$##")
            .Create();

        // Act
        var userResponse = await _sut.Handle(command, CancellationToken.None);

        // Assert  
        Assert.Equal(userResponse.FirstError.Type, Error.Validation().Type);
    }

    [Fact]
    public async Task Handler_ShouldReturnError_WhenUsernameAlreadyExistsInRoom()
    {
        // Arrange
        var command = _fixture.Build<JoinRoomCommand>()
            .With(r => r.Username, "Username")
            .With(r => r.RoomName, "Roomname")
            .Create();

        var room = _fixture.Build<Room>()
            .With(r => r.RoomName, command.RoomName)
            .Create();

        _unitOfWorkMock
            .Setup(u => 
                u.Users.CreateRoomIfNotExists(command.RoomName))
            .ReturnsAsync(room);

        _unitOfWorkMock
            .Setup(u => 
                u.Users.UserExists(command.Username, room.RoomId))
            .ReturnsAsync(true);

        // Act
        var userResponse = await _sut.Handle(command, CancellationToken.None);

        // Assert   
        Assert.Equal(userResponse.FirstError, Errors.User.DuplicateUsername);
    }
}
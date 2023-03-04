using ChatApp.Application.Users.Queries.GetUserByConnId;
using ChatApp.Application.Common.Interfaces;
using ChatApp.Application.Models.Responses;
using ChatApp.Application.Tests.Config;
using ChatApp.Domain.Common.Errors;
using ChatApp.Domain.Entities;
using MapsterMapper;
using AutoFixture;
using Moq;

namespace ChatApp.Application.Tests.Users.Queries;

public class GetUserByConnIdQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly IMapper _mapper = MapsterConfigForTesting.GetMapper();
    private readonly GetUserByConnIdQueryHandler _sut;
    private readonly Fixture _fixture;

    public GetUserByConnIdQueryHandlerTests()
    {
        _fixture = new Fixture();
        _sut = new GetUserByConnIdQueryHandler(
            _unitOfWorkMock.Object,
            _mapper);
    }

    [Fact]
    public async Task Handler_ShouldReturnUserResponse()
    {
        // Arrange
        var user = _fixture.Create<User>();

        _unitOfWorkMock
            .Setup(u => 
                u.Users.GetUserByConnectionIdOrNull(user.ConnectionId))
            .ReturnsAsync(user);

        var room = _fixture.Build<Room>()
            .With(r => r.RoomId, user.RoomId)
            .Create();

        _unitOfWorkMock
            .Setup(u => u.
                Users.GetRoomById(room.RoomId))
            .ReturnsAsync(room);

        var expectedResponse = new UserResponse(
            user.UserId,
            user.Username,
            user.ConnectionId,
            room.RoomId,
            room.RoomName,
            user.Avatar);

        var query = new GetUserByConnIdQuery(user.ConnectionId);

        // Act
        var actualResponse = await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(expectedResponse, actualResponse);
    }

    [Fact]
    public async Task Handler_ShouldReturnError_WhenUserNotExists()
    {
        //Arrange
        var connectionId = Guid.NewGuid().ToString();

        _unitOfWorkMock
            .Setup(u => 
                u.Users.GetUserByConnectionIdOrNull(connectionId))
            .ReturnsAsync(() => null);
        var query = new GetUserByConnIdQuery(Guid.NewGuid().ToString());

        //Act
        var actualResponse = await _sut.Handle(query, CancellationToken.None);

        //Assert
        Assert.Equal(Errors.User.UserNotFound, actualResponse.FirstError);
    }
}
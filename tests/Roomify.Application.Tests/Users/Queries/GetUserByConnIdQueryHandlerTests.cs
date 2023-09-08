using ChatApp.Application.Tests.Config;
using MapsterMapper;
using AutoFixture;
using Moq;
using Roomify.Application.Common.Interfaces;
using Roomify.Application.Models.Responses;
using Roomify.Application.Users.Queries.GetUserByConnId;
using Roomify.Domain.Common.Errors;
using Roomify.Domain.Entities;

namespace ChatApp.Application.Tests.Users.Queries;

public class GetUserByConnIdQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly IMapper _mapper = MapsterConfigForTesting.GetMapper();
    private readonly GetUserByConnectionIdQueryHandler _sut;
    private readonly Fixture _fixture;

    public GetUserByConnIdQueryHandlerTests()
    {
        _fixture = new Fixture();
        _sut = new GetUserByConnectionIdQueryHandler(
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

        var query = new GetUserByConnectionIdQuery(user.ConnectionId);

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
        var query = new GetUserByConnectionIdQuery(Guid.NewGuid().ToString());

        //Act
        var actualResponse = await _sut.Handle(query, CancellationToken.None);

        //Assert
        Assert.Equal(Errors.User.UserNotFound, actualResponse.FirstError);
    }
}
using ChatApp.Application.Common.Interfaces.Persistence;
using ChatApp.Application.Tests.Config;
using ChatApp.Application.Users.Queries.GetUserList;
using ChatApp.Domain.Entities;
using MapsterMapper;
using AutoFixture;
using Moq;

namespace ChatApp.Application.Tests.Users.Queries;

public class GetUserListQueryHandlerTests
{
    private readonly IMapper _mapper = MapsterConfigForTesting.GetMapper();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly GetUserListQueryHandler _sut;
    private readonly Fixture _fixture;
    
    public GetUserListQueryHandlerTests()
    {
        _fixture = new Fixture();
        _sut = new GetUserListQueryHandler(
            _userRepositoryMock.Object,
            _mapper);
    }

    [Fact]
    public async Task Handler_ShouldReturnUserList()
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

        var query = new GetUserListQuery(room.RoomId);

        //Act
        var response = await _sut.Handle(query, CancellationToken.None);

        //Assert
        Assert.Equal(response.Count, userList.Count);
        Assert.Equal(response[1].RoomId, userList[0].RoomId);
    }
}
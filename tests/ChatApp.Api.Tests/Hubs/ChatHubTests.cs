using ChatApp.Application.Services;
using ChatApp.Api.Hubs;
using AutoFixture;
using Moq;
using ErrorOr;
using ChatApp.Contracts.Rooms;
using ChatApp.Application.Models.Requests;
using SignalR_UnitTestingSupportXUnit.Hubs;
using ChatApp.Application.Models.Responses;
using ChatApp.Domain.Common.Errors;

namespace ChatApp.Api.Tests.Hubs;
public  class ChatHubTests : HubUnitTestsBase
{
    private readonly ChatHub _sut;
    private readonly Mock<IUserService> _userServiceMock = new();
    private readonly Mock<IMessageService> _messageServiceMock = new();
    private readonly Fixture _fixture;

    public ChatHubTests() 
    {
        _sut = new ChatHub(_userServiceMock.Object, _messageServiceMock.Object);
        AssignToHubRequiredProperties(_sut);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task Test()
    {
        var joinUserRequest = _fixture.Create<JoinUserRequest>();

        var userResponse = _fixture.Create<UserResponse>();

        _userServiceMock
            .Setup(a => a.AddUserToRoom(It.IsAny<CreateUserRequest>()))
            .ReturnsAsync(userResponse);
        
        _messageServiceMock
            .Setup(x => x.SaveMessage(It.IsAny<SaveMessageRequest>()))
            .ReturnsAsync(_fixture.Create<MessageResponse>());

        await _sut.JoinRoom(joinUserRequest);
        
        _userServiceMock.Verify(u => u.AddUserToRoom(It.IsAny<CreateUserRequest>()), Times.Once);
    }
}


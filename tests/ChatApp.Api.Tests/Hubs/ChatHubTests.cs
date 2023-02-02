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
using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.AspNetCore.SignalR;
using Range = Moq.Range;

namespace ChatApp.Api.Tests.Hubs;

public class ChatHubTests : HubUnitTestsBase
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
    public async Task JoinRoom_ShouldSendAllDataToRoom_WhenServiceReturnsSuccess()
    {
        // Arrange
        var joinUserRequest = _fixture.Create<JoinUserRequest>();

        var userResponse = _fixture.Create<UserResponse>();

        _userServiceMock
            .Setup(a => a.AddUserToRoom(It.IsAny<CreateUserRequest>()))
            .ReturnsAsync(userResponse);

        _messageServiceMock
            .Setup(x => x.SaveMessage(It.IsAny<SaveMessageRequest>()))
            .ReturnsAsync(_fixture.Create<MessageResponse>());


        // Act
        await _sut.JoinRoom(joinUserRequest);

        //Assert
        ClientsMock.Verify(c =>
                c.Client(It.IsAny<string>()),
            Times.Between(2, 2, Range.Inclusive));

        GroupsMock.Verify(g =>
            g.AddToGroupAsync(It.IsAny<string>(),
                It.IsAny<string>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task JoinRoom_ShouldSendErrorToClient_WhenServiceReturnsError()
    {
        // Arrange
        var joinUserRequest = _fixture.Create<JoinUserRequest>();

        _userServiceMock
            .Setup(a => a.AddUserToRoom(It.IsAny<CreateUserRequest>()))
            .ReturnsAsync(Errors.User.DuplicateUsername);

        // Act
        await _sut.JoinRoom(joinUserRequest);

        //Assert
        ClientsMock.Verify(c => c.Client(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task SendUserMessage_ShouldSendMessageToRoom_WhenServiceReturnsSuccess()
    {
        // Arrange
        var userResponse = _fixture.Create<UserResponse>();
        var messageResponse = _fixture.Create<MessageResponse>();
        
        _userServiceMock
            .Setup(u => u.GetUserByConnectionId(It.IsAny<string>()))
            .ReturnsAsync(userResponse);

        _messageServiceMock
            .Setup(m => m.SaveMessage(It.IsAny<SaveMessageRequest>()))
            .ReturnsAsync(messageResponse);
        
        // Act
        await _sut.SendUserMessage("message");
        
        //Assert
        ClientsMock.Verify(c => c.Group(It.IsAny<string>()), Times.Once);
    }
    
    [Fact]
    public async Task SendUserMessage_ShouldSendErrorToClient_WhenServiceReturnsError()
    {
        // Arrange

        _userServiceMock
            .Setup(u => u.GetUserByConnectionId(It.IsAny<string>()))
            .ReturnsAsync(Errors.User.UserNotFound);

        // Act
        await _sut.SendUserMessage("message");
        
        //Assert
        ClientsMock.Verify(c => c.Client(It.IsAny<string>()), Times.Once);
    }
}


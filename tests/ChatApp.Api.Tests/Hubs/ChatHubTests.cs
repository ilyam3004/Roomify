using ChatApp.Application.Messages.Commands.SaveMessage;
using ChatApp.Application.Users.Queries.GetUserByConnId;
using ChatApp.Application.Users.Queries.GetUserList;
using ChatApp.Application.Users.Commands.LeaveRoom;
using ChatApp.Application.Users.Commands.JoinRoom;
using ChatApp.Application.Models.Responses;
using SignalR_UnitTestingSupportXUnit.Hubs;
using ChatApp.Domain.Common.Errors;
using ChatApp.Contracts.Rooms;
using ChatApp.Api.Hubs;
using MapsterMapper;
using AutoFixture;
using MediatR;
using Moq;

namespace ChatApp.Api.Tests.Hubs;

public class ChatHubTests : HubUnitTestsBase
{
    private readonly IMapper _mapper = MapsterConfigForTesting.GetMapper();
    private readonly Mock<ISender> _mediatorMock = new();
    private readonly Fixture _fixture;
    private readonly ChatHub _sut;

    public ChatHubTests()
    {
        _sut = new ChatHub(
            _mediatorMock.Object, 
            _mapper); 
        _fixture = new Fixture();
        AssignToHubRequiredProperties(_sut);
    }

    [Fact]
    public async Task JoinRoom_ShouldSendAllDataToRoom_WhenServiceReturnsSuccess()
    {
        // Arrange
        var request = _fixture.Create<JoinRoomRequest>();
        var messageResponse = _fixture.Create<MessageResponse>();

        var userResponse = _fixture.Create<UserResponse>();
        
        _mediatorMock
            .Setup(m => m.Send(
                It.IsAny<JoinRoomCommand>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(userResponse);
        
        _mediatorMock
            .Setup(m => m.Send(
                It.IsAny<SaveMessageCommand>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(messageResponse);

        // Act
        await _sut.JoinRoom(request);

        //Assert
        _mediatorMock
            .Verify(x => x.Send(
                It.IsAny<JoinRoomCommand>(), 
                It.IsAny<CancellationToken>()), Times.Once());
        
        _mediatorMock
            .Verify(x => x.Send(
                It.IsAny<SaveMessageCommand>(), 
                It.IsAny<CancellationToken>()), Times.Once());

        ClientsMock.Verify(c =>
                c.Client(It.IsAny<string>()),
            Times.Between(2, 2, Moq.Range.Inclusive));

        GroupsMock.Verify(g =>
            g.AddToGroupAsync(It.IsAny<string>(),
                It.IsAny<string>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task JoinRoom_ShouldSendErrorToClient_WhenAddUserToRoomReturnsError()
    {
        // Arrange
        var joinUserRequest = _fixture.Create<JoinRoomRequest>();
    
        _mediatorMock
            .Setup(m => m.Send(
                It.IsAny<JoinRoomCommand>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Errors.User.DuplicateUsername);
    
        // Act
        await _sut.JoinRoom(joinUserRequest);
    
        //Assert
        _mediatorMock
            .Verify(x => x.Send(
                It.IsAny<JoinRoomCommand>(), 
                It.IsAny<CancellationToken>()), Times.Once());
        
        ClientsMock.Verify(c => c.Client(It.IsAny<string>()), Times.Once);
    }
    
    [Fact]
    public async Task SendUserMessage_ShouldSendMessageToRoom_WhenServiceReturnsSuccess()
    {
        // Arrange
        var userResponse = _fixture.Create<UserResponse>();
        var messageResponse = _fixture.Create<MessageResponse>();
        
        _mediatorMock
            .Setup(m => m.Send(
                It.IsAny<GetUserByConnIdQuery>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(userResponse);
        
        _mediatorMock
            .Setup(m => m.Send(
                It.IsAny<SaveMessageCommand>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(messageResponse);
        
        // Act
        await _sut.SendUserMessage("message");
        
        //Assert
        ClientsMock.Verify(c => c.Group(It.IsAny<string>()), Times.Once);
    }
    
    [Fact]
    public async Task SendUserMessage_ShouldSendErrorToClient_WhenGetUserByConnectionIdReturnsError()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(
                It.IsAny<GetUserByConnIdQuery>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Errors.User.UserNotFound);
    
        // Act
        await _sut.SendUserMessage("message");
        
        //Assert
        _mediatorMock
            .Verify(m => m.Send(
                It.IsAny<GetUserByConnIdQuery>(),
                It.IsAny<CancellationToken>()), Times.Once());
        
        ClientsMock.Verify(c => c.Client(It.IsAny<string>()), Times.Once);
    }
    
    [Fact]
    public async Task SendUserMessage_ShouldSendErrorToClient_WhenSaveMessageReturnsError()
    {
        //Arrange
        var userResponse = _fixture.Create<UserResponse>();
        string message = "Message text";
        
        _mediatorMock
            .Setup(m => m.Send(
                It.IsAny<GetUserByConnIdQuery>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(userResponse);
        
        _mediatorMock
            .Setup(m => m.Send(
                It.IsAny<SaveMessageCommand>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Errors.User.UserNotFound);
        
        //Act
        await _sut.SendUserMessage(message);
        
        //Assert
        ClientsMock.Verify(c => c.Client(It.IsAny<string>()), Times.Once);
    }
    
    [Fact]
    public async Task OnDisconnectedAsync_ShouldSendDataAboutUserLeavingToRoom_WhenServiceReturnsSuccess()
    {
        //Arrange
        var userList = _fixture.CreateMany<UserResponse>(2).ToList();
        var userResponse = _fixture.Create<UserResponse>();
        var messageResponse = _fixture.Create<MessageResponse>();

        _mediatorMock
            .Setup(m => m.Send(
                It.IsAny<LeaveRoomCommand>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(userResponse);
    
        _mediatorMock
            .Setup(m => m.Send(
                It.IsAny<GetUserListQuery>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(userList);
    
        _mediatorMock
            .Setup(m => m.Send(
                It.IsAny<SaveMessageCommand>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(messageResponse);
    
        //Act
        await _sut.OnDisconnectedAsync(It.IsAny<Exception>());
    
        //Assert
        ClientsMock.Verify(c => 
            c.Group(It.IsAny<string>()), Times.Between(1, 2, Moq.Range.Inclusive));
    }
    
    [Fact]
    public async Task OnDisconnectedAsync_ShouldSendDataAboutUserLeavingToRoom_WhenServiceReturnsNotFoundError()
    {
        //Arrange
        _mediatorMock
            .Setup(m => m.Send(
                It.IsAny<LeaveRoomCommand>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Errors.User.UserNotFound);

        //Act
        await _sut.OnDisconnectedAsync(It.IsAny<Exception>());
    
        //Assert
        _mediatorMock
            .Verify(m => m.Send(
                It.IsAny<LeaveRoomCommand>(),
                It.IsAny<CancellationToken>()), Times.Once);

        ClientsMock.Verify(c => c.Client(It.IsAny<string>()), Times.Once);
    }
}


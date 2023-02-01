using ChatApp.Api.Hubs;
using ChatApp.Application.Services;
using Moq;

namespace ChatApp.Api.Tests.Hubs;
public  class ChatHubTests
{
    private readonly ChatHub _sut;
    private readonly Mock<IUserService> _userService = new Mock<IUserService>();
    private readonly Mock<IMessageService> _messageService = new Mock<IMessageService>();

    public ChatHubTests() 
    {
        _sut = new ChatHub(_userService.Object, _messageService.Object);
    }   
}


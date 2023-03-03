using ChatApp.Application.Common.Interfaces.Persistence;
using ChatApp.Application.Messages.Commands.SaveMessage;
using ChatApp.Application.Tests.Config;
using ChatApp.Domain.Common.Errors;
using ChatApp.Domain.Entities;
using FluentValidation;
using MapsterMapper;
using AutoFixture;
using ErrorOr;
using Moq;

namespace ChatApp.Application.Tests.Messages.Commands;

public class SaveMessageCommandHandlerTests
{
    private readonly IValidator<SaveMessageCommand> _commandValidator = new SaveMessageCommandValidator();
    private readonly Mock<IMessageRepository> _messageRepositoryMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly IMapper _mapper = MapsterConfigForTesting.GetMapper();
    private readonly SaveMessageCommandHandler _sut;
    private readonly Fixture _fixture;

    public SaveMessageCommandHandlerTests()
    {   
        _fixture = new Fixture();
        _sut = new SaveMessageCommandHandler(
            _userRepositoryMock.Object, 
            _commandValidator,
            _messageRepositoryMock.Object,
            _mapper);
    }

    [Fact]
    public async Task Handler_ShouldReturnMessageResponse()
    {
        //Arrange
        var command = _fixture.Build<SaveMessageCommand>()
            .With(r => r.Text, "message")
            .Create();
        
        var user = _fixture.Build<User>()
            .With(u => u.UserId, command.UserId)
            .Create();

        var message = _fixture.Build<Message>()
            .With(m => m.Text, command.Text)
            .With(m => m.RoomId, command.RoomId)
            .With(m => m.UserId, command.UserId)
            .Create();

        _userRepositoryMock
            .Setup(x => x.UserExists(user.UserId))
            .ReturnsAsync(true);

        _messageRepositoryMock
            .Setup(x => x.SaveMessage(It.IsAny<Message>()))
            .ReturnsAsync(message);
        
        _userRepositoryMock
            .Setup(x => x.GetUserById(user.UserId))
            .ReturnsAsync(user);
        
        //Act
        var actualResponse = await _sut.Handle(command, CancellationToken.None);

        //Assert
        Assert.Equal(message.UserId, actualResponse.Value.UserId);
        Assert.Equal(message.MessageId, actualResponse.Value.MessageId);
    }
    
    [Fact]
    public async Task Handler_ShouldReturnError_WhenUserNotExists()
    {
        //Arrange
        var command = _fixture.Create<SaveMessageCommand>();

        _userRepositoryMock
            .Setup(x => x.UserExists(command.UserId))
            .ReturnsAsync(false);
        
        //Act
        var messageResponse = await _sut.Handle(command, CancellationToken.None);

        //Assert
        Assert.Equal(Errors.User.UserNotFound, messageResponse.FirstError);
    }

    [Fact]
    public async Task Handler_ShouldReturnError_WhenRequestIsNotValid()
    {
        //Arrange
        var command = new SaveMessageCommand(
            "",
            "",
            "",
            "",
            true);

        _userRepositoryMock
            .Setup(x => x.UserExists(command.UserId))
            .ReturnsAsync(true);

        //Act
        var messageResponse = await _sut.Handle(command, CancellationToken.None);

        //Assert
        Assert.Equal(Error.Validation().Type, messageResponse.FirstError.Type);
    }
}
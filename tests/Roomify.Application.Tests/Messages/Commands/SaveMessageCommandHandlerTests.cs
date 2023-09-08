using AutoFixture;
using ChatApp.Application.Tests.Config;
using ErrorOr;
using FluentValidation;
using MapsterMapper;
using Moq;
using Roomify.Application.Common.Interfaces;
using Roomify.Application.Messages.Commands.SaveMessage;
using Roomify.Domain.Common.Errors;
using Roomify.Domain.Entities;

namespace Roomify.Application.Tests.Messages.Commands;

public class SaveMessageCommandHandlerTests
{
    private readonly IValidator<SaveMessageCommand> _commandValidator = new SaveMessageCommandValidator();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly IMapper _mapper = MapsterConfigForTesting.GetMapper();
    private readonly SaveMessageCommandHandler _sut;
    private readonly Fixture _fixture;

    public SaveMessageCommandHandlerTests()
    {   
        _fixture = new Fixture();
        _sut = new SaveMessageCommandHandler(
            _unitOfWorkMock.Object,
            _commandValidator,
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

        _unitOfWorkMock
            .Setup(u => 
                u.Users.UserExists(user.UserId))
            .ReturnsAsync(true);

        _unitOfWorkMock
            .Setup(u => 
                u.Messages.SaveMessage(It.IsAny<Message>()))
            .ReturnsAsync(message);
        
        _unitOfWorkMock
            .Setup(u => 
                u.Users.GetUserById(user.UserId))
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

        _unitOfWorkMock
            .Setup(u => 
                u.Users.UserExists(command.UserId))
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

        _unitOfWorkMock
            .Setup(u => 
                u.Users.UserExists(command.UserId))
            .ReturnsAsync(true);

        //Act
        var messageResponse = await _sut.Handle(command, CancellationToken.None);

        //Assert
        Assert.Equal(Error.Validation().Type, messageResponse.FirstError.Type);
    }
}
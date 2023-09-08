using AutoFixture;
using ChatApp.Application.Tests.Config;
using ErrorOr;
using FluentValidation;
using MapsterMapper;
using Moq;
using Roomify.Application.Common.Interfaces;
using Roomify.Application.Messages.Commands.SaveImage;
using Roomify.Domain.Common.Errors;
using Roomify.Domain.Entities;

namespace Roomify.Application.Tests.Messages.Commands;

public class SaveImageCommandHandlerTests
{
    private readonly IValidator<SaveImageCommand> _commandValidator = new SaveImageCommandValidator();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly IMapper _mapper = MapsterConfigForTesting.GetMapper();
    private readonly SaveImageCommandHandler _sut;
    private readonly Fixture _fixture;

    public SaveImageCommandHandlerTests()
    {
        _fixture = new Fixture();
        _sut = new SaveImageCommandHandler(
            _unitOfWorkMock.Object,
            _commandValidator,
            _mapper);
    }

    [Fact]
    public async Task Handler_ShouldReturnMessageResponse()
    {
        //Arrange
        var command = _fixture.Create<SaveImageCommand>();

        var user = _fixture.Build<User>()
            .With(u => u.UserId, command.UserId)
            .With(u => u.RoomId, command.RoomId)
            .Create();

        var message = _fixture.Build<Message>()
            .With(m => m.UserId, command.UserId)
            .With(m => m.RoomId, command.RoomId)
            .With(m => m.IsImage, true)
            .With(m => m.ImageUrl, command.ImageUrl)
            .Create();

        _unitOfWorkMock
            .Setup(u => 
                u.Users.UserExists(It.IsAny<string>()))
            .ReturnsAsync(true);

        _unitOfWorkMock
            .Setup(u =>
                u.Users.GetUserById(It.IsAny<string>()))
            .ReturnsAsync(user);

        _unitOfWorkMock
            .Setup(u => 
                u.Messages.SaveMessage(It.IsAny<Message>()))
            .ReturnsAsync(message);
        
        //Act
        var actualResponse = await _sut.Handle(command, CancellationToken.None); 
        
        //Assert
        Assert.Equal(message.ImageUrl, actualResponse.Value.ImageUrl);
        Assert.Equal(message.UserId, actualResponse.Value.UserId);
    }

    [Fact]
    public async Task Handler_ShouldReturnError_WhenUserNotExists()
    {
        //Arrange
        var command = _fixture.Create<SaveImageCommand>();
        
        _unitOfWorkMock
            .Setup(u => 
                u.Users.UserExists(It.IsAny<string>()))
            .ReturnsAsync(false);
        
        //Act
        var actualResponse = await _sut.Handle(command, CancellationToken.None);

        //Assert
        Assert.Equal(Errors.User.UserNotFound, actualResponse.FirstError);
    }

    [Fact]
    public async Task Handler_ShouldReturnError_WhenRequestIsNotValid()
    {
        //Arrange
        var command = _fixture.Build<SaveImageCommand>()
            .With(r => r.ImageUrl, "")
            .Create();

        _unitOfWorkMock
            .Setup(u => 
                u.Users.UserExists(It.IsAny<string>()))
            .ReturnsAsync(true);

        //Act
        var actualResponse = await _sut.Handle(command, CancellationToken.None);
        
        //Assert
        Assert.Equal(ErrorType.Validation, actualResponse.FirstError.Type);
    }
}
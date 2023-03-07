using ChatApp.Application.Messages.Commands.UploadImage;
using ChatApp.Application.Common.Interfaces;
using ChatApp.Domain.Common.Errors;
using Microsoft.AspNetCore.Http;
using CloudinaryDotNet.Actions;
using Moq;

namespace ChatApp.Application.Tests.Messages.Commands;

public class UploadImageCommandHandlerTests 
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly UploadImageCommandHandler _sut;
    
    public UploadImageCommandHandlerTests()
    {
        _sut = new UploadImageCommandHandler(_unitOfWorkMock.Object);
    }
    
    [Fact]
    public async Task Handler_ShouldReturnUploadResult()
    {
        //Arrange
        var content = "Hello World from a Fake File";
        var fileName = "test.jpg";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        
        await writer.WriteAsync(content);
        await writer.FlushAsync();
        
        stream.Position = 0;
        
        IFormFile image = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);
        
        var uploadResult = new ImageUploadResult
        {
            PublicId = Guid.NewGuid().ToString(),
            Url = new Uri("https://test-image-url.com")
        };
        bool isAvatar = false;

        _unitOfWorkMock
            .Setup(u => 
                u.Messages.UploadImageToCloudinary(image, isAvatar))
            .ReturnsAsync(uploadResult);

        var command = new UploadImageCommand(image, isAvatar);

        //Act
        var actualResponse = await _sut.Handle(command, CancellationToken.None);
        
        //Assert
        Assert.Equal(uploadResult.Url, actualResponse.Value.Url);
    }
    
    [Fact]
    public async Task Handler_ShouldReturnError_WhenFileLengthLessThanOne()
    {
        //Arrange
        var content = "Hello World from a Fake File";
        var fileName = "test.jpg";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        
        await writer.WriteAsync(content);
        await writer.FlushAsync();
        
        stream.Position = 0;
        
        IFormFile image = new FormFile(stream, 0, 0, "id_from_form", fileName);
        bool isAvatar = false;

        var command = new UploadImageCommand(image, isAvatar);

        //Act
        var actualResponse = await _sut.Handle(command, CancellationToken.None);
        
        //Assert
        Assert.Equal(Errors.Message.ImageFileIsCorrupted, actualResponse.FirstError);
    }
    
    [Fact]
    public async Task Handler_ShouldReturnError_WhenCloudinaryReturnsError()
    {
        //Arrange
        var content = "Hello World from a Fake File";
        var fileName = "test.jpg";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        
        await writer.WriteAsync(content);
        await writer.FlushAsync();
        
        stream.Position = 0;
        
        IFormFile image = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);

        bool isAvatar = false;
        
        _unitOfWorkMock
            .Setup(u => 
                u.Messages.UploadImageToCloudinary(image, isAvatar))
            .ReturnsAsync(() => null);

        var command = new UploadImageCommand(image, isAvatar);
        
        //Act
        var actualResponse = await _sut.Handle(command, CancellationToken.None);
        
        //Assert
        Assert.Equal(Errors.Message.CantUploadImage, actualResponse.FirstError);
    }
}

using ChatApp.Application.Messages.Commands.UploadImage;
using ChatApp.Domain.Common.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CloudinaryDotNet.Actions;
using ChatApp.Api.Controllers;
using MapsterMapper;
using MediatR;
using Moq;

namespace ChatApp.Api.Tests.Controllers;

public class ImageControllerTests
{
    private readonly Mock<ISender> _mediatorMock = new();
    private readonly IMapper _mapper = MapsterConfigForTesting.GetMapper();
    private readonly ImageController _sut;
    
    public ImageControllerTests()
    {
        _sut = new ImageController(
            _mapper, 
            _mediatorMock.Object);
    }

    [Fact]
    public async Task UploadImage_ShouldReturnResponse()
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

        var command = new UploadImageCommand(image, isAvatar);

        _mediatorMock
            .Setup(m => m.Send(command, CancellationToken.None))
            .ReturnsAsync(uploadResult);
        
        //Act
        var actualResponse = await _sut.UploadImage(image, isAvatar.ToString());

        //Assert
        Assert.IsType<OkObjectResult>(actualResponse as OkObjectResult);
    }
    
    [Fact]
    public async Task UploadImage_ShouldReturnError_WhenFileIsNotValid()
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
        
        var command = new UploadImageCommand(image, isAvatar);

        _mediatorMock
            .Setup(m => m.Send(command, CancellationToken.None))
            .ReturnsAsync(Errors.Message.ImageFileIsCorrupted);
        
        //Act
        var actualResponse = await _sut.UploadImage(image, isAvatar.ToString());

        //Assert
        Assert.IsType<ObjectResult>(actualResponse as ObjectResult);
    }
}
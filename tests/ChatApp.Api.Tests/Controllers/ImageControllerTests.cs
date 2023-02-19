using ChatApp.Api.Controllers;
using ChatApp.Application.Services;
using ChatApp.Domain.Common.Errors;
using CloudinaryDotNet.Actions;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ChatApp.Api.Tests.Controllers;

public class ImageControllerTests
{
    private readonly Mock<IMessageService> _messageServiceMock = new();
    private readonly IMapper _mapper = MapsterConfigForTesting.GetMapper();
    private readonly ImageController _sut;
    
    public ImageControllerTests()
    {
        _sut = new ImageController(_messageServiceMock.Object, _mapper);
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
        
        _messageServiceMock
            .Setup(x => x.UploadImage(image))
            .ReturnsAsync(uploadResult);
        
        //Act
        var actualResponse = await _sut.UploadImage(image);

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

        _messageServiceMock
            .Setup(x => x.UploadImage(image))
            .ReturnsAsync(Errors.Message.ImageFileIsCorrupted);
        
        //Act
        var actualResponse = await _sut.UploadImage(image);

        //Assert
        Assert.IsType<ObjectResult>(actualResponse as ObjectResult);
    }
}
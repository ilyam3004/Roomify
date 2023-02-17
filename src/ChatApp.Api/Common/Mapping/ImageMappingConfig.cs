using ChatApp.Application.Models.Responses;
using ChatApp.Contracts.Rooms.Responses;
using ChatApp.Domain.Entities;
using CloudinaryDotNet.Actions;
using Mapster;

namespace ChatApp.Api.Common.Mapping;

public class ImageMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ImageUploadResult, UploadResultResponse>()
            .Map(dest => dest.ImgUrl, src => src.Url);
    }
}
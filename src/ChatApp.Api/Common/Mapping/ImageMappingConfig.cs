using ChatApp.Contracts.Rooms.Responses;
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
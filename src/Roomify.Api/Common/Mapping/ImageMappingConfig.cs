using CloudinaryDotNet.Actions;
using Mapster;
using Roomify.Contracts.Rooms.Responses;

namespace Roomify.Api.Common.Mapping;

public class ImageMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ImageUploadResult, UploadResultResponse>()
            .Map(dest => dest.ImgUrl, src => src.Url);
    }
}
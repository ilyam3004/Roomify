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
        
        config.NewConfig<(User, Room), UserResponse>()
            .Map(dest => dest.UserId, src => src.Item1.UserId)
            .Map(dest => dest.Username, src => src.Item1.Username)
            .Map(dest => dest.ConnectionId, src => src.Item1.ConnectionId)
            .Map(dest => dest.RoomId, src => src.Item2.RoomId)
            .Map(dest => dest.RoomName, src => src.Item2.RoomName);
    }
}
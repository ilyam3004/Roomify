using ChatApp.Application.Models.Responses;
using ChatApp.Domain.Entities;
using Mapster;

namespace ChatApp.Api.Common.Mapping;

public class MessageMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<(Message, User), MessageResponse>()
            .Map(dest => dest.MessageId, src => src.Item1.MessageId)
            .Map(dest => dest.Username, src => src.Item2.Username)
            .Map(dest => dest.UserId, src => src.Item2.UserId)
            .Map(dest => dest.Avatar, src => src.Item2.Avatar)
            .Map(dest => dest.RoomId, src => src.Item2.RoomId)
            .Map(dest => dest.Text, src => src.Item1.Text)
            .Map(dest => dest.Date, src => src.Item1.Date)
            .Map(dest => dest.FromUser, src => src.Item1.FromUser)
            .Map(dest => dest.IsImage, src => src.Item1.IsImage)
            .Map(dest => dest.ImageUrl, src => src.Item1.ImageUrl);
    }
}
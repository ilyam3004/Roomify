using ChatApp.Application.Users.Commands.JoinRoom;
using ChatApp.Contracts.Rooms;
using Mapster;

namespace ChatApp.Api.Common.Mapping;

public class JoinRoomCommandMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<(JoinRoomRequest, string), JoinRoomCommand>()
            .Map(dest => dest.Username, src => src.Item1.Username)
            .Map(dest => dest.RoomName, src => src.Item1.RoomName)
            .Map(dest => dest.Avatar, src => src.Item1.Avatar)
            .Map(dest => dest.ConnectionId, src => src.Item2);
    }
}
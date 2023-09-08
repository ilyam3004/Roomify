using Mapster;
using Roomify.Application.Users.Commands.JoinRoom;
using Roomify.Contracts.Rooms.Requests;

namespace Roomify.Api.Common.Mapping;

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
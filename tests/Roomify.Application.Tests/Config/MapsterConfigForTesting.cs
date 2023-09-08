using Mapster;
using MapsterMapper;
using Roomify.Api.Common.Mapping;

namespace ChatApp.Application.Tests.Config;

public static class MapsterConfigForTesting
{
    public static Mapper GetMapper()
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(typeof(UserMappingConfig).Assembly);
        config.Scan(typeof(MessageMappingConfig).Assembly);
        return new Mapper(config);
    }
}
using ChatApp.Api.Common.Mapping;
using MapsterMapper;
using Mapster;

namespace ChatApp.Api.Tests;

public static class MapsterConfigForTesting
{
    public static Mapper GetMapper()
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(typeof(ImageMappingConfig).Assembly);
        return new Mapper(config);
    }
}


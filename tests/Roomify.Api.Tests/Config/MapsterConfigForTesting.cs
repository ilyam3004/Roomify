using Mapster;
using MapsterMapper;
using Roomify.Api.Common.Mapping;

namespace Roomify.Api.Tests.Config;

public static class MapsterConfigForTesting
{
    public static Mapper GetMapper()
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(typeof(ImageMappingConfig).Assembly);
        return new Mapper(config);
    }
}


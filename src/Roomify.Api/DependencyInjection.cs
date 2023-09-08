using Roomify.Api.Common.Mapping;

namespace Roomify.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddMappings()
            .AddCors()
            .AddSwaggerGen()
            .AddControllers();
        
        services.AddCors()
            .AddSignalR();
        
        return services;
    }
}
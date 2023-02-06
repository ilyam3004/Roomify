using Microsoft.Extensions.DependencyInjection;
using ChatApp.Application.Common.Interfaces.Persistence;
using ChatApp.Infrastructure.Persistence;
using ChatApp.Infrastructure.Config;
using Microsoft.AspNetCore.Builder;

namespace ChatApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();

        services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));

        services.AddSingleton<AppDbContext>();
        
        return services;
    }
}
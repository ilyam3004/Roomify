using Microsoft.Extensions.DependencyInjection;
using ChatApp.Application.Common.Interfaces.Persistence;
using ChatApp.Infrastructure.Persistence;
using ChatApp.Infrastructure.Config;

namespace ChatApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();

        services.AddSingleton<AppDbContext>();
        
        return services;
    }
}
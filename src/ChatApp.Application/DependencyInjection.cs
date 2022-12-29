using Microsoft.Extensions.DependencyInjection;
using ChatApp.Application.Services;

namespace ChatApp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IMessageService, MessageService>();
        return services;
    }
}
using ChatApp.Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using ChatApp.Application.Services;
using FluentValidation;

namespace ChatApp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddValidatorsFromAssemblyContaining<IAssemblyMarker>();
        return services;
    }
}
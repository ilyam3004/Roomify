using ChatApp.Application.Messages.Commands.SaveMessage;
using ChatApp.Application.Messages.Commands.SaveImage;
using ChatApp.Application.Users.Commands.JoinRoom;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;
using MediatR;

namespace ChatApp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        services.AddMediatR(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(assembly);
        
        return services;
    }
}
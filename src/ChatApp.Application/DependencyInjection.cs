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
        services.AddMediatR(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssemblyContaining<JoinRoomCommandValidator>();
        services.AddValidatorsFromAssemblyContaining<SaveMessageCommandValidator>();
        services.AddValidatorsFromAssemblyContaining<SaveImageCommandValidator>();
        return services;
    }
}
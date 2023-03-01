using Microsoft.Extensions.DependencyInjection;
using ChatApp.Application.Common.Validations;
using System.Reflection;
using FluentValidation;
using MediatR;
using ChatApp.Application.Services;
using ChatApp.Application.Users.Commands.JoinRoom;

namespace ChatApp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(Assembly.GetExecutingAssembly());
        services.AddScoped<IMessageService, MessageService>();
        services.AddValidatorsFromAssemblyContaining<JoinRoomCommandValidator>();
        services.AddValidatorsFromAssemblyContaining<SaveImageRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<SaveMessageRequestValidator>();
        return services;
    }
}
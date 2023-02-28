using Microsoft.Extensions.DependencyInjection;
using ChatApp.Application.Common.Validations;
using System.Reflection;
using FluentValidation;
using MediatR;
using ChatApp.Application.Users.JoinRoom;

namespace ChatApp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg
            .RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
        services.AddValidatorsFromAssemblyContaining<JoinRoomCommandValidator>();
        services.AddValidatorsFromAssemblyContaining<SaveImageRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<SaveMessageRequestValidator>();
        return services;
    }
}
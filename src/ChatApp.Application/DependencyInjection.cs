using ChatApp.Application.Common.Interfaces;
using ChatApp.Application.Common.Validations;
using ChatApp.Application.Models.Requests;
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
        services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<SaveImageRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<SaveMessageRequestValidator>();
        return services;
    }
}
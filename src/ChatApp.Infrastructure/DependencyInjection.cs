using ChatApp.Application.Common.Interfaces.Persistence;
using ChatApp.Infrastructure.Interfaces.Persistence;
using Microsoft.Extensions.DependencyInjection;
using ChatApp.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using ChatApp.Infrastructure.Interfaces;
using ChatApp.Infrastructure.Config;
using Microsoft.AspNetCore.Builder;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace ChatApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        WebApplicationBuilder builder)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddTransient<IUnitOfWork, UnitOfWork>();

        services.Configure<CloudinarySettings>(
            builder.Configuration.GetSection("Cloudinary"));
        
        services.AddTransient<IDbConnection>(
            sp => new SqlConnection(builder
                .Configuration.GetConnectionString("SqlConnection")!));

        return services;
    }
}
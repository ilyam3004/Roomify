using System.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Roomify.Application.Common.Interfaces;
using Roomify.Application.Common.Interfaces.Persistence;
using Roomify.Infrastructure.Config;
using Roomify.Infrastructure.Interfaces;
using Roomify.Infrastructure.Interfaces.Persistence;

namespace Roomify.Infrastructure;

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
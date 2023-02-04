using ChatApp.Api.Hubs;
using ChatApp.Application;
using ChatApp.Infrastructure;
using Microsoft.AspNetCore.Cors.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services
    .AddInfrastructure()
    .AddApplication()
    .AddControllers();
    
    builder.Services
        .AddCors()
        .AddSignalR();
}

var app = builder.Build();
{
    app.UseCors(builder => builder
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowCredentials()
            .AllowAnyMethod()
            .WithOrigins("http://localhost:3000"));
    app.MapHub<ChatHub>("/chatHub");
    app.Run();
}
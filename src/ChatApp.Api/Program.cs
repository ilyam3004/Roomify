using ChatApp.Api.Hubs;
using ChatApp.Application;
using ChatApp.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services
    .AddInfrastructure()
    .AddApplication()
    .AddControllers();
    
    builder.Services
        .AddCors()
        .AddSignalR()
        .AddAzureSignalR("Endpoint=https://chat-app-server.service.signalr.net;AccessKey=NBgFpQSN4/PM4qewDJx8f0ujMbIoteI6zInoRhAhxNw=;Version=1.0;");
}

var app = builder.Build();
{
    app.UseCors(builder => builder
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowCredentials()
            .AllowAnyMethod()
            .WithOrigins("http://localhost:3000"));
    
    app.UseAzureSignalR(routes =>
    {
        routes.MapHub<ChatHub>("/chatHub");
    });
    app.Run();
}
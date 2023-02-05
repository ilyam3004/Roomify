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
        .AddSignalR();
}

var app = builder.Build();
{
    app.UseCors(builder => builder
        .AllowAnyOrigin()
        .AllowCredentials()
        .AllowAnyHeader()
        .AllowAnyMethod()
        .WithOrigins("https://chat-app-psi-ruby.vercel.app"));
    
    app.MapHub<ChatHub>("/chatHub");
    app.Run();
}
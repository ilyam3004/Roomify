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
    app.UseHttpsRedirection();
    app.UseCors(builder => builder
        .WithOrigins("null")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
        .AllowAnyOrigin());
    app.MapHub<ChatHub>("/chatHub");
    app.Run();
}
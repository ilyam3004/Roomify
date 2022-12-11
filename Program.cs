using ChatAppServer.Hubs;
using ChatAppServer.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services
        .AddSingleton<Dictionary<string, UserConnection>>()
        .AddCors()
        .AddSignalR();

var app = builder.Build();
{
    app.UseCors(builder => builder
               .WithOrigins("null")
               .AllowAnyHeader()
               .SetIsOriginAllowed((host) => true)
               .AllowAnyMethod()
               .AllowCredentials());
    app.MapHub<ChatHub>("/chatHub");
    app.Run();
};

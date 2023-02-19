using ChatApp.Infrastructure;
using ChatApp.Application;
using ChatApp.Api.Hubs;
using ChatApp.Api;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services
        .AddPresentation()
        .AddInfrastructure(builder)
        .AddApplication();
}

var app = builder.Build();
{
    app.UseCors(builder => builder
        .WithOrigins("null")
        .AllowAnyHeader()
        .SetIsOriginAllowed((host) => true)
        .AllowAnyMethod()
        .AllowCredentials());

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            options.RoutePrefix = string.Empty;
        });
    }

    app.MapHub<ChatHub>("/chatHub");
    app.MapControllers();
    app.Run();
}
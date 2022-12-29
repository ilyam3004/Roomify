using ChatApp.Application;
using ChatApp.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services
    .AddInfrastructure()
    .AddApplication()
    .AddControllers();
}

var app = builder.Build();
{
    app.UseHttpsRedirection();
    app.MapControllers();
    app.Run();
}
using HR.API.Auth;
using HR.API.Middleware;
using DotNetEnv;
using HR.Application;
using HR.Infrastructure;
using HR.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

// Find the nearest .env in this directory or its parents; explicit environment settings win.
Env.NoClobber().TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<EmployeeIdentityFilter>();
builder.Services.AddScoped<HrRoleFilter>();
builder.Services.AddScoped<InternalEmployeeDirectoryFilter>();

var app = builder.Build();

// Finish schema initialization before the contract job or HTTP requests can use the database.
if (app.Configuration.GetValue<bool>("DatabaseInitialization:MigrateOnStartup"))
{
    await using var scope = app.Services.CreateAsyncScope();
    var database = scope.ServiceProvider.GetRequiredService<HrDbContext>();
    app.Logger.LogInformation("Đang tạo/cập nhật database HR bằng các migration hiện có.");
    await database.Database.MigrateAsync(app.Lifetime.ApplicationStopping);
    app.Logger.LogInformation("Database HR đã sẵn sàng.");
}

app.UseMiddleware<ApiExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();
app.MapRazorPages();
app.Run();

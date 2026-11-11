using HR.API.Auth;
using HR.API.Middleware;
using DotNetEnv;
using HR.Application;
using HR.Infrastructure;

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

app.UseMiddleware<ApiExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();
app.MapRazorPages();
app.Run();

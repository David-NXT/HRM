using HR.Core.Interfaces;
using HR.Core.Services;
using HR.Infrastructure.Auth;
using HR.Infrastructure.Data;
using HR.Infrastructure.Middleware;
using HR.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("HrDatabase");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Missing ConnectionStrings:HrDatabase.");
}

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddDbContext<HrDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<HrRoleFilter>();

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

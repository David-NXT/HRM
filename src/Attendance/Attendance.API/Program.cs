using Attendance.Data;
using Attendance.Services;
using DotNetEnv;
using HR.Core.Interfaces;
using HR.Core.Services;
using HR.Infrastructure.Auth;
using HR.Infrastructure.Data;
using HR.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

static string? FindEnvFile()
{
    var startDirectories = new[]
    {
        Directory.GetCurrentDirectory(),
        AppContext.BaseDirectory
    };

    foreach (var startDirectory in startDirectories)
    {
        var directory = new DirectoryInfo(startDirectory);

        while (directory != null)
        {
            var envPath = Path.Combine(directory.FullName, ".env");

            if (File.Exists(envPath))
            {
                return envPath;
            }

            directory = directory.Parent;
        }
    }

    return null;
}

var envPath = FindEnvFile();
if (!string.IsNullOrWhiteSpace(envPath))
{
    Env.Load(envPath);
}

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

var connectionString = builder.Configuration.GetConnectionString("AttendanceDb");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Missing AttendanceDB connection string. Set ConnectionStrings__AttendanceDb in environment or .env");
}

builder.Services.AddDbContext<AttendanceDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IAttendanceExplanationsService, AttendanceExplanationsService>();
builder.Services.AddScoped<IHolidayService, HolidayService>();
builder.Services.AddScoped<IWorkShiftService, WorkShiftService>();
builder.Services.AddScoped<IShiftAssignmentService, ShiftAssignmentService>();
builder.Services.AddScoped<IOTRequestService, OTRequestService>();
builder.Services.AddScoped<ILeaveRequestService, LeaveRequestService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddDbContext<HrDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<HrRoleFilter>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();
app.MapGet("/", () => "Attendance API running");

app.Run();

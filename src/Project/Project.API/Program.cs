using Microsoft.EntityFrameworkCore;
using Project.API.Data;
using Project.API.Services;
using DotNetEnv;


Env.Load("../.env");

var builder = WebApplication.CreateBuilder(args);

// PostgreSQL connection
var connectionString =
    $"Host={Environment.GetEnvironmentVariable("DB_HOST")};" +
    $"Port={Environment.GetEnvironmentVariable("DB_PORT")};" +
    $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
    $"Username={Environment.GetEnvironmentVariable("DB_USER")};" +
    $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")}";

// Register DbContext
builder.Services.AddDbContext<ProjectManagementDbContext>(options =>
    options.UseNpgsql(connectionString));

// Add services to the container
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IProjectMemberService, ProjectMemberService>();
builder.Services.AddScoped<IProjectRoleService, ProjectRoleService>();
builder.Services.AddScoped<IHourlyRateService, HourlyRateService>();
builder.Services.AddScoped<ITimesheetService, TimesheetService>();
builder.Services.AddScoped<IPayrollDataService, PayrollDataService>();
var hrBaseUrl = builder.Configuration["Services:HR:BaseUrl"] ?? "http://localhost:5012/";
builder.Services.AddHttpClient<IHrEmployeeDirectoryClient, HrEmployeeDirectoryClient>(client =>
{
    client.BaseAddress = new Uri(hrBaseUrl);
    client.DefaultRequestHeaders.Add("X-Role", "HR");
});

var app = builder.Build();

// Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

app.MapControllers();

app.Run();


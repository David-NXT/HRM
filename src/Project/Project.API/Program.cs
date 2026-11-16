using Microsoft.EntityFrameworkCore;
using Project.API.Data;
using Project.API.Services;
using DotNetEnv;
using Npgsql;


Env.NoClobber().TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

// PostgreSQL connection
var connectionString =
    $"Host={Environment.GetEnvironmentVariable("DB_HOST")};" +
    $"Port={Environment.GetEnvironmentVariable("DB_PORT")};" +
    $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
    $"Username={Environment.GetEnvironmentVariable("DB_USER")};" +
    $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")}";

var connectionBuilder =
    new NpgsqlConnectionStringBuilder(connectionString);
var databaseName = connectionBuilder.Database;

if (string.IsNullOrWhiteSpace(databaseName))
{
    throw new InvalidOperationException(
        "DB_NAME is missing. Set it in the environment or .env file."
    );
}

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
var attendanceBaseUrl = builder.Configuration["Services:Attendance:BaseUrl"] ?? "http://localhost:5231/";
builder.Services.AddHttpClient<IAttendanceWorkHoursClient, AttendanceWorkHoursClient>(client =>
{
    client.BaseAddress = new Uri(attendanceBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(10);
});
var hrBaseUrl = builder.Configuration["Services:HR:BaseUrl"] ?? "http://localhost:5012/";
builder.Services.AddHttpClient<IHrEmployeeDirectoryClient, HrEmployeeDirectoryClient>(client =>
{
    client.BaseAddress = new Uri(hrBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(10);
    var directoryKey = builder.Configuration["Services:HR:ApiKey"];
    if (!string.IsNullOrWhiteSpace(directoryKey))
        client.DefaultRequestHeaders.Add("X-HR-Directory-Key", directoryKey);
    else
        client.DefaultRequestHeaders.Add("X-Role", "HR");
});

var app = builder.Build();

var databaseConnectionBuilder =
    new NpgsqlConnectionStringBuilder(connectionString)
    {
        Database = "postgres"
    };

await using (var connection =
    new NpgsqlConnection(databaseConnectionBuilder.ConnectionString))
{
    await connection.OpenAsync();

    await using var command = connection.CreateCommand();
    command.CommandText = """
        SELECT EXISTS (
            SELECT 1
            FROM pg_database
            WHERE datname = @databaseName
        );
        """;
    command.Parameters.AddWithValue("databaseName", databaseName);

    var databaseExists =
        (bool)(await command.ExecuteScalarAsync() ?? false);

    if (!databaseExists)
    {
        command.Parameters.Clear();
        var safeDatabaseName =
            "\"" + databaseName.Replace("\"", "\"\"") + "\"";
        command.CommandText = $"CREATE DATABASE {safeDatabaseName};";
        await command.ExecuteNonQueryAsync();

        app.Logger.LogInformation(
            "Project database {DatabaseName} created successfully.",
            databaseName);
    }
    else
    {
        app.Logger.LogInformation(
            "Project database {DatabaseName} already exists.",
            databaseName);
    }
}

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext =
        scope.ServiceProvider.GetRequiredService<ProjectManagementDbContext>();
    await dbContext.Database.MigrateAsync();
    app.Logger.LogInformation("Project database migrations completed.");
}

// Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

app.MapControllers();

app.Run();


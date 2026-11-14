using Attendance.Data;
using Attendance.Services;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Npgsql;

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
                return envPath;

            directory = directory.Parent;
        }
    }

    return null;
}

// 1. Load file .env
var envPath = FindEnvFile();

if (!string.IsNullOrWhiteSpace(envPath))
{
    Env.Load(envPath);
}

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

// 2. Get connection string
var connectionString = builder.Configuration
    .GetConnectionString("AttendanceDb");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Missing AttendanceDB connection string. " +
        "Set ConnectionStrings__AttendanceDb in environment or .env"
    );
}

// 3. Register DbContext
builder.Services.AddDbContext<AttendanceDbContext>(options =>
    options.UseNpgsql(connectionString));

// 4. Register Services
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IAttendanceExplanationsService, AttendanceExplanationsService>();
builder.Services.AddScoped<IHolidayService, HolidayService>();
builder.Services.AddScoped<IWorkShiftService, WorkShiftService>();
builder.Services.AddScoped<IShiftAssignmentService, ShiftAssignmentService>();
builder.Services.AddScoped<IOTRequestService, OTRequestService>();
builder.Services.AddScoped<ILeaveRequestService, LeaveRequestService>();
builder.Services.AddScoped<ITimesheetService, TimesheetService>();
builder.Services.AddScoped<ILeaveBalanceService, LeaveBalanceService>();
builder.Services.AddScoped<IOvertimeRuleService, OvertimeRuleService>();
builder.Services.AddScoped<IAttendanceRuleService, AttendanceRuleService>();
builder.Services.AddScoped<IAttendanceConfigAuditLogService, AttendanceConfigAuditLogService>();
builder.Services.AddScoped<IAttendanceAnalyticsService, AttendanceAnalyticsService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();
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

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (EmployeeNotAvailableException exception) when (!context.Response.HasStarted)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(new { message = exception.Message });
    }
    catch (HttpRequestException) when (!context.Response.HasStarted)
    {
        context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
        await context.Response.WriteAsJsonAsync(new { message = "HR employee directory is unavailable." });
    }
});

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (EmployeeNotAvailableException exception) when (!context.Response.HasStarted)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(new { message = exception.Message });
    }
    catch (HttpRequestException) when (!context.Response.HasStarted)
    {
        context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
        await context.Response.WriteAsJsonAsync(new { message = "HR employee directory is unavailable." });
    }
});

// 5. Automatically create Database if it does not exist
var connectionBuilder =
    new NpgsqlConnectionStringBuilder(connectionString);

var databaseName = connectionBuilder.Database;

if (string.IsNullOrWhiteSpace(databaseName))
{
    throw new InvalidOperationException(
        "Database name is missing in connection string."
    );
}

// Connect to PostgreSQL maintenance database
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

    var exists = (bool)(await command.ExecuteScalarAsync() ?? false);

    if (!exists)
    {
        command.Parameters.Clear();

        var safeDatabaseName =
            "\"" + databaseName.Replace("\"", "\"\"") + "\"";

        command.CommandText =
            $"CREATE DATABASE {safeDatabaseName};";

        await command.ExecuteNonQueryAsync();

        Console.WriteLine(
            $"Database '{databaseName}' created successfully."
        );
    }
    else
    {
        Console.WriteLine(
            $"Database '{databaseName}' already exists."
        );
    }
}

// 6. Automatically apply migrations
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<AttendanceDbContext>();

    await dbContext.Database.MigrateAsync();

    Console.WriteLine("Attendance database migrations completed.");
}

// 7. Configure application
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.MapGet("/", () => "Attendance API running");

app.Run();

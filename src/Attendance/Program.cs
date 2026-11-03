using Attendance.Data;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Npgsql;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Register controllers
builder.Services.AddControllers();

// Load PostgreSQL configuration from .env
var connectionString = new NpgsqlConnectionStringBuilder
{
    Host = Environment.GetEnvironmentVariable("DB_HOST")
        ?? "localhost",

    Port = int.TryParse(
        Environment.GetEnvironmentVariable("DB_PORT"),
        out var port) ? port : 5432,

    Database = Environment.GetEnvironmentVariable("DB_NAME")
        ?? throw new InvalidOperationException("DB_NAME is missing"),

    Username = Environment.GetEnvironmentVariable("DB_USER")
        ?? throw new InvalidOperationException("DB_USER is missing"),

    Password = Environment.GetEnvironmentVariable("DB_PASSWORD")
        ?? throw new InvalidOperationException("DB_PASSWORD is missing")
}.ConnectionString;

// Register PostgreSQL DbContext
builder.Services.AddDbContext<AttendanceDbContext>(options =>
    options.UseNpgsql(connectionString));

// OpenAPI
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

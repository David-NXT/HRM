using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Payroll.Application;
using Payroll.Infrastructure;
using Payroll.Infrastructure.Persistence;

static string FindEnvFile()
{
    // check current path or app path
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
            // current path concat /.env
            var envPath = Path.Combine(directory.FullName, ".env");

            if (File.Exists(envPath))
            {
                return envPath;
            }

            // Backtrack from current folder
            directory = directory.Parent;
        }
    }
    throw new FileNotFoundException("Cannot find .env in the current directory or its parent directories.");
}
var envPath = FindEnvFile();

Env.Load(envPath);

var builder = WebApplication.CreateBuilder(args); 

var connectionString = builder.Configuration.GetConnectionString("PayrollConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("PayrollConnection connection string is missing.");
}

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Auto seed default and sample data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PayrollDbContext>();

    await context.Database.MigrateAsync();

    await PayrollDataSeeder.SeedAsync(context);
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
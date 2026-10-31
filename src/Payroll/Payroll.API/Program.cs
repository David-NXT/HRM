using DotNetEnv;

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
            var envPath = Path.Combine(directory.FullName,".env");

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
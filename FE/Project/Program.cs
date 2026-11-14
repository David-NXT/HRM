using Project.Config;
using Project.Services;

namespace Project
{
    internal static class Program
    {
        public static ProjectApiService ProjectApi { get; } = new(
            new HttpClient { BaseAddress = new Uri(ApiConfig.ProjectServiceUrl) });

        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}
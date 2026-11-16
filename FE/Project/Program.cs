using Project.Config;
using Project.Services;

namespace Project
{
    internal static class Program
    {
        public static ProjectApiService ProjectApi { get; } = new(
            new HttpClient
            {
                BaseAddress = new Uri(ApiConfig.ProjectServiceUrl),
                Timeout = TimeSpan.FromSeconds(30)
            });

        public static AttendanceApiService AttendanceApi { get; } = new(
            new HttpClient
            {
                BaseAddress = new Uri(ApiConfig.AttendanceServiceUrl),
                Timeout = TimeSpan.FromSeconds(15)
            });

        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}
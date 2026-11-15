using System;
using System.Net.Http;
using System.Windows.Forms;
using Attendance.WinForms.Config;
using Attendance.WinForms.Services;

namespace Attendance.WinForms
{
    internal static class Program
    {
        public static AttendanceApiService AttendanceApi { get; private set; } = null!;

        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            var attendanceClient = new HttpClient
            {
                BaseAddress = new Uri(ApiConfig.AttendanceServiceUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };

            AttendanceApi = new AttendanceApiService(attendanceClient);

            Application.Run(new MainForm());
        }
    }
}


using Admin.WinForms.Config;
using Admin.WinForms.Services.Attendance;
using Admin.WinForms.Services.Payroll;

namespace Admin.WinForms
{
    internal static class Program
    {
        public static AttendanceApiService AttendanceApi { get; private set; } = null!;
        public static PayrollApiService PayrollApi { get; private set; } = null!;

        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            var attendanceClient = new HttpClient
            {
                BaseAddress = new Uri(ApiConfig.AttendanceServiceUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };

            var payrollClient = new HttpClient
            {
                BaseAddress = new Uri(ApiConfig.PayrollServiceUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };

            AttendanceApi = new AttendanceApiService(attendanceClient);
            PayrollApi = new PayrollApiService(payrollClient);

            Application.Run(new MainForm());
        }
    }
}

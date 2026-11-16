
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

            var payrollServiceUrl = Environment.GetEnvironmentVariable("Services__Payroll__BaseUrl");
            if (string.IsNullOrWhiteSpace(payrollServiceUrl))
            {
                payrollServiceUrl = ApiConfig.PayrollServiceUrl;
            }

            if (!Uri.TryCreate(payrollServiceUrl, UriKind.Absolute, out var payrollUri)
                || payrollUri.Scheme is not ("http" or "https"))
            {
                throw new InvalidOperationException(
                    "Payroll API URL không hợp lệ. Kiểm tra Services__Payroll__BaseUrl.");
            }

            var payrollClient = new HttpClient
            {
                BaseAddress = new Uri(payrollUri.AbsoluteUri.TrimEnd('/') + "/"),
                Timeout = TimeSpan.FromSeconds(30)
            };

            AttendanceApi = new AttendanceApiService(attendanceClient);
            PayrollApi = new PayrollApiService(payrollClient);

            Application.Run(new MainForm());
        }
    }
}

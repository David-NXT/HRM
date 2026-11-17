
using Admin.WinForms.DAO.Config;
using Admin.WinForms.DAO.Payroll;
using Admin.WinForms.DAO.Attendance;
using Admin.WinForms.GUI;
using Admin.WinForms.BUS.Presenters.Payroll;
using Admin.WinForms.BUS.Services.Payroll;

namespace Admin.WinForms
{
    internal static class Program
    {
        public static AttendanceApiService AttendanceApi { get; private set; } = null!;
        private static PayrollPeriodService PayrollPeriods { get; set; } = null!;
        private static PayrollManagementService PayrollManagement { get; set; } = null!;
        private static PayrollCatalogService PayrollCatalog { get; set; } = null!;
        private static PayslipService Payslips { get; set; } = null!;
        public static PayrollPresenter PayrollPresenter { get; private set; } = null!;

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
            var payrollApiClient = new PayrollApiClient(payrollClient);
            PayrollPeriods = new PayrollPeriodService(payrollApiClient);
            Payslips = new PayslipService(payrollApiClient, PayrollPeriods);
            PayrollManagement = new PayrollManagementService(
                payrollApiClient,
                PayrollPeriods,
                Payslips);
            PayrollCatalog = new PayrollCatalogService(payrollApiClient);
            PayrollPresenter = new PayrollPresenter(
                PayrollPeriods,
                PayrollManagement,
                PayrollCatalog,
                Payslips);

            Application.Run(new MainForm());
        }
    }
}

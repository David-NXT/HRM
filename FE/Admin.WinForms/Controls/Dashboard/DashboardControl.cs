using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Admin.WinForms.DTOs.Attendance;
using Admin.WinForms.DTOs.Payroll;
using Admin.WinForms.Models.HR;
using Admin.WinForms.Services.HR;
using Admin.WinForms.Services.Project;

namespace Admin.WinForms.Controls.Dashboard
{
    public partial class DashboardControl : UserControl
    {
        private readonly CultureInfo _vnCulture = new("vi-VN");
        private CancellationTokenSource? _cts;

        public DashboardControl()
        {
            InitializeComponent();

            button_Refresh.Click += async (s, e) =>
            {
                await Program.PayrollManagement.RefreshPayrollDataAsync();
                await LoadDashboardDataAsync();
            };

            btnNav_HR.Click += (s, e) => (FindForm() as MainForm)?.NavigateTo("hr");
            btnNav_Attendance.Click += (s, e) => (FindForm() as MainForm)?.NavigateTo("attendance");
            btnNav_Payroll.Click += (s, e) => (FindForm() as MainForm)?.NavigateTo("payroll");
            btnNav_Project.Click += (s, e) => (FindForm() as MainForm)?.NavigateTo("project");

            Load += async (s, e) => await LoadDashboardDataAsync();
        }

        private async Task LoadDashboardDataAsync()
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            button_Refresh.Enabled = false;
            button_Refresh.Text = "⏳ Đang tải...";

            try
            {
                // Chạy đồng thời các tác vụ tải dữ liệu
                var hrTask = LoadHrKpiAsync(token);
                var attTask = LoadAttendanceKpiAsync(token);
                var payrollTask = LoadPayrollKpiAsync(token);
                var projTask = LoadProjectKpiAsync(token);
                var healthTask = CheckServicesHealthAsync(token);

                await Task.WhenAll(hrTask, attTask, payrollTask, projTask, healthTask);

                label_LastUpdated.Text = $"Cập nhật lúc: {DateTime.Now:HH:mm:ss}";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Dashboard] Error loading data: {ex.Message}");
            }
            finally
            {
                button_Refresh.Enabled = true;
                button_Refresh.Text = "🔄 Làm mới";
            }
        }

        private async Task LoadAttendanceKpiAsync(CancellationToken token)
        {
            try
            {
                var today = DateOnly.FromDateTime(DateTime.Today);
                var allRecords = await Program.AttendanceApi.GetAllAsync();
                token.ThrowIfCancellationRequested();

                var todayRecords = allRecords.Where(r => r.Date == today).ToList();
                int presentCount = todayRecords.Count(r => r.Status.Equals("PRESENT", StringComparison.OrdinalIgnoreCase));
                int lateCount = todayRecords.Count(r => r.IsLate || r.LateMins > 0);
                int totalToday = todayRecords.Count;

                label_KpiAttVal.Text = totalToday > 0 ? $"{presentCount} / {totalToday}" : "0 / 0";
                label_KpiAttSub.Text = $"Có mặt: {presentCount} | Đi muộn: {lateCount}";

                // Tải danh sách đơn giải trình và đơn OT cần phê duyệt
                var pendingItems = new List<(string Type, string EmpId, string Date, string Reason, string Status)>();

                try
                {
                    var explanations = await Program.AttendanceApi.GetExplanationsAsync();
                    foreach (var exp in explanations.Where(x => x.Status.Equals("PENDING", StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(x.Status)))
                    {
                        pendingItems.Add(($"[Giải trình] {exp.ExplanationType}", exp.EmployeeId.ToString()[..8] + "...", exp.Date.ToString("dd/MM/yyyy"), exp.Reason, exp.Status));
                    }
                }
                catch { }

                try
                {
                    var otRequests = await Program.AttendanceApi.GetOTRequestsAsync();
                    foreach (var ot in otRequests.Where(x => x.Status.Equals("PENDING", StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(x.Status)))
                    {
                        pendingItems.Add(("[Tăng ca] OT", ot.EmployeeId.ToString()[..8] + "...", ot.Date.ToString("dd/MM/yyyy"), $"{ot.Reason} ({ot.PlannedHours}h)", ot.Status));
                    }
                }
                catch { }

                token.ThrowIfCancellationRequested();

                listView_Pending.BeginUpdate();
                listView_Pending.Items.Clear();

                foreach (var item in pendingItems.Take(25))
                {
                    var lvi = new ListViewItem(item.Type);
                    lvi.SubItems.Add(item.EmpId);
                    lvi.SubItems.Add(item.Date);
                    lvi.SubItems.Add(string.IsNullOrWhiteSpace(item.Reason) ? "--" : item.Reason);
                    lvi.SubItems.Add("Chờ phê duyệt");

                    lvi.ForeColor = Color.FromArgb(180, 83, 9); // Màu cam cảnh báo
                    listView_Pending.Items.Add(lvi);
                }

                if (listView_Pending.Items.Count == 0)
                {
                    var emptyItem = new ListViewItem("Không có đơn chờ duyệt");
                    emptyItem.SubItems.Add("--");
                    emptyItem.SubItems.Add(DateTime.Now.ToString("dd/MM/yyyy"));
                    emptyItem.SubItems.Add("Tất cả yêu cầu đã được xử lý.");
                    emptyItem.SubItems.Add("Đã xử lý");
                    emptyItem.ForeColor = Color.FromArgb(22, 101, 52);
                    listView_Pending.Items.Add(emptyItem);
                }

                listView_Pending.EndUpdate();
            }
            catch (Exception ex)
            {
                label_KpiAttVal.Text = "--";
                label_KpiAttSub.Text = "Không thể kết nối Attendance";
                System.Diagnostics.Debug.WriteLine($"[Dashboard] Attendance KPI Error: {ex.Message}");
            }
        }

        private async Task LoadPayrollKpiAsync(CancellationToken token)
        {
            try
            {
                var today = DateTime.Today;
                var summary = await Program.PayrollManagement.GetPaidPayrollSummaryForMonthAsync(today.Year, today.Month);
                token.ThrowIfCancellationRequested();

                label_KpiPayVal.Text = $"{summary.TotalNetSalary.ToString("N0", _vnCulture)} đ";
                label_KpiPaySub.Text = summary.PaidPayrollCount == 0
                    ? $"Chưa ghi nhận chi trả tháng {today:MM/yyyy}"
                    : $"{summary.PaidPayrollCount} phiếu đã chi trong {today:MM/yyyy}";
            }
            catch (Exception ex)
            {
                label_KpiPayVal.Text = "--";
                label_KpiPaySub.Text = "Không thể tải dữ liệu chi trả";
                System.Diagnostics.Debug.WriteLine($"[Dashboard] Payroll KPI Error: {ex.Message}");
            }
        }

        private async Task LoadHrKpiAsync(CancellationToken token)
        {
            try
            {
                using var hrClient = new HrApiClient();
                var employees = await hrClient.AllEmployeesAsync(token);
                int activeCount = employees.Count(e => e.Status != "RESIGNED");

                label_KpiEmpVal.Text = activeCount.ToString();
                label_KpiEmpSub.Text = $"{activeCount} Nhân sự đang hoạt động";
            }
            catch
            {
                // Fallback nếu HR Service chưa có endpoint: Đếm số nhân viên từ danh sách điểm danh
                try
                {
                    var records = await Program.AttendanceApi.GetAllAsync();
                    int distinctEmp = records.Select(r => r.EmployeeId).Distinct().Count();
                    label_KpiEmpVal.Text = distinctEmp.ToString();
                    label_KpiEmpSub.Text = $"{distinctEmp} Nhân sự ghi nhận trên hệ thống";
                }
                catch
                {
                    label_KpiEmpVal.Text = "--";
                    label_KpiEmpSub.Text = "Dịch vụ HR chưa sẵn sàng";
                }
            }
        }

        private async Task LoadProjectKpiAsync(CancellationToken token)
        {
            try
            {
                var projClient = new ProjectApiClient();
                var projects = await projClient.GetProjectsAsync(token);
                var tasks = await projClient.GetTasksAsync(token);

                int projCount = projects.Count;
                int taskCount = tasks.Count(t => t.Status != "COMPLETED");

                label_KpiProjVal.Text = $"{projCount} Dự án";
                label_KpiProjSub.Text = $"{taskCount} Nhiệm vụ đang triển khai";
            }
            catch
            {
                label_KpiProjVal.Text = "0 Dự án";
                label_KpiProjSub.Text = "Dịch vụ Project chưa sẵn sàng";
            }
        }

        private async Task CheckServicesHealthAsync(CancellationToken token)
        {
            // Kiểm tra trạng thái 4 dịch vụ
            await Task.Run(async () =>
            {
                var checkHealth = async (string url) =>
                {
                    try
                    {
                        using var client = new System.Net.Http.HttpClient { Timeout = TimeSpan.FromSeconds(2) };
                        var res = await client.GetAsync(url, token);
                        return res.IsSuccessStatusCode || (int)res.StatusCode < 500;
                    }
                    catch
                    {
                        return false;
                    }
                };

                bool hrOk = await checkHealth("http://localhost:5012/employees");
                bool attOk = await checkHealth("http://localhost:5231/api/attendance");
                bool payOk = await checkHealth("http://localhost:5267/api/payrollperiods");
                bool projOk = await checkHealth("http://localhost:5032/api/Projects");

                if (!IsHandleCreated) return;

                Invoke(() =>
                {
                    lblStatus_HR.Text = hrOk
                        ? "🟢 HR Service (localhost:5012) — Hoạt động bình thường"
                        : "🔴 HR Service (localhost:5012) — Mất kết nối";
                    lblStatus_HR.ForeColor = hrOk ? Color.FromArgb(22, 101, 52) : Color.FromArgb(220, 38, 38);

                    lblStatus_Attendance.Text = attOk
                        ? "🟢 Attendance Service (localhost:5231) — Hoạt động bình thường"
                        : "🔴 Attendance Service (localhost:5231) — Mất kết nối";
                    lblStatus_Attendance.ForeColor = attOk ? Color.FromArgb(22, 101, 52) : Color.FromArgb(220, 38, 38);

                    lblStatus_Payroll.Text = payOk
                        ? "🟢 Payroll Service (localhost:5267) — Hoạt động bình thường"
                        : "🔴 Payroll Service (localhost:5267) — Mất kết nối";
                    lblStatus_Payroll.ForeColor = payOk ? Color.FromArgb(22, 101, 52) : Color.FromArgb(220, 38, 38);

                    lblStatus_Project.Text = projOk
                        ? "🟢 Project Service (localhost:5032) — Hoạt động bình thường"
                        : "🔴 Project Service (localhost:5032) — Mất kết nối";
                    lblStatus_Project.ForeColor = projOk ? Color.FromArgb(22, 101, 52) : Color.FromArgb(220, 38, 38);
                });
            }, token);
        }
    }
}

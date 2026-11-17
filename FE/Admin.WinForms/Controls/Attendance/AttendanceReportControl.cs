using Admin.WinForms.DTOs.Attendance;
using Admin.WinForms.DTOs.Department;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Color = System.Drawing.Color;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Admin.WinForms.Controls.Attendance
{
    public partial class AttendanceReportControl : UserControl
    {
        private readonly Color primaryColor = Color.FromArgb(59, 130, 246);
        private readonly Color secondaryColor = Color.FromArgb(220, 53, 69);
        private readonly Color successColor = Color.FromArgb(16, 185, 129);
        private readonly Color textColor = Color.FromArgb(31, 41, 55);

        private List<AttendanceWeeklyTrendDto> _cachedWeeklyTrends = new();
        private List<AttendanceDepartmentTrendDto> _cachedDeptTrends = new();

        public AttendanceReportControl()
        {
            InitializeComponent();

            button_Generate.Click += button_Generate_Click;
            button_CreateMonthlySummary.Click += async (s, e) => await CreateMonthlySummaryAsync();
            button_LockTimesheetPeriod.Click += async (s, e) => await LockMonthlyTimesheetPeriodAsync();
            button_ExportExcel.Click += button_ExportExcel_Click;
            button_ExportPDF.Click += button_ExportPDF_Click;
            dateTimePicker1.ValueChanged += async (s, e) =>
            {
                await LoadReportDataAsync();
                await LoadAnalyticsChartsAsync();
            };

            tabControl_Reports.SelectedIndexChanged += async (s, e) =>
            {
                if (tabControl_Reports.SelectedTab == tabPage_Charts)
                {
                    await LoadAnalyticsChartsAsync();
                }
            };

            panel_Trend.Resize += (s, e) =>
            {
                if (_cachedWeeklyTrends.Count > 0)
                {
                    RenderWeeklyTrends(_cachedWeeklyTrends);
                }
            };

            panel_TrendByDepartment.Resize += (s, e) =>
            {
                if (_cachedDeptTrends.Count > 0)
                {
                    RenderDepartmentTrends(_cachedDeptTrends);
                }
            };

            Load += AttendanceReportControl_Load;
        }

        private async void AttendanceReportControl_Load(object? sender, EventArgs e)
        {
            //await LoadDepartmentsAsync();
            await LoadReportDataAsync();
            await LoadAnalyticsChartsAsync();
        }

        private async Task LoadReportDataAsync()
        {
            try
            {
                var targetMonth = dateTimePicker1.Value.Month;
                var targetYear = dateTimePicker1.Value.Year;
                var periodName = $"Kỳ công Tháng {targetMonth:D2}/{targetYear}";
                DisplayReportSummaries(new List<TimesheetSummaryDto>(), periodName);

                List<TimesheetSummaryDto>? summaries = null;

                var periods = await Program.AttendanceApi.GetTimesheetPeriodsAsync(targetYear);
                var activePeriod = periods.FirstOrDefault(p => p.Month == targetMonth && p.Year == targetYear);

                if (activePeriod != null)
                {
                    periodName = activePeriod.Name;
                    summaries = await Program.AttendanceApi.GetTimesheetSummariesByPeriodAsync(activePeriod.Id);
                }

                // Nếu kỳ công chưa có summary, tổng hợp từ bản ghi chấm công thực tế.
                if (summaries == null || summaries.Count == 0)
                {
                    var allRecords = await Program.AttendanceApi.GetAllAsync();
                    var monthRecords = allRecords
                        .Where(r => r.Date.Month == targetMonth && r.Date.Year == targetYear)
                        .ToList();

                    var standardWorkingDays = Enumerable.Range(1, DateTime.DaysInMonth(targetYear, targetMonth))
                        .Count(day =>
                        {
                            var dayOfWeek = new DateTime(targetYear, targetMonth, day).DayOfWeek;
                            return dayOfWeek != DayOfWeek.Saturday && dayOfWeek != DayOfWeek.Sunday;
                        });

                    summaries = monthRecords
                        .GroupBy(r => r.EmployeeId)
                        .Select(g =>
                        {
                            var first = g.First();
                            var employeeCode = !string.IsNullOrWhiteSpace(first.EmployeeCode)
                                ? first.EmployeeCode
                                : g.Key.ToString()[..8].ToUpperInvariant();

                            return new TimesheetSummaryDto
                            {
                                Id = Guid.NewGuid(),
                                EmployeeId = g.Key,
                                EmployeeCode = employeeCode,
                                EmployeeName = !string.IsNullOrWhiteSpace(first.EmployeeName)
                                    ? first.EmployeeName
                                    : $"Nhân viên {employeeCode}",
                                StandardWorkingDays = standardWorkingDays,
                                ActualWorkingDays = g.Count(r =>
                                    r.WorkHour > 0
                                    || string.Equals(r.Status, "Present", StringComparison.OrdinalIgnoreCase)
                                    || r.TimeIn.HasValue),
                                TotalWorkingHours = g.Sum(r => r.WorkHour),
                                LateOccurrences = g.Count(r =>
                                    r.IsLate
                                    || r.LateMins > 0
                                    || string.Equals(r.Status, "Late", StringComparison.OrdinalIgnoreCase)),
                                TotalLateMinutes = g.Sum(r => r.LateMins),
                                EarlyLeaveOccurrences = g.Count(r => r.IsEarlyLeave || r.EarlyLeaveMins > 0),
                                TotalEarlyLeaveMinutes = g.Sum(r => r.EarlyLeaveMins),
                                NormalOTHours = g.Sum(r => r.ActualOTMins > 0 ? r.ActualOTMins : r.OTMins) / 60m
                            };
                        })
                        .ToList();
                }

                DisplayReportSummaries(summaries ?? new List<TimesheetSummaryDto>(), periodName);
            }
            catch (Exception ex)
            {
                DisplayReportSummaries(new List<TimesheetSummaryDto>(), $"Kỳ công Tháng {dateTimePicker1.Value:MM/yyyy}");
                MessageBox.Show(
                    $"Không thể tải báo cáo công: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private async Task LoadAnalyticsChartsAsync()
        {
            try
            {
                var targetMonth = dateTimePicker1.Value.Month;
                var targetYear = dateTimePicker1.Value.Year;

                var weeklyTrends = await Program.AttendanceApi.GetWeeklyTrendsAsync(targetMonth, targetYear);
                var deptTrends = await Program.AttendanceApi.GetDepartmentTrendsAsync(targetMonth, targetYear);

                RenderWeeklyTrends(weeklyTrends);
                RenderDepartmentTrends(deptTrends);
            }
            catch (Exception ex)
            {
                RenderWeeklyTrends(new List<AttendanceWeeklyTrendDto>());
                RenderDepartmentTrends(new List<AttendanceDepartmentTrendDto>());
                MessageBox.Show(
                    $"Không thể tải thống kê biểu đồ: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void DisplayReportSummaries(List<TimesheetSummaryDto> summaries, string periodName)
        {
            listView_Report.BeginUpdate();
            listView_Report.Items.Clear();

            int totalEmployees = summaries?.Count ?? 0;
            int presentEmployees = summaries?.Count(s => s.ActualWorkingDays > 0) ?? 0;
            int totalLateOccurrences = summaries?.Sum(s => s.LateOccurrences) ?? 0;
            int absentEmployees = summaries?.Count(s => s.ActualWorkingDays == 0 && s.PaidLeaveDays == 0) ?? 0;

            label_NumberRecords.Text = totalEmployees.ToString();
            label_NumberPresent.Text = presentEmployees.ToString();
            label_NumberLate.Text = totalLateOccurrences.ToString();
            label_NumberAbsent.Text = absentEmployees.ToString();
            label_ReportList.Text = totalEmployees == 0
                ? $"Không có dữ liệu chấm công trong {periodName}."
                : "Monthly Timesheet Summary";

            if (summaries != null)
            {
                foreach (var s in summaries)
                {
                    // 1. Mã NV
                    var empCode = !string.IsNullOrWhiteSpace(s.EmployeeCode)
                        ? s.EmployeeCode
                        : (s.EmployeeId != Guid.Empty ? s.EmployeeId.ToString()[..8] : "--");
                    var item = new ListViewItem(empCode);

                    // 2. Họ và tên
                    item.SubItems.Add(!string.IsNullOrWhiteSpace(s.EmployeeName) ? s.EmployeeName : "--");

                    // 3. Kỳ công
                    item.SubItems.Add(periodName);

                    // 4. Công chuẩn
                    item.SubItems.Add(s.StandardWorkingDays.ToString("0.##"));

                    // 5. Ngày làm thực tế
                    item.SubItems.Add(s.ActualWorkingDays.ToString("0.##"));

                    // 6. Tổng giờ làm
                    item.SubItems.Add(s.TotalWorkingHours.ToString("0.##"));

                    // 7. Nghỉ phép có lương
                    item.SubItems.Add(s.PaidLeaveDays.ToString("0.##"));

                    // 8. Nghỉ không lương
                    item.SubItems.Add(s.UnpaidLeaveDays.ToString("0.##"));

                    // 9. Số lần đi trễ
                    item.SubItems.Add(s.LateOccurrences.ToString());

                    // 10. Phút đi trễ
                    item.SubItems.Add(s.TotalLateMinutes.ToString());

                    // 11. Số lần về sớm
                    item.SubItems.Add(s.EarlyLeaveOccurrences.ToString());

                    // 12. Phút về sớm
                    item.SubItems.Add(s.TotalEarlyLeaveMinutes.ToString());

                    // 13. OT ngày thường
                    item.SubItems.Add((s.NormalOTHours > 0 ? s.NormalOTHours : s.OTHours).ToString("0.##"));

                    // 14. OT cuối tuần
                    item.SubItems.Add(s.WeekendOTHours.ToString("0.##"));

                    // 15. OT ngày lễ
                    item.SubItems.Add(s.HolidayOTHours.ToString("0.##"));

                    item.Tag = s.Id;
                    listView_Report.Items.Add(item);
                }
            }

            listView_Report.EndUpdate();
        }

        private async void button_Generate_Click(object? sender, EventArgs e)
        {
            await LoadReportDataAsync();
            await LoadAnalyticsChartsAsync();
        }

        private async Task CreateMonthlySummaryAsync()
        {
            var month = dateTimePicker1.Value.Month;
            var year = dateTimePicker1.Value.Year;
            var confirm = MessageBox.Show(
                $"Tạo hoặc tính lại summary kỳ công tháng {month:D2}/{year} cho tất cả nhân viên đang hoạt động?",
                "Xác nhận tạo summary kỳ công",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
            {
                return;
            }

            var originalText = button_CreateMonthlySummary.Text;
            button_CreateMonthlySummary.Enabled = false;

            try
            {
                button_CreateMonthlySummary.Text = "Đang chuẩn bị...";
                var employees = await Program.AttendanceApi.GetAllEmployeesAsync();
                if (employees.Count == 0)
                {
                    throw new InvalidOperationException("Không tìm thấy nhân viên đang hoạt động để tạo summary.");
                }

                var periods = await Program.AttendanceApi.GetTimesheetPeriodsAsync(year);
                var period = periods.FirstOrDefault(item => item.Month == month && item.Year == year);
                var createdPeriod = false;

                if (period == null)
                {
                    var startDate = new DateOnly(year, month, 1);
                    period = await Program.AttendanceApi.CreateTimesheetPeriodAsync(new CreateTimesheetPeriodDto
                    {
                        Name = $"Kỳ công Tháng {month:D2}/{year}",
                        Month = month,
                        Year = year,
                        StartDate = startDate,
                        EndDate = startDate.AddMonths(1).AddDays(-1)
                    });
                    createdPeriod = true;
                }

                if (string.Equals(period.Status, "LOCKED", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show(
                        $"Kỳ công {period.Name} đã khóa và không thể tạo lại summary.",
                        "Kỳ công đã khóa",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                var succeeded = 0;
                var failures = new List<string>();
                for (var index = 0; index < employees.Count; index++)
                {
                    var employee = employees[index];
                    button_CreateMonthlySummary.Text = $"Đang tổng hợp {index + 1}/{employees.Count}...";

                    try
                    {
                        await Program.AttendanceApi.GenerateTimesheetSummaryAsync(period.Id, employee.EmployeeId);
                        succeeded++;
                    }
                    catch (Exception ex)
                    {
                        failures.Add($"{employee.EmployeeCode}: {ex.Message}");
                    }
                }

                var summaries = await Program.AttendanceApi.GetTimesheetSummariesByPeriodAsync(period.Id);
                DisplayReportSummaries(summaries, period.Name);

                var message = $"Đã tạo/tính lại summary cho {succeeded}/{employees.Count} nhân viên trong {period.Name}.";
                if (createdPeriod)
                {
                    message += Environment.NewLine + "Hệ thống đã tạo kỳ công mới cho tháng này.";
                }
                if (failures.Count > 0)
                {
                    var displayedFailures = failures.Take(5).ToList();
                    message += Environment.NewLine + Environment.NewLine
                        + $"Không thành công {failures.Count} nhân viên:"
                        + Environment.NewLine
                        + string.Join(Environment.NewLine, displayedFailures);
                    if (failures.Count > displayedFailures.Count)
                    {
                        message += Environment.NewLine + $"... và {failures.Count - displayedFailures.Count} lỗi khác.";
                    }
                }

                MessageBox.Show(
                    message,
                    failures.Count == 0 ? "Hoàn tất" : "Kết quả một phần",
                    MessageBoxButtons.OK,
                    failures.Count == 0 ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Không thể tạo summary kỳ công: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                button_CreateMonthlySummary.Text = originalText;
                button_CreateMonthlySummary.Enabled = true;
            }
        }

        private async Task LockMonthlyTimesheetPeriodAsync()
        {
            var month = dateTimePicker1.Value.Month;
            var year = dateTimePicker1.Value.Year;

            try
            {
                var periods = await Program.AttendanceApi.GetTimesheetPeriodsAsync(year);
                var period = periods.FirstOrDefault(item => item.Month == month && item.Year == year);
                if (period == null)
                {
                    MessageBox.Show(
                        $"Chưa có kỳ công tháng {month:D2}/{year}. Hãy tạo summary kỳ công trước.",
                        "Chưa có kỳ công",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                if (string.Equals(period.Status, "LOCKED", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show(
                        $"Kỳ công {period.Name} đã được khóa lúc {period.LockedAt?.ToLocalTime():dd/MM/yyyy HH:mm}.",
                        "Kỳ công đã khóa",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                var summaries = await Program.AttendanceApi.GetTimesheetSummariesByPeriodAsync(period.Id);
                if (summaries.Count == 0)
                {
                    MessageBox.Show(
                        $"Kỳ công {period.Name} chưa có summary. Hãy tạo summary trước khi khóa.",
                        "Chưa có summary",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                var confirm = MessageBox.Show(
                    $"Khóa kỳ công {period.Name} với {summaries.Count} summary? Sau khi khóa, không thể tính lại summary.",
                    "Xác nhận khóa kỳ công",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);
                if (confirm != DialogResult.Yes)
                {
                    return;
                }

                button_LockTimesheetPeriod.Enabled = false;
                var lockedPeriod = await Program.AttendanceApi.LockTimesheetPeriodAsync(period.Id, "HR Manager");
                MessageBox.Show(
                    $"Đã khóa {lockedPeriod.Name} thành công.",
                    "Hoàn tất",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                await LoadReportDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Không thể khóa kỳ công: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                button_LockTimesheetPeriod.Enabled = true;
            }
        }

        private void button_ExportExcel_Click(object? sender, EventArgs e)
        {
            try
            {
                using var sfd = new SaveFileDialog
                {
                    Filter = "CSV File (*.csv)|*.csv",
                    FileName = $"AttendanceReport_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
                };

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    var sb = new StringBuilder();
                    var headers = listView_Report.Columns.Cast<ColumnHeader>().Select(c => $"\"{c.Text}\"");
                    sb.AppendLine(string.Join(",", headers));

                    foreach (ListViewItem item in listView_Report.Items)
                    {
                        var cells = item.SubItems.Cast<ListViewItem.ListViewSubItem>().Select(s => $"\"{s.Text.Replace("\"", "\"\"")}\"");
                        sb.AppendLine(string.Join(",", cells));
                    }

                    File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                    MessageBox.Show("Xuất file báo cáo thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xuất file: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // 1. DYNAMIC ATTENDANCE TREND (THEO TUẦN)
        // ==========================================
        private void RenderWeeklyTrends(List<AttendanceWeeklyTrendDto> weeks)
        {
            _cachedWeeklyTrends = weeks;
            panel_Trend.SuspendLayout();
            panel_Trend.Controls.Clear();

            var titleLabel = new Label
            {
                Text = "Xu hướng chuyên cần theo tuần",
                Location = new Point(20, 16),
                AutoSize = true,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = textColor
            };
            panel_Trend.Controls.Add(titleLabel);

            int startY = 65;
            int clientW = panel_Trend.ClientSize.Width;
            int barWidth = Math.Max(260, clientW - 55);

            var weekFont = new Font("Segoe UI", 10F, FontStyle.Bold);
            var pctFont = new Font("Segoe UI", 10F, FontStyle.Bold);

            foreach (var week in weeks)
            {
                var labelWeek = new Label
                {
                    Text = week.Name,
                    Location = new Point(20, startY),
                    AutoSize = true,
                    Font = weekFont,
                    ForeColor = textColor
                };

                var pctText = $"{week.Present}% Có mặt";
                var labelPercentage = new Label
                {
                    Text = pctText,
                    AutoSize = true,
                    Font = pctFont,
                    ForeColor = primaryColor
                };
                labelPercentage.Location = new Point(20 + barWidth - labelPercentage.PreferredWidth, startY);

                int barY = startY + Math.Max(labelWeek.PreferredHeight, labelPercentage.PreferredHeight) + 8;

                var panelBar = new Panel
                {
                    Location = new Point(20, barY),
                    Size = new System.Drawing.Size(barWidth, 18),
                    BackColor = Color.FromArgb(229, 231, 235)
                };

                int presentWidth = (int)Math.Round((double)barWidth * Math.Clamp(week.Present, 0, 100) / 100.0);
                if (presentWidth > 0)
                {
                    var panelPresent = new Panel
                    {
                        Location = new Point(0, 0),
                        Size = new System.Drawing.Size(presentWidth, panelBar.Height),
                        BackColor = primaryColor
                    };
                    panelBar.Controls.Add(panelPresent);
                }

                if (week.Absent > 0 && presentWidth < barWidth)
                {
                    var panelAbsent = new Panel
                    {
                        Location = new Point(presentWidth, 0),
                        Size = new System.Drawing.Size(barWidth - presentWidth, panelBar.Height),
                        BackColor = secondaryColor
                    };
                    panelBar.Controls.Add(panelAbsent);
                }

                panel_Trend.Controls.Add(labelWeek);
                panel_Trend.Controls.Add(labelPercentage);
                panel_Trend.Controls.Add(panelBar);

                startY = barY + panelBar.Height + 20;
            }

            panel_Trend.ResumeLayout();
        }

        // ==========================================
        // 2. DYNAMIC ATTENDANCE BY DEPARTMENT
        // ==========================================
        private void RenderDepartmentTrends(List<AttendanceDepartmentTrendDto> departments)
        {
            _cachedDeptTrends = departments;
            panel_TrendByDepartment.SuspendLayout();
            panel_TrendByDepartment.Controls.Clear();

            var titleLabel = new Label
            {
                Text = "Tỷ lệ chuyên cần theo phòng ban",
                Location = new Point(20, 16),
                AutoSize = true,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = textColor
            };
            panel_TrendByDepartment.Controls.Add(titleLabel);

            int startY = 65;
            int clientW = panel_TrendByDepartment.ClientSize.Width;
            int barWidth = Math.Max(260, clientW - 55);

            var deptFont = new Font("Segoe UI", 10F, FontStyle.Bold);
            var pctFont = new Font("Segoe UI", 10F, FontStyle.Bold);

            foreach (var department in departments)
            {
                var labelDepartment = new Label
                {
                    Text = department.Name,
                    Location = new Point(20, startY),
                    AutoSize = true,
                    Font = deptFont,
                    ForeColor = textColor
                };

                var pctText = $"{department.Percentage}%";
                var labelPercentage = new Label
                {
                    Text = pctText,
                    AutoSize = true,
                    Font = pctFont,
                    ForeColor = successColor
                };
                labelPercentage.Location = new Point(20 + barWidth - labelPercentage.PreferredWidth, startY);

                int barY = startY + Math.Max(labelDepartment.PreferredHeight, labelPercentage.PreferredHeight) + 8;

                var panelBar = new Panel
                {
                    Location = new Point(20, barY),
                    Size = new System.Drawing.Size(barWidth, 18),
                    BackColor = Color.FromArgb(229, 231, 235)
                };

                int fillWidth = (int)Math.Round((double)barWidth * Math.Clamp(department.Percentage, 0, 100) / 100.0);
                if (fillWidth > 0)
                {
                    var panelFill = new Panel
                    {
                        Location = new Point(0, 0),
                        Size = new System.Drawing.Size(fillWidth, panelBar.Height),
                        BackColor = successColor
                    };
                    panelBar.Controls.Add(panelFill);
                }

                panel_TrendByDepartment.Controls.Add(labelDepartment);
                panel_TrendByDepartment.Controls.Add(labelPercentage);
                panel_TrendByDepartment.Controls.Add(panelBar);

                startY = barY + panelBar.Height + 20;
            }

            panel_TrendByDepartment.ResumeLayout();
        }


        // ==========================================
        // EXPORT ATTENDANCE REPORT TO PDF
        // ==========================================

        private void button_ExportPDF_Click(object? sender, EventArgs e)
        {
            try
            {
                // 1. Kiểm tra dữ liệu
                if (listView_Report.Items.Count == 0)
                {
                    MessageBox.Show(
                        "Không có dữ liệu để xuất PDF.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                var headers = listView_Report.Columns
                    .Cast<ColumnHeader>()
                    .Select(c => c.Text)
                    .ToList();

                if (headers.Count == 0)
                {
                    MessageBox.Show(
                        "Không tìm thấy cột dữ liệu.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                // 2. Chọn nơi lưu file
                using var sfd = new SaveFileDialog
                {
                    Filter = "PDF File (*.pdf)|*.pdf",
                    DefaultExt = "pdf",
                    AddExtension = true,
                    OverwritePrompt = true,
                    FileName = $"AttendanceReport_{dateTimePicker1.Value:yyyy_MM}.pdf",
                    Title = "Xuất báo cáo chấm công"
                };

                if (sfd.ShowDialog() != DialogResult.OK)
                    return;

                // 3. Lấy dữ liệu từ ListView
                var rows = listView_Report.Items
                    .Cast<ListViewItem>()
                    .Select(item => Enumerable.Range(0, headers.Count)
                        .Select(index =>
                            index < item.SubItems.Count
                                ? item.SubItems[index].Text
                                : string.Empty)
                        .ToList())
                    .ToList();

                // 4. Cấu hình QuestPDF
                QuestPDF.Settings.License = LicenseType.Community;

                // Đăng ký font Arial để hỗ trợ tiếng Việt
                string fontPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Fonts),
                    "arial.ttf");

                if (File.Exists(fontPath))
                {
                    using var fontStream = File.OpenRead(fontPath);
                    FontManager.RegisterFont(fontStream);
                }

                string monthYear = dateTimePicker1.Value.ToString("MM/yyyy");

                // 5. Tạo PDF
                QuestPDF.Fluent.Document.Create(document =>
                {
                    document.Page(page =>
                    {
                        // Khổ giấy A3 ngang
                        page.Size(PageSizes.A3.Landscape());
                        page.Margin(18);
                        page.PageColor(Colors.White);

                        page.DefaultTextStyle(style =>
                            style.FontFamily("Arial").FontSize(7));

                        // HEADER
                        page.Header()
                            .PaddingBottom(12)
                            .Column(column =>
                            {
                                column.Item()
                                    .AlignCenter()
                                    .Text("BÁO CÁO CHẤM CÔNG NHÂN VIÊN")
                                    .FontSize(18)
                                    .Bold();

                                column.Item()
                                    .PaddingTop(3)
                                    .AlignCenter()
                                    .Text($"Kỳ công tháng {monthYear}")
                                    .FontSize(11);

                                column.Item()
                                    .PaddingTop(5)
                                    .AlignRight()
                                    .Text($"Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm}")
                                    .FontSize(8);
                            });

                        // CONTENT
                        page.Content().Table(table =>
                        {
                            // Chia độ rộng các cột
                            table.ColumnsDefinition(columns =>
                            {
                                for (int i = 0; i < headers.Count; i++)
                                {
                                    columns.RelativeColumn(GetPdfColumnWeight(i));
                                }
                            });

                            // Tiêu đề bảng
                            table.Header(header =>
                            {
                                foreach (var title in headers)
                                {
                                    header.Cell()
                                        .Element(HeaderStyle)
                                        .AlignCenter()
                                        .Text(title)
                                        .Bold()
                                        .FontSize(7);
                                }
                            });

                            // Nội dung bảng
                            foreach (var row in rows)
                            {
                                for (int i = 0; i < headers.Count; i++)
                                {
                                    var cell = table.Cell().Element(BodyStyle);

                                    if (i == 1)
                                    {
                                        // Họ tên căn trái
                                        cell.AlignLeft()
                                            .Text(row[i])
                                            .FontSize(6.5f);
                                    }
                                    else
                                    {
                                        cell.AlignCenter()
                                            .Text(row[i])
                                            .FontSize(6.5f);
                                    }
                                }
                            }
                        });

                        // FOOTER
                        page.Footer()
                            .PaddingTop(8)
                            .AlignCenter()
                            .Text(text =>
                            {
                                text.Span("Trang ");
                                text.CurrentPageNumber();
                                text.Span(" / ");
                                text.TotalPages();
                            });
                    });
                })
                .GeneratePdf(sfd.FileName);

                // 6. Thông báo thành công
                MessageBox.Show(
                    $"Xuất báo cáo PDF thành công!\n\n{sfd.FileName}",
                    "Hoàn tất",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // Mở file PDF bằng ứng dụng mặc định
                try
                {
                    System.Diagnostics.Process.Start(
                        new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = sfd.FileName,
                            UseShellExecute = true
                        });
                }
                catch
                {
                    // File đã xuất thành công, không ảnh hưởng nếu không mở được.
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi xuất PDF: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // Điều chỉnh độ rộng cột
        private static float GetPdfColumnWeight(int index)
        {
            return index switch
            {
                0 => 1.1f, // Mã nhân viên
                1 => 2.2f, // Họ và tên
                2 => 1.6f, // Kỳ công
                _ => 1.0f
            };
        }

        private QuestPDF.Infrastructure.IContainer HeaderStyle(
    QuestPDF.Infrastructure.IContainer container)
        {
            return container
                .Background(Colors.Grey.Lighten2)
                .Padding(4)
                .DefaultTextStyle(x => x.SemiBold());
        }

        private QuestPDF.Infrastructure.IContainer BodyStyle(
            QuestPDF.Infrastructure.IContainer container)
        {
            return container
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten2)
                .Padding(4);
        }


    }

}

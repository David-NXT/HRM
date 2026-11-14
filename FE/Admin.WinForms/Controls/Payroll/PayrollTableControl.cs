using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Admin.WinForms.DTOs.Payroll;

namespace Admin.WinForms.Controls.Payroll
{
    public partial class PayrollTableControl : UserControl
    {
        private List<PayrollPeriodDto> _periods = new();
        private PayrollPeriodDto? _selectedPeriod;

        public PayrollTableControl()
        {
            InitializeComponent();

            comboBox_Periods.SelectedIndexChanged += async (s, e) =>
            {
                _selectedPeriod = comboBox_Periods.SelectedItem as PayrollPeriodDto;
                await LoadPayrollsAsync();
            };

            button_NewPeriod.Click += async (s, e) => await OnNewPeriodAsync();
            button_SyncAttendance.Click += async (s, e) => await OnSyncAttendanceAsync();
            button_SyncSalary.Click += async (s, e) => await OnSyncSalaryAsync();
            button_CreatePayrolls.Click += async (s, e) => await OnCreatePayrollsAsync();
            button_CalculateAll.Click += async (s, e) => await OnCalculateAllAsync();
            button_ExportExcel.Click += (s, e) => ExportToCsv();

            Load += async (s, e) =>
            {
                await LoadPeriodsAsync();
            };
        }

        public async Task LoadPeriodsAsync()
        {
            try
            {
                _periods = await Program.PayrollApi.GetPeriodsAsync();
                comboBox_Periods.Items.Clear();

                if (_periods.Count == 0)
                {
                    // Tự động seed hoặc mở kỳ mẫu tháng hiện tại nếu chưa có
                    try
                    {
                        await Program.PayrollApi.SeedSampleDataAsync();
                        var current = await Program.PayrollApi.CreatePeriodAsync(2026, 11);
                        if (current != null) _periods.Add(current);
                    }
                    catch { }

                    _periods = await Program.PayrollApi.GetPeriodsAsync();
                }

                foreach (var p in _periods)
                {
                    comboBox_Periods.Items.Add(p);
                }

                comboBox_Periods.DisplayMember = "DisplayName";
                comboBox_Periods.ValueMember = "PayrollPeriodID";

                if (comboBox_Periods.Items.Count > 0)
                {
                    comboBox_Periods.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải kỳ lương: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadPayrollsAsync()
        {
            if (_selectedPeriod == null)
            {
                listView_Payrolls.Items.Clear();
                ResetStats();
                return;
            }

            try
            {
                var payrolls = await Program.PayrollApi.GetPayrollsByPeriodAsync(_selectedPeriod.PayrollPeriodID);

                listView_Payrolls.BeginUpdate();
                listView_Payrolls.Items.Clear();

                decimal totalNet = 0;
                decimal totalAllowance = 0;
                decimal totalPayCut = 0;

                foreach (var p in payrolls)
                {
                    totalNet += p.NetSalary;
                    totalAllowance += p.TotalAllowance;
                    totalPayCut += p.TotalPayCut;

                    var empCode = !string.IsNullOrWhiteSpace(p.EmployeeCode) ? p.EmployeeCode : p.EmployeeID.ToString()[..8].ToUpper();
                    var item = new ListViewItem(empCode);

                    item.SubItems.Add(!string.IsNullOrWhiteSpace(p.EmployeeName) ? p.EmployeeName : $"Nhân viên {empCode}");
                    item.SubItems.Add(p.BaseSalary.ToString("#,##0 đ"));
                    item.SubItems.Add(p.StandardWorkingDay.ToString("0.##"));
                    item.SubItems.Add(p.WorkingDays.ToString("0.##"));
                    item.SubItems.Add(p.TotalAllowance.ToString("#,##0 đ"));
                    item.SubItems.Add(p.TotalPayCut.ToString("#,##0 đ"));
                    item.SubItems.Add(p.GrossSalary.ToString("#,##0 đ"));
                    item.SubItems.Add(p.NetSalary.ToString("#,##0 đ"));
                    item.SubItems.Add(FormatStatus(p.Status));
                    item.SubItems.Add(p.CalculatedAt.HasValue ? p.CalculatedAt.Value.ToString("dd/MM/yyyy HH:mm") : "--");

                    item.Tag = p.PayrollID;

                    // Màu sắc theo trạng thái
                    if (p.Status.Equals("paid", StringComparison.OrdinalIgnoreCase))
                    {
                        item.ForeColor = Color.FromArgb(16, 149, 74);
                    }
                    else if (p.Status.Equals("approved", StringComparison.OrdinalIgnoreCase))
                    {
                        item.ForeColor = Color.FromArgb(37, 99, 235);
                    }
                    else if (p.Status.Equals("calculated", StringComparison.OrdinalIgnoreCase))
                    {
                        item.ForeColor = Color.FromArgb(180, 83, 9);
                    }

                    listView_Payrolls.Items.Add(item);
                }

                listView_Payrolls.EndUpdate();

                // Cập nhật thẻ thống kê
                label_TotalNet.Text = totalNet.ToString("#,##0 đ");
                label_TotalEmployees.Text = payrolls.Count.ToString();
                label_TotalAllowance.Text = totalAllowance.ToString("#,##0 đ");
                label_TotalPayCut.Text = totalPayCut.ToString("#,##0 đ");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải bảng lương: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ResetStats()
        {
            label_TotalNet.Text = "0 đ";
            label_TotalEmployees.Text = "0";
            label_TotalAllowance.Text = "0 đ";
            label_TotalPayCut.Text = "0 đ";
        }

        private static string FormatStatus(string status)
        {
            return status?.ToLowerInvariant() switch
            {
                "draft" => "Bản nháp",
                "calculated" => "Đã tính lương",
                "approved" => "Đã phê duyệt",
                "paid" => "Đã chi trả",
                _ => status ?? "--"
            };
        }

        private async Task OnNewPeriodAsync()
        {
            using var form = new Form
            {
                Text = "Mở Kỳ Lương Mới",
                Size = new Size(380, 220),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var labelMonth = new Label { Text = "Tháng:", Location = new Point(30, 30), AutoSize = true };
            var numMonth = new NumericUpDown { Location = new Point(120, 28), Minimum = 1, Maximum = 12, Value = DateTime.Today.Month, Width = 180 };

            var labelYear = new Label { Text = "Năm:", Location = new Point(30, 75), AutoSize = true };
            var numYear = new NumericUpDown { Location = new Point(120, 73), Minimum = 2020, Maximum = 2030, Value = DateTime.Today.Year, Width = 180 };

            var btnOk = new Button { Text = "Mở Kỳ", DialogResult = DialogResult.OK, Location = new Point(120, 120), Width = 90, BackColor = Color.FromArgb(37, 99, 235), ForeColor = Color.White };
            var btnCancel = new Button { Text = "Hủy", DialogResult = DialogResult.Cancel, Location = new Point(220, 120), Width = 80 };

            form.Controls.AddRange(new Control[] { labelMonth, numMonth, labelYear, numYear, btnOk, btnCancel });
            form.AcceptButton = btnOk;
            form.CancelButton = btnCancel;

            if (form.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var created = await Program.PayrollApi.CreatePeriodAsync((int)numYear.Value, (int)numMonth.Value);
                    MessageBox.Show("Mở kỳ lương mới thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadPeriodsAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async Task OnSyncAttendanceAsync()
        {
            if (_selectedPeriod == null)
            {
                MessageBox.Show("Vui lòng chọn kỳ lương cần đồng bộ công.", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                $"Hệ thống sẽ tiến hành chốt dữ liệu công từ phân hệ Chấm công (Attendance Service) và cập nhật số ngày làm việc cho Kỳ lương Tháng {_selectedPeriod.Month}/{_selectedPeriod.Year}.\n\nBạn có muốn tiếp tục?",
                "Xác nhận Chốt & Đồng bộ công",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            try
            {
                var count = await Program.PayrollApi.SyncAttendanceAsync(_selectedPeriod.PayrollPeriodID);
                MessageBox.Show($"Đồng bộ chấm công thành công! Đã cập nhật công cho {count} nhân sự.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadPayrollsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task OnSyncSalaryAsync()
        {
            if (_selectedPeriod == null)
            {
                MessageBox.Show("Vui lòng chọn kỳ lương cần đồng bộ mức lương.", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var count = await Program.PayrollApi.SyncSalaryAsync(_selectedPeriod.PayrollPeriodID);
                MessageBox.Show($"Đồng bộ mức lương cơ bản thành công! Đã cập nhật lương cho {count} nhân sự.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadPayrollsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task OnCreatePayrollsAsync()
        {
            if (_selectedPeriod == null)
            {
                MessageBox.Show("Vui lòng chọn kỳ lương.", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Lấy danh sách ID nhân viên từ chấm công hoặc HR
                var attendanceRecords = await Program.AttendanceApi.GetAllAsync();
                var empIds = attendanceRecords.Select(r => r.EmployeeId).Distinct().ToList();

                if (empIds.Count == 0)
                {
                    // Danh sách nhân viên mẫu nếu DB rỗng
                    empIds = new List<Guid>
                    {
                        Guid.Parse("11111111-1111-1111-1111-111111111111"),
                        Guid.Parse("22222222-2222-2222-2222-222222222222"),
                        Guid.Parse("33333333-3333-3333-3333-333333333333"),
                        Guid.Parse("44444444-4444-4444-4444-444444444444")
                    };
                }

                await Program.PayrollApi.CreatePayrollsAsync(_selectedPeriod.PayrollPeriodID, empIds);
                MessageBox.Show($"Đã khởi tạo bảng lương thành công cho {empIds.Count} nhân sự!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadPayrollsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task OnCalculateAllAsync()
        {
            if (_selectedPeriod == null)
            {
                MessageBox.Show("Vui lòng chọn kỳ lương.", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var payrolls = await Program.PayrollApi.GetPayrollsByPeriodAsync(_selectedPeriod.PayrollPeriodID);
            if (payrolls.Count == 0)
            {
                MessageBox.Show("Chưa có nhân sự trong bảng lương của kỳ này. Vui lòng bấm 'Tạo Bảng Lương' trước.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int success = 0;
            foreach (var p in payrolls)
            {
                try
                {
                    await Program.PayrollApi.CalculatePayrollAsync(p.PayrollID);
                    success++;
                }
                catch { }
            }

            MessageBox.Show($"Hoàn tất tính toán bảng lương cho {success}/{payrolls.Count} nhân viên!", "Kết quả tính toán", MessageBoxButtons.OK, MessageBoxIcon.Information);
            await LoadPayrollsAsync();
        }

        private void ExportToCsv()
        {
            try
            {
                using var sfd = new SaveFileDialog
                {
                    Filter = "CSV File (*.csv)|*.csv",
                    FileName = $"BangLuong_{_selectedPeriod?.Month:D2}_{_selectedPeriod?.Year}_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
                };

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    var sb = new StringBuilder();
                    var headers = listView_Payrolls.Columns.Cast<ColumnHeader>().Select(c => $"\"{c.Text}\"");
                    sb.AppendLine(string.Join(",", headers));

                    foreach (ListViewItem item in listView_Payrolls.Items)
                    {
                        var cells = item.SubItems.Cast<ListViewItem.ListViewSubItem>().Select(s => $"\"{s.Text.Replace("\"", "\"\"")}\"");
                        sb.AppendLine(string.Join(",", cells));
                    }

                    File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                    MessageBox.Show("Xuất file bảng lương thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xuất file: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

using Admin.WinForms.BUS.DTOs.Attendance;
using Admin.WinForms.BUS.DTOs.Department;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Admin.WinForms.GUI.Controls.Attendance
{
    public partial class AttendanceOverviewPageControl : UserControl
    {
        public AttendanceOverviewPageControl()
        {
            InitializeComponent();

            InitializeFilters();

            // Wire up event handlers
            button_Search.Click += button_Search_Click;
            button_ExportExcel.Click += button_ExportExcel_Click;
            button_Print.Click += button_Print_Click;
            textBox_Search.KeyDown += TextBox_Search_KeyDown;
            dateTimePicker1.ValueChanged += DateTimePicker1_ValueChanged;

            Load += AttendanceOverviewPageControl_Load;
        }

        private async void DateTimePicker1_ValueChanged(object? sender, EventArgs e)
        {
            var selectedDate = DateOnly.FromDateTime(dateTimePicker1.Value);
            var records = await Program.AttendanceApi.GetAllAsync();
            await UpdateStatisticsFromApiAsync(selectedDate, records);
        }

        private void TextBox_Search_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                button_Search.PerformClick();
            }
        }

        private async void AttendanceOverviewPageControl_Load(object? sender, EventArgs e)
        {
            await LoadDepartmentsAsync();
            await LoadDataAsync();
        }

        private async Task LoadDepartmentsAsync()
        {
            try
            {
                var departments = await Program.AttendanceApi.GetDepartmentsAsync();

                comboBox_Department.Items.Clear();
                comboBox_Department.Items.Add(new DepartmentDto { Id = Guid.Empty, Name = "All Departments" });

                if (departments != null && departments.Count > 0)
                {
                    foreach (var dept in departments)
                    {
                        comboBox_Department.Items.Add(dept);
                    }
                }

                comboBox_Department.DisplayMember = "Name";
                comboBox_Department.ValueMember = "Id";
                comboBox_Department.SelectedIndex = 0;
            }
            catch
            {
                // Fallback nếu API lỗi
                comboBox_Department.Items.Clear();
                comboBox_Department.Items.Add(new DepartmentDto { Id = Guid.Empty, Name = "All Departments" });
                comboBox_Department.SelectedIndex = 0;
            }
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var records = await Program.AttendanceApi.GetAllAsync();
                DisplayAttendance(records);

                var selectedDate = DateOnly.FromDateTime(dateTimePicker1.Value);
                await UpdateStatisticsFromApiAsync(selectedDate, records);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Không thể tải dữ liệu Attendance",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private async Task UpdateStatisticsFromApiAsync(DateOnly date, List<AttendanceRecordDto> fallbackRecords)
        {
            try
            {
                var stats = await Program.AttendanceApi.GetOverviewStatsAsync(date);
                if (stats != null && stats.TotalEmployees > 0)
                {
                    label_NumberEmployees.Text = stats.TotalEmployees.ToString();
                    label_NumberPresent.Text = stats.PresentCount.ToString();
                    label_NumberLate.Text = stats.LateCount.ToString();
                    label_NumberAbsent.Text = stats.AbsentCount.ToString();
                    return;
                }
            }
            catch
            {
            }

            // Fallback nếu chưa kết nối được API thống kê
            label_NumberEmployees.Text = fallbackRecords.Select(r => r.EmployeeId).Distinct().Count().ToString();
            label_NumberPresent.Text = fallbackRecords.Count(r => r.Status.Equals("Present", StringComparison.OrdinalIgnoreCase) || r.WorkHour > 0).ToString();
            label_NumberLate.Text = fallbackRecords.Count(r => r.IsLate || r.LateMins > 0 || r.Status.Equals("Late", StringComparison.OrdinalIgnoreCase)).ToString();
            label_NumberAbsent.Text = fallbackRecords.Count(r => r.Status.Equals("Absent", StringComparison.OrdinalIgnoreCase)).ToString();
        }

        private void DisplayAttendance(List<AttendanceRecordDto> records)
        {
            listView_Attendance.BeginUpdate();
            listView_Attendance.Items.Clear();

            foreach (var record in records)
            {
                // 1. Mã NV / Employee Code
                var empCode = !string.IsNullOrWhiteSpace(record.EmployeeCode)
                    ? record.EmployeeCode
                    : (record.EmployeeId != Guid.Empty ? record.EmployeeId.ToString()[..8] : "--");
                var item = new ListViewItem(empCode);

                // 2. Họ tên
                item.SubItems.Add(!string.IsNullOrWhiteSpace(record.EmployeeName) ? record.EmployeeName : "--");

                // 3. Ngày
                item.SubItems.Add(record.Date.ToString("dd/MM/yyyy"));

                // 4. Ca làm việc
                item.SubItems.Add(record.ShiftId != Guid.Empty ? record.ShiftId.ToString()[..8] : "Ca chuẩn");

                // 5. Giờ vào
                item.SubItems.Add(record.TimeIn?.ToString("HH:mm") ?? "--:--");

                // 6. Giờ ra
                item.SubItems.Add(record.TimeOut?.ToString("HH:mm") ?? "--:--");

                // 7. Số giờ làm
                item.SubItems.Add(record.WorkHour.ToString("0.##"));

                // 8. Trạng thái
                item.SubItems.Add(record.Status);

                // 9. Đi trễ (phút)
                item.SubItems.Add(record.LateMins.ToString());

                // 10. Về sớm (phút)
                item.SubItems.Add(record.EarlyLeaveMins.ToString());

                // 11. OT (phút)
                item.SubItems.Add(record.OTMins.ToString());

                // 12. Vị trí check-in
                item.SubItems.Add(record.CheckInLocation ?? "--");

                // 13. Thiết bị
                item.SubItems.Add(record.DeviceId ?? "--");

                // 14. Ghi chú
                item.SubItems.Add(record.Note ?? "--");

                item.Tag = record.Id;
                listView_Attendance.Items.Add(item);
            }

            listView_Attendance.EndUpdate();
        }

        private void InitializeFilters()
        {
            // Date
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "dd/MM/yyyy";
            dateTimePicker1.Value = DateTime.Today;

            // Department
            comboBox_Department.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_Department.Items.Clear();
            comboBox_Department.Items.Add(new DepartmentDto { Id = Guid.Empty, Name = "All Departments" });
            comboBox_Department.DisplayMember = "Name";
            comboBox_Department.ValueMember = "Id";
            comboBox_Department.SelectedIndex = 0;

            // Status
            comboBox_Status.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_Status.Items.Clear();
            comboBox_Status.Items.AddRange(new object[]
            {
                "All Status",
                "Present",
                "Late",
                "Absent",
                "On Leave",
                "Pending"
            });
            comboBox_Status.SelectedIndex = 0;
        }

        private AttendanceSearchRequest GetSearchRequest()
        {
            var selectedDepartment = comboBox_Department.SelectedItem as DepartmentDto;
            string selectedStatus = comboBox_Status.SelectedItem?.ToString() ?? "All Status";

            return new AttendanceSearchRequest
            {
                Date = DateOnly.FromDateTime(dateTimePicker1.Value),

                DepartmentId = (selectedDepartment == null || selectedDepartment.Id == Guid.Empty)
                    ? null
                    : selectedDepartment.Id,

                Status = selectedStatus == "All Status"
                    ? null
                    : selectedStatus,

                Keyword = string.IsNullOrWhiteSpace(textBox_Search.Text)
                    ? null
                    : textBox_Search.Text.Trim()
            };
        }

        private async void button_Search_Click(object? sender, EventArgs e)
        {
            try
            {
                var request = GetSearchRequest();
                var records = await Program.AttendanceApi.SearchAsync(request);
                DisplayAttendance(records);

                if (request.Date.HasValue)
                {
                    await UpdateStatisticsFromApiAsync(request.Date.Value, records);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Tìm kiếm thất bại: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void button_ExportExcel_Click(object? sender, EventArgs e)
        {
            try
            {
                using var sfd = new SaveFileDialog
                {
                    Filter = "CSV File (*.csv)|*.csv",
                    FileName = $"Attendance_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
                };

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    var sb = new StringBuilder();
                    var headers = listView_Attendance.Columns.Cast<ColumnHeader>().Select(c => $"\"{c.Text}\"");
                    sb.AppendLine(string.Join(",", headers));

                    foreach (ListViewItem item in listView_Attendance.Items)
                    {
                        var cells = item.SubItems.Cast<ListViewItem.ListViewSubItem>().Select(s => $"\"{s.Text.Replace("\"", "\"\"")}\"");
                        sb.AppendLine(string.Join(",", cells));
                    }

                    File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                    MessageBox.Show("Xuất file CSV thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xuất file: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button_Print_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("Chức năng In bảng công đang sẵn sàng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}

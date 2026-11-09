using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json;

namespace Attendance
{
    public partial class Main : Form
    {
        private static readonly HttpClient ApiClient = new()
        {
            BaseAddress = new Uri("http://localhost:5231/")
        };

        public Main()
        {
            InitializeComponent();

            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                Shown += async (_, _) => await LoadAttendanceAsync();
            }
        }

        private async Task LoadAttendanceAsync()
        {
            try
            {
                var records = await ApiClient.GetFromJsonAsync<List<AttendanceRecordResponse>>("api/Attendance") ?? [];

                lvAttendance.BeginUpdate();
                lvAttendance.Items.Clear();

                foreach (var record in records)
                {
                    AddAttendance(record);
                }

                lvAttendance.EndUpdate();
                UpdateTotal();
            }
            catch (Exception ex) when (ex is HttpRequestException or JsonException)
            {
                MessageBox.Show(
                    "Không thể tải dữ liệu từ Attendance API. Hãy kiểm tra API đang chạy tại http://localhost:5231.\n\n" + ex.Message,
                    "Lỗi kết nối API",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void AddAttendance(AttendanceRecordResponse record)
        {
            var item = new ListViewItem(record.EmployeeId.ToString());
            item.SubItems.Add(record.EmployeeId.ToString());
            item.SubItems.Add(record.ShiftId.ToString());
            item.SubItems.Add(record.Date.ToString("dd/MM/yyyy"));
            item.SubItems.Add(record.TimeIn?.ToString("HH:mm") ?? "--:--");
            item.SubItems.Add(record.TimeOut?.ToString("HH:mm") ?? "--:--");
            item.SubItems.Add(record.Status);
            item.SubItems.Add(record.Note ?? string.Empty);
            lvAttendance.Items.Add(item);
        }

        private void UpdateTotal()
        {
            lblTotal.Text = $"Tổng số bản ghi: {lvAttendance.Items.Count}";
        }

        private async void btnRefresh_Click(object? sender, EventArgs e)
        {
            txtSearch.Clear();
            cboStatus.SelectedIndex = 0;
            dtpDate.Value = DateTime.Today;
            await LoadAttendanceAsync();
        }

        private void btnAdd_Click(object? sender, EventArgs e)
        {
            var item = new ListViewItem($"NV{lvAttendance.Items.Count + 1:000}");
            item.SubItems.Add("Nhân viên mới");
            item.SubItems.Add("Chưa phân phòng");
            item.SubItems.Add(dtpDate.Value.ToString("dd/MM/yyyy"));
            item.SubItems.Add("--:--");
            item.SubItems.Add("--:--");
            item.SubItems.Add("Chưa chấm công");
            item.SubItems.Add(string.Empty);
            lvAttendance.Items.Add(item);
            UpdateTotal();
        }

    internal sealed class AttendanceRecordResponse
    {
        public Guid EmployeeId { get; set; }
        public Guid ShiftId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly? TimeIn { get; set; }
        public TimeOnly? TimeOut { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Note { get; set; }
    }
    }
}

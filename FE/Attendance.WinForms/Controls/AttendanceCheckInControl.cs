using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Attendance.WinForms.DTOs;

namespace Attendance.WinForms.Controls
{
    public partial class AttendanceCheckInControl : UserControl
    {
        private readonly System.Windows.Forms.Timer _clockTimer = new();
        private List<EmployeeDirectoryDto> _employees = new();
        private List<WorkShiftDto> _shifts = new();
        private EmployeeDirectoryDto? _currentEmployee;
        private AttendanceRecordDto? _todayRecord;
        private Guid _defaultShiftId = Guid.Parse("00000001-0000-0000-0000-000000000001");
        private WorkShiftDto? _defaultShift;

        public AttendanceCheckInControl()
        {
            InitializeComponent();

            _clockTimer.Interval = 1000;
            _clockTimer.Tick += (s, e) =>
            {
                label_CurrentClock.Text = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy");
            };
            _clockTimer.Start();

            button_Lookup.Click += async (s, e) => await LookupEmployeeAsync(textBox_EmpCode.Text.Trim());
            textBox_EmpCode.KeyDown += async (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    await LookupEmployeeAsync(textBox_EmpCode.Text.Trim());
                }
            };

            comboBox_Employees.SelectedIndexChanged += async (s, e) =>
            {
                if (comboBox_Employees.SelectedItem is EmployeeDirectoryDto emp)
                {
                    await DisplayEmployeeAsync(emp);
                }
            };

            button_CheckIn.Click += async (s, e) => await OnCheckInAsync();
            button_CheckOut.Click += async (s, e) => await OnCheckOutAsync();

            button_CheckIn.Enabled = false;
            button_CheckOut.Enabled = false;

            Load += async (s, e) =>
            {
                label_CurrentClock.Text = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy");
                await LoadInitialDataAsync();
                await LoadRecentLogsAsync();
            };
        }

        private async Task LoadInitialDataAsync()
        {
            try
            {
                _shifts = await Program.AttendanceApi.GetWorkShiftsAsync();
                _defaultShift = _shifts.FirstOrDefault(s => s.IsActive) ?? _shifts.FirstOrDefault();
                if (_defaultShift != null)
                {
                    _defaultShiftId = _defaultShift.Id;
                }
            }
            catch
            {
                _defaultShift = new WorkShiftDto
                {
                    Id = _defaultShiftId,
                    Name = "Ca Hành Chính",
                    StartTime = new TimeOnly(8, 0),
                    EndTime = new TimeOnly(17, 0),
                    BreakMins = 60,
                    IsActive = true
                };
            }

            try
            {
                _employees = await Program.AttendanceApi.GetEmployeesAsync();
            }
            catch
            {
                _employees = new List<EmployeeDirectoryDto>();
            }

            if (_employees == null || _employees.Count == 0)
            {
                _employees = new List<EmployeeDirectoryDto>
                {
                    new() { EmployeeId = Guid.Parse("11111111-1111-1111-1111-111111111111"), EmployeeCode = "NV001", FullName = "Nguyễn Văn An", DepartmentName = "Phòng Công Nghệ Thông Tin", PositionName = "Lập trình viên Backend" },
                    new() { EmployeeId = Guid.Parse("22222222-2222-2222-2222-222222222222"), EmployeeCode = "NV002", FullName = "Trần Thị Bình", DepartmentName = "Phòng Hành Chính Nhân Sự", PositionName = "Chuyên viên Nhân sự" },
                    new() { EmployeeId = Guid.Parse("33333333-3333-3333-3333-333333333333"), EmployeeCode = "NV003", FullName = "Lê Hoàng Cường", DepartmentName = "Phòng Marketing", PositionName = "Trưởng nhóm Marketing" },
                    new() { EmployeeId = Guid.Parse("44444444-4444-4444-4444-444444444444"), EmployeeCode = "NV004", FullName = "Phạm Thị Dung", DepartmentName = "Phòng Kế Toán", PositionName = "Kế toán viên" }
                };
            }

            comboBox_Employees.Items.Clear();
            foreach (var emp in _employees)
            {
                comboBox_Employees.Items.Add(emp);
            }

            comboBox_Employees.DisplayMember = "DisplayInfo";
            comboBox_Employees.ValueMember = "EmployeeId";

            if (comboBox_Employees.Items.Count > 0)
            {
                comboBox_Employees.SelectedIndex = 0;
            }
        }

        private async Task LookupEmployeeAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                MessageBox.Show("Vui lòng nhập Mã Nhân Viên (ví dụ: NV001, NV002...).", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var emp = _employees.FirstOrDefault(e => e.EmployeeCode.Equals(code, StringComparison.OrdinalIgnoreCase) ||
                                                     e.EmployeeId.ToString().StartsWith(code, StringComparison.OrdinalIgnoreCase));

            if (emp == null)
            {
                var results = await Program.AttendanceApi.GetEmployeesAsync(code);
                emp = results.FirstOrDefault();
            }

            if (emp == null)
            {
                MessageBox.Show($"Không tìm thấy nhân viên với mã: {code}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            await DisplayEmployeeAsync(emp);
        }

        public async Task DisplayEmployeeAsync(EmployeeDirectoryDto emp)
        {
            _currentEmployee = emp;

            textBox_EmpCode.Text = emp.EmployeeCode;
            textBox_EmpCodeDisplay.Text = emp.EmployeeCode;
            textBox_FullName.Text = emp.FullName;
            textBox_Department.Text = emp.DepartmentName ?? "Chưa phân phòng";
            textBox_Position.Text = emp.PositionName ?? "Nhân viên";
            textBox_Shift.Text = _defaultShift != null ? $"{_defaultShift.Name} ({_defaultShift.StartTime:HH:mm} - {_defaultShift.EndTime:HH:mm})" : "Ca Hành Chính (08:00 - 17:00)";

            var today = DateOnly.FromDateTime(DateTime.Today);
            _todayRecord = await Program.AttendanceApi.GetTodayRecordAsync(emp.EmployeeId, today);

            UpdateActionButtonsStatus();
        }

        public async Task SelectEmployeeAsync(Guid employeeId)
        {
            if (_employees == null || _employees.Count == 0)
            {
                await LoadInitialDataAsync();
            }

            var emp = _employees?.FirstOrDefault(e => e.EmployeeId == employeeId);
            if (emp != null)
            {
                comboBox_Employees.SelectedItem = emp;
                await DisplayEmployeeAsync(emp);
            }
        }

        public async Task SelectEmployeeAsync(EmployeeDirectoryDto emp)
        {
            if (emp != null)
            {
                comboBox_Employees.SelectedItem = emp;
                await DisplayEmployeeAsync(emp);
            }
        }

        private void UpdateActionButtonsStatus()
        {
            if (_todayRecord == null)
            {
                label_StatusToday.Text = "Trạng thái hôm nay: CHƯA CHECK-IN\nNhấn nút 'CHECK-IN' bên dưới để bắt đầu ca làm việc.";
                label_StatusToday.ForeColor = Color.FromArgb(180, 83, 9);
                button_CheckIn.Enabled = true;
                button_CheckOut.Enabled = false;
            }
            else if (_todayRecord.TimeOut == null)
            {
                label_StatusToday.Text = $"Trạng thái hôm nay: ĐÃ CHECK-IN LÚC {_todayRecord.TimeIn:HH:mm:ss}\nBạn đang trong ca làm việc. Nhấn 'CHECK-OUT' khi kết thúc ca.";
                label_StatusToday.ForeColor = Color.FromArgb(37, 99, 235);
                button_CheckIn.Enabled = false;
                button_CheckOut.Enabled = true;
            }
            else
            {
                label_StatusToday.Text = $"Trạng thái hôm nay: ĐÃ HOÀN TẤT CA LÀM VIỆC\nVào: {_todayRecord.TimeIn:HH:mm:ss} | Ra: {_todayRecord.TimeOut:HH:mm:ss} | Tổng giờ: {_todayRecord.WorkHour:0.##}h";
                label_StatusToday.ForeColor = Color.FromArgb(16, 149, 74);
                button_CheckIn.Enabled = false;
                button_CheckOut.Enabled = false;
            }
        }

        private async Task OnCheckInAsync()
        {
            if (_currentEmployee == null)
            {
                MessageBox.Show("Vui lòng chọn hoặc tra cứu mã nhân viên trước khi Check-in.", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var now = TimeOnly.FromDateTime(DateTime.Now);
            var today = DateOnly.FromDateTime(DateTime.Today);

            int lateMins = 0;
            bool isLate = false;
            var shiftStartTime = _defaultShift?.StartTime ?? new TimeOnly(8, 0);

            if (now > shiftStartTime.AddMinutes(15))
            {
                lateMins = (int)(now - shiftStartTime).TotalMinutes;
                isLate = true;
            }

            var dto = new CreateAttendanceRecordDto
            {
                EmployeeId = _currentEmployee.EmployeeId,
                ShiftId = _defaultShiftId,
                Date = today,
                TimeIn = now,
                Status = "Present",
                IsLate = isLate,
                LateMins = lateMins,
                CheckInSource = "Portal/Kiosk"
            };

            try
            {
                button_CheckIn.Enabled = false;
                _todayRecord = await Program.AttendanceApi.CreateAttendanceRecordAsync(dto);

                string lateMsg = isLate ? $"\n⚠️ Chú ý: Bạn đã đi trễ {lateMins} phút (Quy định vào ca: {shiftStartTime:HH:mm}). Hãy nộp đơn giải trình nếu có lý do chính đáng!" : "\n✅ Bạn đã đến đúng giờ quy định.";
                MessageBox.Show($"Check-in thành công cho {_currentEmployee.FullName} vào lúc {now:HH:mm:ss}!{lateMsg}", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                UpdateActionButtonsStatus();
                await LoadRecentLogsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi Check-in: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                button_CheckIn.Enabled = true;
            }
        }

        private async Task OnCheckOutAsync()
        {
            if (_currentEmployee == null || _todayRecord == null)
            {
                MessageBox.Show("Không tìm thấy bản ghi Check-in của ngày hôm nay để Check-out.", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var now = TimeOnly.FromDateTime(DateTime.Now);
            int earlyMins = 0;
            bool isEarly = false;
            var shiftEndTime = _defaultShift?.EndTime ?? new TimeOnly(17, 0);

            if (now < shiftEndTime)
            {
                earlyMins = (int)(shiftEndTime - now).TotalMinutes;
                isEarly = true;
            }

            decimal workHour = 0;
            if (_todayRecord.TimeIn.HasValue)
            {
                var duration = now - _todayRecord.TimeIn.Value;
                workHour = (decimal)Math.Max(0, duration.TotalHours - (_defaultShift?.BreakMins ?? 60) / 60.0);
            }

            var dto = new UpdateAttendanceRecordDto
            {
                TimeOut = now,
                WorkHour = Math.Round(workHour, 2),
                IsEarlyLeave = isEarly,
                EarlyLeaveMins = earlyMins,
                CheckOutSource = "Portal/Kiosk"
            };

            try
            {
                button_CheckOut.Enabled = false;
                _todayRecord = await Program.AttendanceApi.UpdateAttendanceRecordAsync(_todayRecord.Id, dto);

                string earlyMsg = isEarly ? $"\n⚠️ Chú ý: Bạn đã về sớm {earlyMins} phút (Ca kết thúc lúc {shiftEndTime:HH:mm})." : "\n✅ Bạn đã hoàn thành đủ thời gian ca làm việc.";
                MessageBox.Show($"Check-out thành công cho {_currentEmployee.FullName} vào lúc {now:HH:mm:ss}!\nTổng thời gian làm việc: {workHour:0.##} giờ.{earlyMsg}", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                UpdateActionButtonsStatus();
                await LoadRecentLogsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi Check-out: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                button_CheckOut.Enabled = true;
            }
        }

        private async Task LoadRecentLogsAsync()
        {
            try
            {
                var logs = await Program.AttendanceApi.GetRecentLogsAsync(_currentEmployee?.EmployeeId);
                listView_RecentLogs.Items.Clear();

                foreach (var log in logs)
                {
                    var item = new ListViewItem(log.Date.ToString("dd/MM/yyyy"));
                    item.SubItems.Add(log.TimeIn.HasValue ? log.TimeIn.Value.ToString("HH:mm:ss") : "--:--:--");
                    item.SubItems.Add(log.TimeOut.HasValue ? log.TimeOut.Value.ToString("HH:mm:ss") : "--:--:--");
                    item.SubItems.Add($"{log.WorkHour:0.##}h");

                    string statusStr;
                    Color color;
                    if (log.IsLate && log.IsEarlyLeave)
                    {
                        statusStr = $"Đi trễ ({log.LateMins}p) & Về sớm";
                        color = Color.FromArgb(220, 38, 38);
                    }
                    else if (log.IsLate)
                    {
                        statusStr = $"Đi trễ ({log.LateMins} phút)";
                        color = Color.FromArgb(217, 119, 6);
                    }
                    else if (log.IsEarlyLeave)
                    {
                        statusStr = $"Về sớm ({log.EarlyLeaveMins} phút)";
                        color = Color.FromArgb(217, 119, 6);
                    }
                    else if (log.TimeIn.HasValue && log.TimeOut.HasValue)
                    {
                        statusStr = "Hoàn thành ca";
                        color = Color.FromArgb(22, 163, 74);
                    }
                    else
                    {
                        statusStr = "Đang trong ca";
                        color = Color.FromArgb(37, 99, 235);
                    }

                    item.SubItems.Add(statusStr);
                    item.ForeColor = color;
                    listView_RecentLogs.Items.Add(item);
                }
            }
            catch
            {
            }
        }
    }
}

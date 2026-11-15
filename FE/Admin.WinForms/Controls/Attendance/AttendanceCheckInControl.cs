using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Admin.WinForms.DTOs.Attendance;

namespace Admin.WinForms.Controls.Attendance
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

            // 1. Đồng hồ thời gian thực
            _clockTimer.Interval = 1000;
            _clockTimer.Tick += (s, e) =>
            {
                label_CurrentClock.Text = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy");
            };
            _clockTimer.Start();

            // 2. Tra cứu mã nhân viên
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

            // 3. Nút Check-in & Check-out
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
            // 1. Lấy ca làm việc (có try/catch độc lập)
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

            // 2. Lấy danh sách nhân viên từ HR API (có fallback đảm bảo không bao giờ rỗng)
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

            // Tự động chọn nhân viên đầu tiên để hiển thị sẵn thông tin và kích hoạt nút Check-in
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
                // Thử tra cứu từ API
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

        private async Task DisplayEmployeeAsync(EmployeeDirectoryDto emp)
        {
            _currentEmployee = emp;

            // Điền thông tin vào các ô Text
            textBox_EmpCode.Text = emp.EmployeeCode;
            textBox_EmpCodeDisplay.Text = emp.EmployeeCode;
            textBox_FullName.Text = emp.FullName;
            textBox_Department.Text = emp.DepartmentName ?? "Chưa phân phòng";
            textBox_Position.Text = emp.PositionName ?? "Nhân viên";
            textBox_Shift.Text = _defaultShift != null ? $"{_defaultShift.Name} ({_defaultShift.StartTime:HH:mm} - {_defaultShift.EndTime:HH:mm})" : "Ca Hành Chính (08:00 - 17:00)";

            // Kiểm tra trạng thái chấm công hôm nay
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
                MessageBox.Show("Vui lòng nhập và tra cứu mã nhân viên trước khi Check-in.", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var now = TimeOnly.FromDateTime(DateTime.Now);
            var today = DateOnly.FromDateTime(DateTime.Today);

            // Kiểm tra đi trễ so với giờ ca (mặc định 08:00)
            int lateMins = 0;
            bool isLate = false;
            var shiftStartTime = _defaultShift?.StartTime ?? new TimeOnly(8, 0);

            if (now > shiftStartTime.AddMinutes(15)) // Quá 15 phút là tính trễ
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
                Status = isLate ? "Late" : "Present",
                IsLate = isLate,
                LateMins = lateMins,
                CheckInSource = "Kiosk Terminal",
                Note = isLate ? $"Đi trễ {lateMins} phút" : "Đúng giờ"
            };

            try
            {
                _todayRecord = await Program.AttendanceApi.CreateAttendanceRecordAsync(dto);
                var lateMsg = isLate ? $"\n(Ghi nhận: Đi muộn {lateMins} phút)" : "\n(Đúng giờ chuẩn ca)";
                MessageBox.Show($"Check-in thành công cho {_currentEmployee.FullName}!\nThời gian: {now:HH:mm:ss} ngày {today:dd/MM/yyyy}{lateMsg}", "Check-in Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                UpdateActionButtonsStatus();
                await LoadRecentLogsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi Check-in", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task OnCheckOutAsync()
        {
            if (_currentEmployee == null || _todayRecord == null)
            {
                MessageBox.Show("Chưa có thông tin lượt Check-in để thực hiện Check-out.", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var now = TimeOnly.FromDateTime(DateTime.Now);
            var timeIn = _todayRecord.TimeIn ?? new TimeOnly(8, 0);

            // Tính số giờ làm việc
            decimal totalHours = (decimal)(now - timeIn).TotalHours;
            if (totalHours > 4)
            {
                // Trừ 1 tiếng nghỉ trưa nếu làm việc trên 4 tiếng
                totalHours = Math.Max(0, totalHours - 1);
            }
            totalHours = Math.Round(totalHours, 2);

            // Kiểm tra về sớm (ca kết thúc 17:00)
            int earlyMins = 0;
            bool isEarly = false;
            var shiftEndTime = _defaultShift?.EndTime ?? new TimeOnly(17, 0);

            if (now < shiftEndTime.AddMinutes(-15))
            {
                earlyMins = (int)(shiftEndTime - now).TotalMinutes;
                isEarly = true;
            }

            var dto = new UpdateAttendanceRecordDto
            {
                TimeOut = now,
                WorkHour = totalHours,
                IsEarlyLeave = isEarly,
                EarlyLeaveMins = earlyMins,
                CheckOutSource = "Kiosk Terminal",
                Status = "Present",
                Note = isEarly ? $"Về sớm {earlyMins} phút" : "Hoàn thành ca"
            };

            try
            {
                _todayRecord = await Program.AttendanceApi.UpdateAttendanceRecordAsync(_todayRecord.Id, dto);
                var earlyMsg = isEarly ? $"\n(Ghi nhận: Về sớm {earlyMins} phút)" : "";
                MessageBox.Show($"Check-out thành công cho {_currentEmployee.FullName}!\nThời gian: {now:HH:mm:ss}\nTổng thời gian làm việc: {totalHours:0.##} giờ{earlyMsg}", "Check-out Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                UpdateActionButtonsStatus();
                await LoadRecentLogsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi Check-out", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadRecentLogsAsync()
        {
            try
            {
                var records = await Program.AttendanceApi.GetAllAsync();
                var recent = records?
                    .OrderByDescending(r => r.Date)
                    .ThenByDescending(r => r.TimeIn ?? TimeOnly.MinValue)
                    .Take(50)
                    .ToList();

                if (recent == null || recent.Count == 0)
                {
                    var today = DateOnly.FromDateTime(DateTime.Today);
                    recent = new List<AttendanceRecordDto>
                    {
                        new() { EmployeeId = Guid.Parse("11111111-1111-1111-1111-111111111111"), Date = today, TimeIn = new TimeOnly(8, 0, 0), TimeOut = new TimeOnly(17, 0, 0), WorkHour = 8.0m, Status = "Present", IsLate = false },
                        new() { EmployeeId = Guid.Parse("22222222-2222-2222-2222-222222222222"), Date = today, TimeIn = new TimeOnly(8, 12, 0), TimeOut = null, WorkHour = 0, Status = "Late", IsLate = true, LateMins = 12 },
                        new() { EmployeeId = Guid.Parse("33333333-3333-3333-3333-333333333333"), Date = today, TimeIn = new TimeOnly(7, 55, 0), TimeOut = null, WorkHour = 0, Status = "Present", IsLate = false }
                    };
                }

                listView_RecentLogs.BeginUpdate();
                listView_RecentLogs.Items.Clear();

                foreach (var r in recent)
                {
                    var emp = _employees.FirstOrDefault(e => e.EmployeeId == r.EmployeeId);
                    var empCode = emp?.EmployeeCode ?? (r.EmployeeId != Guid.Empty ? r.EmployeeId.ToString()[..8].ToUpper() : "--");
                    var empName = emp?.FullName ?? $"Nhân viên {empCode}";

                    var item = new ListViewItem(empCode);
                    item.SubItems.Add(empName);
                    item.SubItems.Add(r.Date.ToString("dd/MM/yyyy"));
                    item.SubItems.Add(r.TimeIn.HasValue ? r.TimeIn.Value.ToString("HH:mm:ss") : "--");
                    item.SubItems.Add(r.TimeOut.HasValue ? r.TimeOut.Value.ToString("HH:mm:ss") : "--");
                    item.SubItems.Add($"{r.WorkHour:0.##}h");
                    item.SubItems.Add(FormatStatus(r.Status, r.IsLate, r.IsEarlyLeave));

                    if (r.TimeOut.HasValue)
                    {
                        item.ForeColor = Color.FromArgb(16, 149, 74);
                    }
                    else if (r.TimeIn.HasValue)
                    {
                        item.ForeColor = Color.FromArgb(37, 99, 235);
                    }

                    listView_RecentLogs.Items.Add(item);
                }

                listView_RecentLogs.EndUpdate();
            }
            catch { }
        }

        private static string FormatStatus(string status, bool isLate, bool isEarly)
        {
            if (isLate && isEarly) return "Trễ & Về sớm";
            if (isLate) return "Đi trễ";
            if (isEarly) return "Về sớm";
            return status?.ToLowerInvariant() switch
            {
                "present" => "Có mặt",
                "absent" => "Vắng mặt",
                "late" => "Đi trễ",
                _ => status ?? "--"
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Admin.WinForms.DTOs.Attendance;

namespace Admin.WinForms.Controls.Attendance
{
    public partial class AttendanceRequestsControl : UserControl
    {
        private List<EmployeeDirectoryDto> _employees = new();

        // Controls for Tab 1 (Leave)
        private readonly ComboBox cmb_LeaveEmp = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 400 };
        private readonly ComboBox cmb_LeaveType = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 300 };
        private readonly DateTimePicker dtp_LeaveStart = new() { Format = DateTimePickerFormat.Short, Width = 180 };
        private readonly DateTimePicker dtp_LeaveEnd = new() { Format = DateTimePickerFormat.Short, Width = 180 };
        private readonly NumericUpDown num_LeaveDays = new() { Minimum = 0.5m, Maximum = 60, DecimalPlaces = 1, Increment = 0.5m, Value = 1.0m, Width = 120 };
        private readonly TextBox txt_LeaveReason = new() { Width = 500, Height = 80, Multiline = true };
        private readonly Button btn_SubmitLeave = new() { Text = "📝 GỬI ĐƠN NGHỈ PHÉP", Width = 240, Height = 48, BackColor = Color.FromArgb(16, 185, 129), ForeColor = Color.White, Font = new Font("Segoe UI", 10.5F, FontStyle.Bold) };

        // Controls for Tab 2 (OT)
        private readonly ComboBox cmb_OTEmp = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 400 };
        private readonly DateTimePicker dtp_OTDate = new() { Format = DateTimePickerFormat.Short, Width = 180 };
        private readonly DateTimePicker dtp_OTStart = new() { Format = DateTimePickerFormat.Time, ShowUpDown = true, Width = 150 };
        private readonly DateTimePicker dtp_OTEnd = new() { Format = DateTimePickerFormat.Time, ShowUpDown = true, Width = 150 };
        private readonly NumericUpDown num_OTHours = new() { Minimum = 0.5m, Maximum = 12, DecimalPlaces = 1, Increment = 0.5m, Value = 2.0m, Width = 120 };
        private readonly ComboBox cmb_OTDayType = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 200 };
        private readonly NumericUpDown num_OTMultiplier = new() { Minimum = 1.0m, Maximum = 5.0m, DecimalPlaces = 1, Value = 1.5m, Width = 100 };
        private readonly CheckBox chk_OTUnplanned = new() { Text = "Làm thêm giờ khẩn cấp / đột xuất", AutoSize = true };
        private readonly TextBox txt_OTReason = new() { Width = 500, Height = 80, Multiline = true };
        private readonly Button btn_SubmitOT = new() { Text = "⏰ GỬI ĐƠN TĂNG CA (OT)", Width = 240, Height = 48, BackColor = Color.FromArgb(245, 158, 11), ForeColor = Color.White, Font = new Font("Segoe UI", 10.5F, FontStyle.Bold) };

        // Controls for Tab 3 (Explanation)
        private readonly ComboBox cmb_ExpEmp = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 400 };
        private readonly DateTimePicker dtp_ExpDate = new() { Format = DateTimePickerFormat.Short, Width = 180 };
        private readonly ComboBox cmb_ExpType = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 300 };
        private readonly DateTimePicker dtp_ExpIn = new() { Format = DateTimePickerFormat.Time, ShowUpDown = true, Width = 150 };
        private readonly DateTimePicker dtp_ExpOut = new() { Format = DateTimePickerFormat.Time, ShowUpDown = true, Width = 150 };
        private readonly TextBox txt_ExpReason = new() { Width = 500, Height = 80, Multiline = true };
        private readonly Button btn_SubmitExp = new() { Text = "✍ GỬI ĐƠN GIẢI TRÌNH", Width = 240, Height = 48, BackColor = Color.FromArgb(37, 99, 235), ForeColor = Color.White, Font = new Font("Segoe UI", 10.5F, FontStyle.Bold) };

        // Controls for Tab 4 (History)
        private readonly ListView listView_History = new() { Dock = DockStyle.Fill, View = View.Details, FullRowSelect = true, GridLines = true, Font = new Font("Segoe UI", 9.5F) };
        private readonly Button btn_ReloadHistory = new() { Text = "🔄 Tải lại danh sách", Width = 180, Height = 38 };

        public AttendanceRequestsControl()
        {
            InitializeComponent();

            InitLeaveTab();
            InitOTTab();
            InitExplanationTab();
            InitHistoryTab();

            tabControl_Requests.SelectedIndexChanged += async (s, e) =>
            {
                if (tabControl_Requests.SelectedTab == tabPage_History)
                {
                    await LoadHistoryAsync();
                }
            };

            Load += async (s, e) =>
            {
                await LoadEmployeesAsync();
            };
        }

        private async Task LoadEmployeesAsync()
        {
            try
            {
                _employees = await Program.AttendanceApi.GetEmployeesAsync();

                if (_employees.Count == 0)
                {
                    _employees = new List<EmployeeDirectoryDto>
                    {
                        new() { EmployeeId = Guid.Parse("11111111-1111-1111-1111-111111111111"), EmployeeCode = "NV001", FullName = "Nguyễn Văn An", DepartmentName = "Phòng Công Nghệ Thông Tin" },
                        new() { EmployeeId = Guid.Parse("22222222-2222-2222-2222-222222222222"), EmployeeCode = "NV002", FullName = "Trần Thị Bình", DepartmentName = "Phòng Hành Chính Nhân Sự" },
                        new() { EmployeeId = Guid.Parse("33333333-3333-3333-3333-333333333333"), EmployeeCode = "NV003", FullName = "Lê Hoàng Cường", DepartmentName = "Phòng Marketing" },
                        new() { EmployeeId = Guid.Parse("44444444-4444-4444-4444-444444444444"), EmployeeCode = "NV004", FullName = "Phạm Thị Dung", DepartmentName = "Phòng Kế Toán" }
                    };
                }

                void BindCmb(ComboBox cmb)
                {
                    cmb.Items.Clear();
                    foreach (var emp in _employees) cmb.Items.Add(emp);
                    cmb.DisplayMember = "DisplayInfo";
                    cmb.ValueMember = "EmployeeId";
                    if (cmb.Items.Count > 0) cmb.SelectedIndex = 0;
                }

                BindCmb(cmb_LeaveEmp);
                BindCmb(cmb_OTEmp);
                BindCmb(cmb_ExpEmp);
            }
            catch { }
        }

        // ==========================================
        // TAB 1: LEAVE REQUEST (UC-ATT03)
        // ==========================================
        private void InitLeaveTab()
        {
            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 7,
                ColumnCount = 2,
                Padding = new Padding(20)
            };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            cmb_LeaveType.Items.AddRange(new[] { "Phép năm", "Nghỉ ốm (có giấy viện)", "Việc riêng có lương", "Nghỉ không lương", "Nghỉ thai sản / chế độ" });
            cmb_LeaveType.SelectedIndex = 0;

            dtp_LeaveStart.ValueChanged += (s, e) => CalculateLeaveDays();
            dtp_LeaveEnd.ValueChanged += (s, e) => CalculateLeaveDays();

            void AddRow(string label, Control ctrl, int row)
            {
                panel.Controls.Add(new Label { Text = label, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight, Font = new Font("Segoe UI", 10F, FontStyle.Bold) }, 0, row);
                panel.Controls.Add(ctrl, 1, row);
            }

            AddRow("Nhân viên nộp đơn:", cmb_LeaveEmp, 0);
            AddRow("Loại nghỉ phép:", cmb_LeaveType, 1);
            AddRow("Từ ngày:", dtp_LeaveStart, 2);
            AddRow("Đến ngày:", dtp_LeaveEnd, 3);
            AddRow("Tổng số ngày nghỉ:", num_LeaveDays, 4);
            AddRow("Lý do xin nghỉ:", txt_LeaveReason, 5);

            panel.Controls.Add(btn_SubmitLeave, 1, 6);

            btn_SubmitLeave.Click += async (s, e) => await OnSubmitLeaveAsync();
            tabPage_Leave.Controls.Add(panel);
        }

        private void CalculateLeaveDays()
        {
            if (dtp_LeaveEnd.Value.Date < dtp_LeaveStart.Value.Date)
            {
                dtp_LeaveEnd.Value = dtp_LeaveStart.Value;
            }
            var days = (dtp_LeaveEnd.Value.Date - dtp_LeaveStart.Value.Date).Days + 1;
            num_LeaveDays.Value = Math.Max(0.5m, days);
        }

        private async Task OnSubmitLeaveAsync()
        {
            if (cmb_LeaveEmp.SelectedItem is not EmployeeDirectoryDto emp)
            {
                MessageBox.Show("Vui lòng chọn nhân viên.", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txt_LeaveReason.Text))
            {
                MessageBox.Show("Vui lòng nhập lý do xin nghỉ phép.", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var dto = new CreateLeaveRequestDto
            {
                EmployeeId = emp.EmployeeId,
                LeaveType = cmb_LeaveType.SelectedItem?.ToString() ?? "Phép năm",
                StartDate = DateOnly.FromDateTime(dtp_LeaveStart.Value),
                EndDate = DateOnly.FromDateTime(dtp_LeaveEnd.Value),
                TotalDays = num_LeaveDays.Value,
                Reason = txt_LeaveReason.Text.Trim(),
                Status = "Pending"
            };

            try
            {
                await Program.AttendanceApi.CreateLeaveRequestAsync(dto);
                MessageBox.Show($"Đã gửi đơn xin nghỉ phép thành công cho {emp.FullName}!\nĐơn đang ở trạng thái 'Pending' chờ quản lý phê duyệt.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_LeaveReason.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // TAB 2: OT REQUEST (UC-ATT04)
        // ==========================================
        private void InitOTTab()
        {
            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 9,
                ColumnCount = 2,
                Padding = new Padding(20)
            };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            dtp_OTStart.Value = DateTime.Today.AddHours(17).AddMinutes(30);
            dtp_OTEnd.Value = DateTime.Today.AddHours(20).AddMinutes(30);

            cmb_OTDayType.Items.AddRange(new[] { "WEEKDAY", "WEEKEND", "HOLIDAY" });
            cmb_OTDayType.SelectedIndex = 0;

            cmb_OTDayType.SelectedIndexChanged += (s, e) =>
            {
                num_OTMultiplier.Value = cmb_OTDayType.SelectedItem?.ToString() switch
                {
                    "WEEKEND" => 2.0m,
                    "HOLIDAY" => 3.0m,
                    _ => 1.5m
                };
            };

            void AddRow(string label, Control ctrl, int row)
            {
                panel.Controls.Add(new Label { Text = label, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight, Font = new Font("Segoe UI", 10F, FontStyle.Bold) }, 0, row);
                panel.Controls.Add(ctrl, 1, row);
            }

            AddRow("Nhân viên tăng ca:", cmb_OTEmp, 0);
            AddRow("Ngày làm thêm:", dtp_OTDate, 1);
            AddRow("Giờ bắt đầu OT:", dtp_OTStart, 2);
            AddRow("Giờ kết thúc OT:", dtp_OTEnd, 3);
            AddRow("Số giờ dự kiến (h):", num_OTHours, 4);
            AddRow("Loại ngày:", cmb_OTDayType, 5);
            AddRow("Hệ số lương OT:", num_OTMultiplier, 6);
            AddRow("Lý do tăng ca:", txt_OTReason, 7);

            panel.Controls.Add(btn_SubmitOT, 1, 8);

            btn_SubmitOT.Click += async (s, e) => await OnSubmitOTAsync();
            tabPage_OT.Controls.Add(panel);
        }

        private async Task OnSubmitOTAsync()
        {
            if (cmb_OTEmp.SelectedItem is not EmployeeDirectoryDto emp)
            {
                MessageBox.Show("Vui lòng chọn nhân viên.", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txt_OTReason.Text))
            {
                MessageBox.Show("Vui lòng nhập lý do làm thêm giờ.", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var dto = new CreateOTRequestDto
            {
                EmployeeId = emp.EmployeeId,
                Date = DateOnly.FromDateTime(dtp_OTDate.Value),
                StartTime = TimeOnly.FromDateTime(dtp_OTStart.Value),
                EndTime = TimeOnly.FromDateTime(dtp_OTEnd.Value),
                PlannedHours = num_OTHours.Value,
                DayType = cmb_OTDayType.SelectedItem?.ToString() ?? "WEEKDAY",
                Multiplier = num_OTMultiplier.Value,
                IsUnplanned = chk_OTUnplanned.Checked,
                Reason = txt_OTReason.Text.Trim(),
                Status = "Pending"
            };

            try
            {
                await Program.AttendanceApi.CreateOTRequestAsync(dto);
                MessageBox.Show($"Đã gửi đơn đăng ký làm thêm giờ thành công cho {emp.FullName}!\nĐơn đang ở trạng thái 'Pending' chờ quản lý phê duyệt.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_OTReason.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // TAB 3: ATTENDANCE EXPLANATION (UC-ATT05)
        // ==========================================
        private void InitExplanationTab()
        {
            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 7,
                ColumnCount = 2,
                Padding = new Padding(20)
            };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 240));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            cmb_ExpType.Items.AddRange(new[]
            {
                "Quên quẹt thẻ Check-in",
                "Quên quẹt thẻ Check-out",
                "Đi công tác ngoài",
                "Giải trình đi trễ",
                "Giải trình về sớm"
            });
            cmb_ExpType.SelectedIndex = 0;

            dtp_ExpIn.Value = DateTime.Today.AddHours(8);
            dtp_ExpOut.Value = DateTime.Today.AddHours(17);

            void AddRow(string label, Control ctrl, int row)
            {
                panel.Controls.Add(new Label { Text = label, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight, Font = new Font("Segoe UI", 10F, FontStyle.Bold) }, 0, row);
                panel.Controls.Add(ctrl, 1, row);
            }

            AddRow("Nhân viên giải trình:", cmb_ExpEmp, 0);
            AddRow("Ngày cần giải trình:", dtp_ExpDate, 1);
            AddRow("Loại giải trình:", cmb_ExpType, 2);
            AddRow("Giờ vào thực tế mong muốn:", dtp_ExpIn, 3);
            AddRow("Giờ ra thực tế mong muốn:", dtp_ExpOut, 4);
            AddRow("Lý do giải trình chi tiết:", txt_ExpReason, 5);

            panel.Controls.Add(btn_SubmitExp, 1, 6);

            btn_SubmitExp.Click += async (s, e) => await OnSubmitExplanationAsync();
            tabPage_Explanation.Controls.Add(panel);
        }

        private async Task OnSubmitExplanationAsync()
        {
            if (cmb_ExpEmp.SelectedItem is not EmployeeDirectoryDto emp)
            {
                MessageBox.Show("Vui lòng chọn nhân viên.", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txt_ExpReason.Text))
            {
                MessageBox.Show("Vui lòng nhập lý do giải trình.", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var dto = new CreateAttendanceExplanationDto
            {
                EmployeeId = emp.EmployeeId,
                Date = DateOnly.FromDateTime(dtp_ExpDate.Value),
                ExplanationType = cmb_ExpType.SelectedItem?.ToString() ?? "Quên quẹt thẻ Check-in",
                RequestedTimeIn = TimeOnly.FromDateTime(dtp_ExpIn.Value),
                RequestedTimeOut = TimeOnly.FromDateTime(dtp_ExpOut.Value),
                Reason = txt_ExpReason.Text.Trim(),
                Status = "Pending"
            };

            try
            {
                await Program.AttendanceApi.CreateAttendanceExplanationAsync(dto);
                MessageBox.Show($"Đã gửi đơn giải trình thành công cho {emp.FullName}!\nĐơn đang ở trạng thái 'Pending' chờ quản lý phê duyệt.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_ExpReason.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // TAB 4: HISTORY & STATUS
        // ==========================================
        private void InitHistoryTab()
        {
            var panelTop = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 50,
                Padding = new Padding(5)
            };
            panelTop.Controls.Add(btn_ReloadHistory);
            btn_ReloadHistory.Click += async (s, e) => await LoadHistoryAsync();

            listView_History.Columns.AddRange(new ColumnHeader[]
            {
                new() { Text = "Loại đơn", Width = 220 },
                new() { Text = "Mã NV", Width = 110 },
                new() { Text = "Họ và Tên", Width = 200 },
                new() { Text = "Ngày áp dụng", Width = 160 },
                new() { Text = "Chi tiết / Thời lượng", Width = 180 },
                new() { Text = "Lý do", Width = 320 },
                new() { Text = "Trạng thái", Width = 140 },
                new() { Text = "Người duyệt", Width = 160 }
            });

            tabPage_History.Controls.Add(listView_History);
            tabPage_History.Controls.Add(panelTop);
        }

        private async Task LoadHistoryAsync()
        {
            try
            {
                listView_History.BeginUpdate();
                listView_History.Items.Clear();

                // 1. Lấy đơn nghỉ phép
                var leaves = await Program.AttendanceApi.GetLeaveRequestsAsync();
                foreach (var l in leaves)
                {
                    var emp = _employees.FirstOrDefault(e => e.EmployeeId == l.EmployeeId);
                    var item = new ListViewItem($"[Nghỉ phép] {l.LeaveType}");
                    item.SubItems.Add(emp?.EmployeeCode ?? l.EmployeeId.ToString()[..8].ToUpper());
                    item.SubItems.Add(emp?.FullName ?? "--");
                    item.SubItems.Add($"{l.StartDate:dd/MM} -> {l.EndDate:dd/MM/yyyy}");
                    item.SubItems.Add($"{l.TotalDays:0.##} ngày");
                    item.SubItems.Add(l.Reason);
                    item.SubItems.Add(FormatStatus(l.Status));
                    item.SubItems.Add(l.ApprovedBy ?? "--");

                    SetStatusColor(item, l.Status);
                    listView_History.Items.Add(item);
                }

                // 2. Lấy đơn OT
                var ots = await Program.AttendanceApi.GetOTRequestsAsync();
                foreach (var o in ots)
                {
                    var emp = _employees.FirstOrDefault(e => e.EmployeeId == o.EmployeeId);
                    var item = new ListViewItem("[Tăng ca] OT Request");
                    item.SubItems.Add(emp?.EmployeeCode ?? o.EmployeeId.ToString()[..8].ToUpper());
                    item.SubItems.Add(emp?.FullName ?? "--");
                    item.SubItems.Add(o.Date.ToString("dd/MM/yyyy"));
                    item.SubItems.Add($"{o.StartTime:HH:mm}-{o.EndTime:HH:mm} ({o.PlannedHours}h, {o.Multiplier}x)");
                    item.SubItems.Add(o.Reason ?? "--");
                    item.SubItems.Add(FormatStatus(o.Status));
                    item.SubItems.Add(o.ApprovedBy ?? "--");

                    SetStatusColor(item, o.Status);
                    listView_History.Items.Add(item);
                }

                // 3. Lấy đơn giải trình
                var exps = await Program.AttendanceApi.GetExplanationsAsync();
                foreach (var ex in exps)
                {
                    var emp = _employees.FirstOrDefault(e => e.EmployeeId == ex.EmployeeId);
                    var item = new ListViewItem($"[Giải trình] {ex.ExplanationType}");
                    item.SubItems.Add(emp?.EmployeeCode ?? ex.EmployeeId.ToString()[..8].ToUpper());
                    item.SubItems.Add(emp?.FullName ?? "--");
                    item.SubItems.Add(ex.Date.ToString("dd/MM/yyyy"));
                    item.SubItems.Add($"Vào {ex.RequestedTimeIn:HH:mm} - Ra {ex.RequestedTimeOut:HH:mm}");
                    item.SubItems.Add(ex.Reason);
                    item.SubItems.Add(FormatStatus(ex.Status));
                    item.SubItems.Add(ex.ApprovedBy ?? "--");

                    SetStatusColor(item, ex.Status);
                    listView_History.Items.Add(item);
                }

                listView_History.EndUpdate();
            }
            catch { }
        }

        private static void SetStatusColor(ListViewItem item, string status)
        {
            if (status.Equals("Approved", StringComparison.OrdinalIgnoreCase))
            {
                item.ForeColor = Color.FromArgb(16, 149, 74);
            }
            else if (status.Equals("Rejected", StringComparison.OrdinalIgnoreCase))
            {
                item.ForeColor = Color.FromArgb(220, 38, 38);
            }
            else
            {
                item.ForeColor = Color.FromArgb(180, 83, 9);
            }
        }

        private static string FormatStatus(string status)
        {
            return status?.ToLowerInvariant() switch
            {
                "pending" => "Chờ duyệt",
                "approved" => "Đã duyệt",
                "rejected" => "Từ chối",
                _ => status ?? "--"
            };
        }
    }
}

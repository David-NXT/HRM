using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Attendance.WinForms.Controls;
using Attendance.WinForms.DTOs;

namespace Attendance.WinForms
{
    public partial class MainForm : Form
    {
        private readonly System.Windows.Forms.Timer _clockTimer = new();
        private List<EmployeeDirectoryDto> _employees = new();
        private EmployeeDirectoryDto? _currentEmployee;

        private AttendanceCheckInControl? _checkInControl;
        private AttendanceRequestsControl? _requestsControl;
        private Panel? _rulesPanel;

        private Button? _activeNavButton;

        public MainForm()
        {
            InitializeComponent();
            SetupCustomUI();

            _clockTimer.Interval = 1000;
            _clockTimer.Tick += (s, e) =>
            {
                if (label_Clock != null)
                {
                    label_Clock.Text = DateTime.Now.ToString("HH:mm:ss  |  dd/MM/yyyy");
                }
            };
            _clockTimer.Start();

            Load += async (s, e) =>
            {
                label_Clock.Text = DateTime.Now.ToString("HH:mm:ss  |  dd/MM/yyyy");
                await LoadEmployeesAsync();
            };
        }

        private void SetupCustomUI()
        {
            btnNav_CheckIn_Click(button_NavCheckIn, EventArgs.Empty);
        }

        private async Task LoadEmployeesAsync()
        {
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

        private async void comboBox_Employees_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_Employees.SelectedItem is EmployeeDirectoryDto emp)
            {
                _currentEmployee = emp;
                label_EmpBadge.Text = $"👤 {emp.FullName} ({emp.EmployeeCode}) - {emp.DepartmentName ?? "Chưa phân phòng"} | {emp.PositionName ?? "Nhân viên"}";

                if (_checkInControl != null)
                {
                    await _checkInControl.SelectEmployeeAsync(emp);
                }

                if (_requestsControl != null)
                {
                    _requestsControl.SelectEmployee(emp);
                }
            }
        }

        private void SetActiveNavButton(Button btn)
        {
            if (_activeNavButton != null)
            {
                _activeNavButton.BackColor = Color.FromArgb(30, 41, 59);
                _activeNavButton.ForeColor = Color.FromArgb(203, 213, 225);
            }

            _activeNavButton = btn;
            _activeNavButton.BackColor = Color.FromArgb(37, 99, 235);
            _activeNavButton.ForeColor = Color.White;
        }

        private void btnNav_CheckIn_Click(object sender, EventArgs e)
        {
            SetActiveNavButton((Button)sender);
            label_TabTitle.Text = "⏱️ CHẤM CÔNG HÀNG NGÀY (CHECK-IN / CHECK-OUT)";
            label_TabDesc.Text = "Quẹt thẻ bắt đầu và kết thúc ca làm việc trực tuyến theo thời gian thực.";

            if (_checkInControl == null)
            {
                _checkInControl = new AttendanceCheckInControl { Dock = DockStyle.Fill };
            }

            LoadContentControl(_checkInControl);

            if (_currentEmployee != null)
            {
                _ = _checkInControl.SelectEmployeeAsync(_currentEmployee);
            }
        }

        private void btnNav_Requests_Click(object sender, EventArgs e)
        {
            SetActiveNavButton((Button)sender);
            label_TabTitle.Text = "📝 TẠO ĐƠN TỪ & ĐỀ XUẤT (REQUESTS)";
            label_TabDesc.Text = "Nộp đơn xin nghỉ phép, đăng ký tăng ca (OT), đơn giải trình đi trễ/về sớm và theo dõi trạng thái phê duyệt.";

            if (_requestsControl == null)
            {
                _requestsControl = new AttendanceRequestsControl { Dock = DockStyle.Fill };
            }

            LoadContentControl(_requestsControl);

            if (_currentEmployee != null)
            {
                _requestsControl.SelectEmployee(_currentEmployee);
            }
        }

        private void btnNav_Rules_Click(object sender, EventArgs e)
        {
            SetActiveNavButton((Button)sender);
            label_TabTitle.Text = "📋 NỘI QUY & QUY ĐỊNH CHẤM CÔNG";
            label_TabDesc.Text = "Các quy định về giờ giấc làm việc, mức phạt đi trễ và quy trình phê duyệt đơn từ nội bộ.";

            if (_rulesPanel == null)
            {
                _rulesPanel = CreateRulesPanel();
            }

            LoadContentControl(_rulesPanel);
        }

        private void LoadContentControl(Control ctrl)
        {
            panel_MainContent.SuspendLayout();
            panel_MainContent.Controls.Clear();
            ctrl.Dock = DockStyle.Fill;
            panel_MainContent.Controls.Add(ctrl);
            panel_MainContent.ResumeLayout();
        }

        private Panel CreateRulesPanel()
        {
            var pnl = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(30), BackColor = Color.White };

            var table = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 1, RowCount = 4, Padding = new Padding(10) };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            table.Controls.Add(CreateRuleCard("⏰ 1. Thời gian làm việc tiêu chuẩn",
                "• Ca Hành Chính: Sáng 08:00 - 12:00 | Chiều 13:00 - 17:00 (Nghỉ trưa 60 phút).\n" +
                "• Giờ vào ca được chấm hợp lệ từ trước 15 phút đến đúng giờ quy định.\n" +
                "• Quá 08:15 được tính là Đi trễ (Late). Quẹt thẻ trước 17:00 được tính là Về sớm (Early Leave).",
                Color.FromArgb(239, 246, 255), Color.FromArgb(37, 99, 235)), 0, 0);

            table.Controls.Add(CreateRuleCard("🏖️ 2. Quy định Đơn Nghỉ phép (Leave Request)",
                "• Nghỉ phép năm: Nộp đơn trước ít nhất 01 ngày làm việc để Quản lý trực tiếp phê duyệt.\n" +
                "• Nghỉ ốm / Khẩn cấp: Có thể nộp bổ sung kèm giấy khám bệnh trong vòng 24 giờ kể từ khi đi làm lại.\n" +
                "• Số ngày nghỉ được tính theo hạn mức phép năm còn lại trong hồ sơ nhân sự.",
                Color.FromArgb(240, 253, 244), Color.FromArgb(16, 185, 129)), 0, 1);

            table.Controls.Add(CreateRuleCard("⚡ 3. Quy định Đăng ký Làm thêm giờ (Overtime - OT)",
                "• Đăng ký trước ca OT ít nhất 4 tiếng với lý do công việc cụ thể.\n" +
                "• Hệ số làm thêm: Ngày thường x1.5 | Ngày nghỉ cuối tuần x2.0 | Ngày lễ tết x3.0.\n" +
                "• Giờ OT chỉ được đồng bộ tính lương sau khi cấp Quản lý phê duyệt thành công.",
                Color.FromArgb(254, 252, 232), Color.FromArgb(202, 138, 4)), 0, 2);

            table.Controls.Add(CreateRuleCard("✍️ 4. Quy định Đơn Giải trình Chấm công (Explanation)",
                "• Áp dụng khi: Quên quẹt thẻ vào/ra, thiết bị chấm công lỗi, đi công tác đột xuất bên ngoài.\n" +
                "• Thời hạn gửi giải trình: Tối đa 48 giờ kể từ ngày phát sinh sự cố.\n" +
                "• Sau khi duyệt, hệ thống sẽ tự động cập nhật lại giờ công hợp lệ cho bảng lương.",
                Color.FromArgb(248, 250, 252), Color.FromArgb(100, 116, 139)), 0, 3);

            pnl.Controls.Add(table);
            return pnl;
        }

        private Panel CreateRuleCard(string title, string content, Color bg, Color border)
        {
            var card = new Panel { Dock = DockStyle.Top, BackColor = bg, Padding = new Padding(18), Margin = new Padding(0, 0, 0, 15), Height = 145 };
            var lblTitle = new Label { Text = title, Font = new Font("Segoe UI", 11.5F, FontStyle.Bold), ForeColor = border, Dock = DockStyle.Top, Height = 32 };
            var lblContent = new Label { Text = content, Font = new Font("Segoe UI", 10F, FontStyle.Regular), ForeColor = Color.FromArgb(51, 65, 85), Dock = DockStyle.Fill };
            card.Controls.Add(lblContent);
            card.Controls.Add(lblTitle);
            return card;
        }
    }
}

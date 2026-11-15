using Admin.WinForms.Controls.Attendance;
using Admin.WinForms.Controls.Dashboard;
using Admin.WinForms.Controls.HR;
using Admin.WinForms.Controls.Payroll;
using Admin.WinForms.Controls.Project;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Admin.WinForms
{
    public partial class MainForm : Form
    {
        // Biến lưu trữ nút đang được chọn (Active) hiện tại
        private Button? currentActiveButton = null;

        public MainForm()
        {
            InitializeComponent();
            ConfigureLayout();
        }

        private void ConfigureLayout()
        {
            // Preserve the shared navigation, but let content follow window size.
            var shell = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2, Margin = Padding.Empty };
            shell.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300));
            shell.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            shell.RowStyles.Add(new RowStyle(SizeType.Absolute, 135));
            shell.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            Controls.Clear();
            shell.Controls.Add(panel_Sidebar, 0, 0);
            shell.SetRowSpan(panel_Sidebar, 2);
            shell.Controls.Add(panel_header, 1, 0);
            shell.Controls.Add(panel_Content, 1, 1);
            Controls.Add(shell);
            panel_Sidebar.Dock = panel_header.Dock = panel_Content.Dock = DockStyle.Fill;
            panel_Sidebar.Margin = panel_header.Margin = panel_Content.Margin = Padding.Empty;
            flowLayoutPanel1.Visible = flowLayoutPanel2.Visible = false;
            MinimumSize = new Size(1080, 680);
            ClientSize = new Size(1440, 850);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "HRM - Quản trị nhân sự";
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Mặc định khi mở app sẽ chọn sẵn nút Dashboard
            button_Dashboard_Click(button_Dashboard, EventArgs.Empty);
        }

        // Hàm xử lý đổi màu Active cho nút được chọn
        private void ActivateButton(Button selectedButton)
        {
            // 1. Trả nút cũ về trạng thái ban đầu (Transparent)
            if (currentActiveButton != null)
            {
                currentActiveButton.BackColor = Color.Transparent;
                currentActiveButton.ForeColor = Color.White;
            }

            // 2. Gán nút hiện tại thành nút active mới
            currentActiveButton = selectedButton;

            // 3. Đổi màu nút được chọn sang màu xanh dương hiện đại (Active)
            currentActiveButton.BackColor = Color.FromArgb(35, 83, 166); // Mã màu xanh dương chuẩn Admin
            currentActiveButton.ForeColor = Color.White;
        }

        // Sự kiện Click cho từng nút trên Sidebar
        private void button_Dashboard_Click(object sender, EventArgs e)
        {
            ActivateButton((Button)sender);
            label_Title.Text = "Dashboard";
            label_Description.Text = "Tổng quan hoạt động và các chỉ số trọng yếu của hệ thống HR & Payroll.";
            LoadControl(new DashboardControl());
        }

        public void NavigateTo(string target)
        {
            switch (target.ToLowerInvariant())
            {
                case "employee":
                case "hr":
                    button_Employee_Click(button_Employee, EventArgs.Empty);
                    break;
                case "attendance":
                    button_Attendance_Click(button_Attendance, EventArgs.Empty);
                    break;
                case "payroll":
                    button_Payroll_Click(button_Payroll, EventArgs.Empty);
                    break;
                case "project":
                    button_Project_Click(button_Project, EventArgs.Empty);
                    break;
                case "contract":
                    button_Contract_Work_Click(button_Contract_Work, EventArgs.Empty);
                    break;
                case "org":
                case "organization":
                    button_Organization_Click(button_Organization, EventArgs.Empty);
                    break;
                default:
                    button_Dashboard_Click(button_Dashboard, EventArgs.Empty);
                    break;
            }
        }

        private void button_Employee_Click(object sender, EventArgs e)
        {
            ActivateButton((Button)sender);
            label_Title.Text = "Quản lý nhân sự";
            label_Description.Text = "Hồ sơ, tiếp nhận, bàn giao nghỉ việc, duyệt cập nhật và cảnh báo hợp đồng.";
            LoadControl(new HrWorkspaceControl());
        }

        private void button_Contract_Work_Click(object sender, EventArgs e)
        {
            ActivateButton((Button)sender);
            label_Title.Text = "Contract & Work Management";
            label_Description.Text = "Manage employee contracts, work assignments, and related information.";
            LoadControl(new ContractWorkControl());
        }

        private void button_Organization_Click(object sender, EventArgs e)
        {
            ActivateButton((Button)sender);
            label_Title.Text = "Organization Management";
            label_Description.Text = "Manage organizational structure, departments, and related information.";
            LoadControl(new OrganizationManagementControl());
        }

        private void button_Attendance_Click(object sender, EventArgs e)
        {
            ActivateButton((Button)sender);
            label_Title.Text = "Attendance Management";
            label_Description.Text = "Manage employee attendance records, shifts, and related information.";
            LoadControl(new AttendanceOverviewControl());
        }

        private void button_Payroll_Click(object sender, EventArgs e)
        {
            ActivateButton((Button)sender);
            label_Title.Text = "Payroll Management";
            label_Description.Text = "Manage employee payroll, salary calculations, and related information.";
            LoadControl(new PayrollOverviewControl());
        }

        private void button_Project_Click(object sender, EventArgs e)
        {
            ActivateButton((Button)sender);
            label_Title.Text = "Project Management";
            label_Description.Text = "Manage projects, tasks, timesheets, and approved labor costs.";
            LoadControl(new ProjectOverviewControl());
        }

        private void button_System_Click(object sender, EventArgs e)
        {
            ActivateButton((Button)sender);
            label_Title.Text = "System Management";
            label_Description.Text = "Manage system settings, configurations, and related information.";
            LoadPlaceholder("System", "Màn hình quản trị hệ thống chưa được tích hợp.");
        }
        private void LoadPlaceholder(string title, string description)
        {
            var page = new UserControl();
            page.Controls.Add(new Label { Text = title + Environment.NewLine + Environment.NewLine + description,
                Dock = DockStyle.Fill, Padding = new Padding(30), Font = new Font("Segoe UI", 12F) });
            LoadControl(page);
        }

        private void LoadControl(UserControl control)
        {
            panel_Content.SuspendLayout();
            try
            {
                // Disposing the previous page cancels its pending API requests.
                foreach (Control previous in panel_Content.Controls.Cast<Control>().ToArray())
                    previous.Dispose();
                panel_Content.Controls.Clear();
                control.Dock = DockStyle.Fill;
                panel_Content.Controls.Add(control);
            }
            finally
            {
                panel_Content.ResumeLayout();
            }
        }
    }
}

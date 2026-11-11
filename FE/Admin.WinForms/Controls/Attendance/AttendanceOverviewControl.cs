using System;
using System.Drawing;
using System.Windows.Forms;

namespace Admin.WinForms.Controls.Attendance
{
    public partial class AttendanceOverviewControl : UserControl
    {
        private Button? currentActiveButton = null;

        public AttendanceOverviewControl()
        {
            InitializeComponent();

            // Mặc định khi mở giao diện Chấm công, load trang con Overview
            ShowPage(new AttendanceOverviewPageControl(), button_Overview);
        }

        private void btnOverview_Click(object sender, EventArgs e)
        {
            ShowPage(new AttendanceOverviewPageControl(), (Button)sender);
        }

        private void btnApprovals_Click(object sender, EventArgs e)
        {
            ShowPage(new AttendanceApprovalControl(), (Button)sender);
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            ShowPage(new AttendanceReportControl(), (Button)sender);
        }

        private void ShowPage(UserControl page, Button selectedButton)
        {
            // 1. Cập nhật màu sắc Active cho nút navigation
            if (currentActiveButton != null)
            {
                currentActiveButton.BackColor = Color.Transparent;
                currentActiveButton.ForeColor = Color.Black;
                currentActiveButton.Font = new Font(currentActiveButton.Font, FontStyle.Regular);
            }

            currentActiveButton = selectedButton;
            currentActiveButton.BackColor = Color.FromArgb(35, 83, 166);
            currentActiveButton.ForeColor = Color.White;
            currentActiveButton.Font = new Font(currentActiveButton.Font, FontStyle.Bold);

            // 2. Chuyển đổi UserControl con trong panel_Attendance
            panel_Attendance.SuspendLayout();

            foreach (Control control in panel_Attendance.Controls)
            {
                control.Dispose();
            }

            panel_Attendance.Controls.Clear();

            page.Dock = DockStyle.Fill;
            panel_Attendance.Controls.Add(page);

            panel_Attendance.ResumeLayout();
        }
    }
}

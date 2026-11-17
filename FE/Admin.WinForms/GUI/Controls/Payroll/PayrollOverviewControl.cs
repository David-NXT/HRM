using System;
using System.Drawing;
using System.Windows.Forms;

namespace Admin.WinForms.GUI.Controls.Payroll
{
    public partial class PayrollOverviewControl : UserControl
    {
        private Button? _currentActiveButton = null;

        public PayrollOverviewControl()
        {
            InitializeComponent();

            // Mặc định nạp trang Bảng lương
            ShowPage(new PayrollTableControl(), button_Table);
        }

        private void button_Table_Click(object? sender, EventArgs e)
        {
            ShowPage(new PayrollTableControl(), (Button)sender!);
        }

        private void button_Adjustments_Click(object? sender, EventArgs e)
        {
            ShowPage(new PayrollAdjustmentControl(), (Button)sender!);
        }

        private void button_Approval_Click(object? sender, EventArgs e)
        {
            ShowPage(new PayrollApprovalPaymentControl(), (Button)sender!);
        }

        private void button_Payslip_Click(object? sender, EventArgs e)
        {
            ShowPage(new PayslipViewerControl(), (Button)sender!);
        }

        private void ShowPage(UserControl page, Button selectedButton)
        {
            // 1. Cập nhật màu sắc Active cho tab navigation
            if (_currentActiveButton != null)
            {
                _currentActiveButton.BackColor = Color.Transparent;
                _currentActiveButton.ForeColor = Color.Black;
                _currentActiveButton.Font = new Font(_currentActiveButton.Font, FontStyle.Regular);
            }

            _currentActiveButton = selectedButton;
            _currentActiveButton.BackColor = Color.FromArgb(35, 83, 166);
            _currentActiveButton.ForeColor = Color.White;
            _currentActiveButton.Font = new Font(_currentActiveButton.Font, FontStyle.Bold);

            // 2. Chuyển đổi UserControl con trong panel_Payroll
            panel_Payroll.SuspendLayout();

            foreach (Control control in panel_Payroll.Controls)
            {
                control.Dispose();
            }

            panel_Payroll.Controls.Clear();

            page.Dock = DockStyle.Fill;
            panel_Payroll.Controls.Add(page);

            panel_Payroll.ResumeLayout();
        }
    }
}

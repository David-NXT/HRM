#nullable disable
namespace Admin.WinForms.Controls.HR;

partial class HrWorkspaceControl
{
    private void InitializeComponent()
    {
        tabs = new TabControl();
        employeesPage = new TabPage();
        employeeManagementControl = new EmployeeManagementControl();
        offboardingPage = new TabPage();
        offboardingControl = new OffboardingControl();
        reviewPage = new TabPage();
        profileReviewControl = new ProfileReviewControl();
        alertsPage = new TabPage();
        contractAlertsControl = new ContractAlertsControl();
        tabs.SuspendLayout();
        employeesPage.SuspendLayout();
        offboardingPage.SuspendLayout();
        reviewPage.SuspendLayout();
        alertsPage.SuspendLayout();
        SuspendLayout();
        // 
        // tabs
        // 
        tabs.Controls.Add(employeesPage);
        tabs.Controls.Add(offboardingPage);
        tabs.Controls.Add(reviewPage);
        tabs.Controls.Add(alertsPage);
        tabs.Dock = DockStyle.Fill;
        tabs.Location = new Point(0, 0);
        tabs.Name = "tabs";
        tabs.SelectedIndex = 0;
        tabs.Size = new Size(1100, 700);
        tabs.TabIndex = 0;
        // 
        // employeesPage
        // 
        employeesPage.Controls.Add(employeeManagementControl);
        employeesPage.Location = new Point(4, 29);
        employeesPage.Name = "employeesPage";
        employeesPage.Size = new Size(1092, 667);
        employeesPage.TabIndex = 0;
        employeesPage.Text = "Nhân viên / Tiếp nhận";
        // 
        // employeeManagementControl
        // 
        employeeManagementControl.BackColor = Color.FromArgb(244, 246, 249);
        employeeManagementControl.Dock = DockStyle.Fill;
        employeeManagementControl.Font = new Font("Segoe UI", 9F);
        employeeManagementControl.Location = new Point(0, 0);
        employeeManagementControl.Name = "employeeManagementControl";
        employeeManagementControl.Size = new Size(1092, 667);
        employeeManagementControl.TabIndex = 0;
        // 
        // offboardingPage
        // 
        offboardingPage.Controls.Add(offboardingControl);
        offboardingPage.Location = new Point(4, 29);
        offboardingPage.Name = "offboardingPage";
        offboardingPage.Size = new Size(1092, 667);
        offboardingPage.TabIndex = 1;
        offboardingPage.Text = "Nghỉ việc / Bàn giao";
        // 
        // offboardingControl
        // 
        offboardingControl.BackColor = Color.FromArgb(244, 246, 249);
        offboardingControl.Dock = DockStyle.Fill;
        offboardingControl.Font = new Font("Segoe UI", 9F);
        offboardingControl.Location = new Point(0, 0);
        offboardingControl.Name = "offboardingControl";
        offboardingControl.Size = new Size(1092, 667);
        offboardingControl.TabIndex = 0;
        // 
        // reviewPage
        // 
        reviewPage.Controls.Add(profileReviewControl);
        reviewPage.Location = new Point(4, 29);
        reviewPage.Name = "reviewPage";
        reviewPage.Size = new Size(1092, 667);
        reviewPage.TabIndex = 2;
        reviewPage.Text = "Duyệt cập nhật hồ sơ";
        // 
        // profileReviewControl
        // 
        profileReviewControl.BackColor = Color.FromArgb(244, 246, 249);
        profileReviewControl.Dock = DockStyle.Fill;
        profileReviewControl.Font = new Font("Segoe UI", 9F);
        profileReviewControl.Location = new Point(0, 0);
        profileReviewControl.Name = "profileReviewControl";
        profileReviewControl.Size = new Size(1092, 667);
        profileReviewControl.TabIndex = 0;
        // 
        // alertsPage
        // 
        alertsPage.Controls.Add(contractAlertsControl);
        alertsPage.Location = new Point(4, 29);
        alertsPage.Name = "alertsPage";
        alertsPage.Size = new Size(1092, 667);
        alertsPage.TabIndex = 3;
        alertsPage.Text = "Cảnh báo hợp đồng";
        // 
        // contractAlertsControl
        // 
        contractAlertsControl.BackColor = Color.FromArgb(244, 246, 249);
        contractAlertsControl.Dock = DockStyle.Fill;
        contractAlertsControl.Font = new Font("Segoe UI", 9F);
        contractAlertsControl.Location = new Point(0, 0);
        contractAlertsControl.Name = "contractAlertsControl";
        contractAlertsControl.Size = new Size(1092, 667);
        contractAlertsControl.TabIndex = 0;
        // 
        // HrWorkspaceControl
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(tabs);
        Name = "HrWorkspaceControl";
        Size = new Size(1100, 700);
        tabs.ResumeLayout(false);
        employeesPage.ResumeLayout(false);
        offboardingPage.ResumeLayout(false);
        reviewPage.ResumeLayout(false);
        alertsPage.ResumeLayout(false);
        ResumeLayout(false);
    }
    private TabControl tabs;
    private TabPage employeesPage;
    private TabPage offboardingPage;
    private TabPage reviewPage;
    private TabPage alertsPage;
    private EmployeeManagementControl employeeManagementControl;
    private OffboardingControl offboardingControl;
    private ProfileReviewControl profileReviewControl;
    private ContractAlertsControl contractAlertsControl;
}

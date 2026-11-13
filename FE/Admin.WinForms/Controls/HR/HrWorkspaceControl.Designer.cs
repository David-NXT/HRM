#nullable disable
namespace Admin.WinForms.Controls.HR;

partial class HrWorkspaceControl
{
    private void InitializeComponent()
    {
        tabs = new TabControl();
        employeesPage = new TabPage();
        offboardingPage = new TabPage();
        reviewPage = new TabPage();
        alertsPage = new TabPage();
        SuspendLayout();
        tabs.Dock = DockStyle.Fill;
        tabs.Name = "tabs";
        tabs.TabPages.AddRange(new TabPage[] { employeesPage, offboardingPage, reviewPage, alertsPage });
        employeesPage.Text = "Nhân viên / Tiếp nhận";
        employeesPage.Name = "employeesPage";
        offboardingPage.Text = "Nghỉ việc / Bàn giao";
        offboardingPage.Name = "offboardingPage";
        reviewPage.Text = "Duyệt cập nhật hồ sơ";
        reviewPage.Name = "reviewPage";
        alertsPage.Text = "Cảnh báo hợp đồng";
        alertsPage.Name = "alertsPage";
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(tabs);
        Name = "HrWorkspaceControl";
        Size = new Size(1100, 700);
        ResumeLayout(false);
    }
    private TabControl tabs;
    private TabPage employeesPage;
    private TabPage offboardingPage;
    private TabPage reviewPage;
    private TabPage alertsPage;
}

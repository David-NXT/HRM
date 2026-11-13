namespace Admin.WinForms.Controls.HR;

public partial class HrWorkspaceControl : UserControl
{
    public HrWorkspaceControl()
    {
        InitializeComponent();
        employeesPage.Controls.Add(new EmployeeManagementControl { Dock = DockStyle.Fill });
        offboardingPage.Controls.Add(new OffboardingControl { Dock = DockStyle.Fill });
        reviewPage.Controls.Add(new ProfileReviewControl { Dock = DockStyle.Fill });
        alertsPage.Controls.Add(new ContractAlertsControl { Dock = DockStyle.Fill });
    }
}

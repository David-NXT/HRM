using Admin.WinForms.Services.Project;

namespace Admin.WinForms.Controls.Project;

public sealed class ProjectOverviewControl : UserControl
{
    private static readonly string[] TabNames =
    [
        "Projects",
        "Tasks",
        "Timesheet Approval",
        "Labor Costs",
        "Statistics"
    ];

    private readonly ProjectApiClient _api = new();
    private readonly FlowLayoutPanel _navigation = new();
    private readonly Panel _content = new();
    private Button? _activeButton;
    private UserControl? _activePage;

    public ProjectOverviewControl()
    {
        BuildLayout();
        ShowPage(0);
    }

    private void BuildLayout()
    {
        BackColor = ProjectUi.Background;
        Font = new Font("Segoe UI", 10F);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(12),
            ColumnCount = 1,
            RowCount = 2,
            BackColor = ProjectUi.Background
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        _navigation.Dock = DockStyle.Fill;
        _navigation.FlowDirection = FlowDirection.LeftToRight;
        _navigation.WrapContents = false;
        _navigation.BackColor = ProjectUi.Background;
        _navigation.Padding = new Padding(0, 2, 0, 5);

        foreach (var (title, index) in TabNames.Select((title, index) => (title, index)))
        {
            var button = ProjectUi.CreateButton(title, primary: false);
            button.Width = 1;
            button.Height = 42;
            button.Margin = new Padding(index == 0 ? 0 : 8, 0, 0, 0);
            button.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            button.Click += (_, _) => ShowPage(index);
            _navigation.Controls.Add(button);
        }
        _navigation.SizeChanged += (_, _) =>
        {
            var gapTotal = Math.Max(0, _navigation.Controls.Count - 1) * 8;
            var buttonWidth = Math.Max(100, (_navigation.ClientSize.Width - gapTotal) / _navigation.Controls.Count);
            foreach (Control button in _navigation.Controls)
            {
                button.Width = buttonWidth;
            }
        };

        _content.Dock = DockStyle.Fill;
        _content.BackColor = ProjectUi.Background;

        layout.Controls.Add(_navigation, 0, 0);
        layout.Controls.Add(_content, 0, 1);
        Controls.Add(layout);
    }

    private void ShowPage(int index)
    {
        if (_activeButton is not null)
        {
            ProjectUi.SetTabButtonState(_activeButton, selected: false);
        }

        _activeButton = (Button)_navigation.Controls[index];
        ProjectUi.SetTabButtonState(_activeButton, selected: true);

        _activePage?.Dispose();
        _activePage = index switch
        {
            0 => new ProjectsPageControl(_api),
            1 => new TasksPageControl(_api),
            2 => new TimesheetApprovalPageControl(_api),
            3 => new LaborCostsPageControl(_api),
            _ => new ProjectStatisticsPageControl(_api)
        };

        _content.SuspendLayout();
        _content.Controls.Clear();
        _activePage.Dock = DockStyle.Fill;
        _content.Controls.Add(_activePage);
        _content.ResumeLayout();
    }
}

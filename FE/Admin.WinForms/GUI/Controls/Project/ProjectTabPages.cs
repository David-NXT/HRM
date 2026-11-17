using Admin.WinForms.BUS.Models.Project;
using Admin.WinForms.DAO.Project;
using System.Net;
using System.Text.Json;

namespace Admin.WinForms.GUI.Controls.Project;

internal abstract class ProjectPageControl : UserControl
{
    protected readonly ProjectApiClient Api;
    protected readonly Label StatusLabel = ProjectUi.CreateStatusLabel();
    private readonly CancellationTokenSource _lifetime = new();
    private readonly CancellationToken _lifetimeToken;

    protected ProjectPageControl(ProjectApiClient api)
    {
        Api = api;
        _lifetimeToken = _lifetime.Token;
        BackColor = ProjectUi.Background;
        ForeColor = ProjectUi.Text;
        Font = new Font("Segoe UI", 10F);
    }

    protected CancellationToken LifetimeToken => _lifetimeToken;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _lifetime.Cancel();
            _lifetime.Dispose();
        }

        base.Dispose(disposing);
    }

    protected async Task LoadSafelyAsync(Func<CancellationToken, Task> load)
    {
        try
        {
            await load(LifetimeToken);
        }
        catch (OperationCanceledException) when (LifetimeToken.IsCancellationRequested)
        {
        }
        catch (HttpRequestException ex)
        {
            if (!IsDisposed)
            {
                var message = ex.StatusCode is null
                    ? $"Project API is not reachable at {Api.BaseAddress}. Start it with: dotnet run --project src\\Project\\Project.API\\Project.csproj --launch-profile http"
                    : ex.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => "Project API access was denied.",
                    HttpStatusCode.Forbidden => "You are not allowed to view project data.",
                    HttpStatusCode.Conflict => ex.Message,
                    HttpStatusCode.ServiceUnavailable => "The HR employee directory is unavailable.",
                    _ => "Cannot load project data. Check that the Project API is running, then refresh."
                };
                ProjectUi.SetError(StatusLabel, message);
            }
        }
        catch (JsonException)
        {
            if (!IsDisposed)
            {
                ProjectUi.SetError(StatusLabel, "The Project API returned data in an invalid format.");
            }
        }
        catch (InvalidDataException)
        {
            if (!IsDisposed)
            {
                ProjectUi.SetError(StatusLabel, "The Project API returned empty or invalid data.");
            }
        }
        catch (TaskCanceledException) when (!LifetimeToken.IsCancellationRequested)
        {
            if (!IsDisposed)
            {
                ProjectUi.SetError(StatusLabel, "The request timed out. Check that the Project API is running, then refresh.");
            }
        }
    }

    protected async Task<List<EmployeeDirectoryDto>?> TryGetEmployeeDirectoryAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await Api.GetAllEmployeesAsync(cancellationToken);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.ServiceUnavailable)
        {
            return null;
        }
    }

    protected async Task<Guid?> ResolveEmployeeIdAsync(
        string employeeIdentifier,
        CancellationToken cancellationToken)
    {
        if (Guid.TryParse(employeeIdentifier, out var employeeId) && employeeId != Guid.Empty)
        {
            return employeeId;
        }

        var employees = await Api.GetEmployeesAsync(employeeIdentifier, cancellationToken);
        return employees.Items
            .FirstOrDefault(employee => string.Equals(
                employee.EmployeeCode,
                employeeIdentifier,
                StringComparison.OrdinalIgnoreCase))
            ?.EmployeeId;
    }

    protected static TableLayoutPanel CreateLayout(params float[] rowHeights)
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = rowHeights.Length,
            BackColor = ProjectUi.Background,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        for (var index = 0; index < rowHeights.Length; index++)
        {
            var sizeType = rowHeights[index] < 0 ? SizeType.Percent : SizeType.Absolute;
            layout.RowStyles.Add(new RowStyle(sizeType, Math.Abs(rowHeights[index])));
        }
        return layout;
    }

    protected static FlowLayoutPanel CreateToolbar()
    {
        return new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            WrapContents = false,
            FlowDirection = FlowDirection.LeftToRight,
            BackColor = ProjectUi.Background,
            Padding = new Padding(0, 4, 0, 2),
            AutoScroll = true
        };
    }

    protected static void ConfigureCombo(ComboBox combo, int width)
    {
        combo.Width = width;
        combo.Height = 36;
        combo.Margin = new Padding(0, 0, 8, 0);
    }

    protected static void ConfigureButton(Button button, int width)
    {
        button.Width = width;
        button.Height = 38;
        button.Margin = new Padding(0, 0, 8, 0);
    }

    protected static void ConfigureSearch(TextBox box, int width)
    {
        box.Width = width;
        box.Height = 36;
        box.Margin = new Padding(0, 0, 8, 0);
    }

    protected static DateTimePicker CreateDatePicker(DateTime value)
    {
        return new DateTimePicker
        {
            Value = value,
            Format = DateTimePickerFormat.Custom,
            CustomFormat = "dd/MM/yyyy",
            Width = 125,
            Height = 36,
            CalendarForeColor = ProjectUi.Text,
            CalendarMonthBackground = ProjectUi.Background,
            Font = new Font("Segoe UI", 10F),
            Margin = new Padding(0, 0, 8, 0)
        };
    }

    internal static DateTimePicker CreateDatePickerForDialog(DateTime value) => CreateDatePicker(value);

    protected static void AddProjectChoices(ComboBox combo, IEnumerable<ProjectDto> projects, bool includeAll = false)
    {
        combo.Items.Clear();
        if (includeAll)
        {
            combo.Items.Add(ProjectChoice.All);
        }
        foreach (var project in projects)
        {
            combo.Items.Add(new ProjectChoice(project.ProjectId, project.ProjectName));
        }
        if (combo.Items.Count > 0)
        {
            combo.SelectedIndex = 0;
        }
    }

    protected static Guid? SelectedProjectId(ComboBox combo)
    {
        return (combo.SelectedItem as ProjectChoice)?.ProjectId;
    }

    protected sealed record ProjectChoice(Guid? ProjectId, string Name)
    {
        public static ProjectChoice All { get; } = new(null, "All Projects");
        public override string ToString() => ProjectId.HasValue
            ? $"{ProjectUi.ShortId(ProjectId.Value)} - {Name}"
            : Name;
    }
}

internal sealed class ProjectsPageControl : ProjectPageControl
{
    private readonly TextBox _searchBox = ProjectUi.CreateSearchBox("Search projects by name or ID");
    private readonly DataGridView _projectsGrid = ProjectUi.CreateGrid();
    private readonly DataGridView _membersGrid = ProjectUi.CreateGrid();
    private List<ProjectDto> _projects = [];
    private List<ProjectMemberDto> _members = [];
    private Dictionary<Guid, string> _employeeNames = [];
    private Dictionary<Guid, string> _employeeCodes = [];
    private Dictionary<Guid, decimal> _hourlyRates = [];
    private bool _renderingProjects;
    private bool _employeeDirectoryUnavailable;

    internal ProjectsPageControl(ProjectApiClient api) : base(api)
    {
        BuildLayout();
        _searchBox.TextChanged += (_, _) => RenderProjects();
        _projectsGrid.SelectionChanged += async (_, _) =>
        {
            if (!_renderingProjects)
            {
                await LoadSelectedProjectMembersAsync();
            }
        };
        Load += async (_, _) => await RefreshAsync();
    }

    private void BuildLayout()
    {
        ConfigureProjectGrid();
        ProjectUi.AddColumn(_membersGrid, "EmployeeId", "Employee ID", 1F);
        ProjectUi.AddColumn(_membersGrid, "EmployeeCode", "Employee Code", 1F);
        ProjectUi.AddColumn(_membersGrid, "Name", "Full Name", 1.8F);
        ProjectUi.AddColumn(_membersGrid, "Role", "Project Role", 1.5F);
        ProjectUi.AddColumn(_membersGrid, "HourlyRate", "Hourly Rate", 1F);
        ProjectUi.AddColumn(_membersGrid, "JoinDate", "Join Date", 1F);
        ProjectUi.AddColumn(_membersGrid, "LeaveDate", "Leave Date", 1F);

        var layout = CreateLayout(46, 28, -38, 42, 34, -42, 42, 25);
        var toolbar = CreateToolbar();
        ConfigureSearch(_searchBox, 320);
        var refresh = ProjectUi.CreateButton("⟳ Refresh");
        ConfigureButton(refresh, 110);
        refresh.Click += async (_, _) => await RefreshAsync();
        toolbar.Controls.Add(_searchBox);
        toolbar.Controls.Add(refresh);

        var projectActions = CreateToolbar();
        projectActions.FlowDirection = FlowDirection.RightToLeft;
        AddActionButton(projectActions, "+ Add", 92, CreateProjectAsync);
        AddActionButton(projectActions, "✎ Edit", 92, UpdateProjectAsync);
        AddActionButton(projectActions, "▤ Delete", 92, DeleteProjectAsync);

        var memberActions = CreateToolbar();
        memberActions.FlowDirection = FlowDirection.RightToLeft;
        AddActionButton(memberActions, "♙ Add Member", 190, () => EditMemberAsync(null));
        AddActionButton(memberActions, "✎ Edit", 92, UpdateMemberAsync);
        AddActionButton(memberActions, "♙ Remove", 92, DeleteMemberAsync);

        layout.Controls.Add(toolbar, 0, 0);
        layout.Controls.Add(ProjectUi.CreateSectionTitle("Project List"), 0, 1);
        layout.Controls.Add(_projectsGrid, 0, 2);
        layout.Controls.Add(projectActions, 0, 3);
        layout.Controls.Add(ProjectUi.CreateSectionTitle("Project Members"), 0, 4);
        layout.Controls.Add(_membersGrid, 0, 5);
        layout.Controls.Add(memberActions, 0, 6);
        layout.Controls.Add(StatusLabel, 0, 7);
        Controls.Add(layout);
    }

    private void ConfigureProjectGrid()
    {
        ProjectUi.AddColumn(_projectsGrid, "Code", "Project ID", 0.8F);
        ProjectUi.AddColumn(_projectsGrid, "Name", "Project Name", 1.6F);
        ProjectUi.AddColumn(_projectsGrid, "Start", "Start Date", 0.9F);
        ProjectUi.AddColumn(_projectsGrid, "End", "End Date", 0.9F);
        ProjectUi.AddColumn(_projectsGrid, "Status", "Status", 0.9F);
        ProjectUi.AddColumn(_projectsGrid, "Budget", "Budget", 1F);
        _projectsGrid.SelectionChanged += (_, _) => RenderMembers();
    }

    private static void AddActionButton(FlowLayoutPanel panel, string title, int width, Func<Task> action)
    {
        var button = ProjectUi.CreateButton(title);
        ConfigureButton(button, width);
        button.Click += async (_, _) => await action();
        panel.Controls.Add(button);
    }

    private async Task CreateProjectAsync()
    {
        using var dialog = new ProjectEditorDialog(null);
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }
        await LoadSafelyAsync(async token =>
        {
            await Api.CreateProjectAsync(dialog.Project, token);
            await RefreshAsync();
        });
    }

    private async Task UpdateProjectAsync()
    {
        if (SelectedProject is not { } project)
        {
            MessageBox.Show("Select a project first.", "Project Management", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        using var dialog = new ProjectEditorDialog(project);
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }
        await LoadSafelyAsync(async token =>
        {
            await Api.UpdateProjectAsync(project.ProjectId, dialog.Project, token);
            await RefreshAsync();
        });
    }

    private async Task DeleteProjectAsync()
    {
        if (SelectedProject is not { } project)
        {
            MessageBox.Show("Select a project first.", "Project Management", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (MessageBox.Show($"Delete project \"{project.ProjectName}\"?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
        {
            return;
        }
        await LoadSafelyAsync(async token =>
        {
            await Api.DeleteProjectAsync(project.ProjectId, token);
            await RefreshAsync();
        });
    }

    private async Task EditMemberAsync(ProjectMemberDto? member)
    {
        if (SelectedProject is not { } project)
        {
            MessageBox.Show("Select a project first.", "Project Members", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (!project.EndDate.HasValue)
        {
            MessageBox.Show(
                this,
                "Set the project end date before adding or editing members.",
                "Project Members",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }
        await LoadSafelyAsync(async token =>
        {
            var roles = await Api.GetProjectRolesAsync(token);
            using var dialog = new ProjectMemberEditorDialog(
                project,
                roles,
                member,
                () => Api.GetAllEmployeesAsync(token));
            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }
            Guid? employeeId = member?.EmployeeId;
            if (!employeeId.HasValue)
            {
                employeeId = await ResolveEmployeeIdAsync(dialog.EmployeeIdentifier, token);
            }
            if (!employeeId.HasValue)
            {
                MessageBox.Show(
                    this,
                    $"No active HR employee matches \"{dialog.EmployeeIdentifier}\".",
                    "Project Members",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }
            dialog.Member.EmployeeId = employeeId.Value;
            if (member is null)
            {
                await Api.CreateProjectMemberAsync(dialog.Member, token);
            }
            else
            {
                await Api.UpdateProjectMemberAsync(member.ProjectMemberId, dialog.Member, token);
            }
            await LoadMembersAsync(project.ProjectId, token);
        });
    }

    private Task UpdateMemberAsync()
    {
        var memberId = _membersGrid.CurrentRow?.Tag as Guid?;
        var member = memberId.HasValue
            ? _members.FirstOrDefault(item => item.ProjectMemberId == memberId.Value)
            : null;
        if (member is null)
        {
            MessageBox.Show("Select a project member first.", "Project Members", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return Task.CompletedTask;
        }
        return EditMemberAsync(member);
    }

    private async Task DeleteMemberAsync()
    {
        var memberId = _membersGrid.CurrentRow?.Tag as Guid?;
        if (!memberId.HasValue)
        {
            MessageBox.Show("Select a project member first.", "Project Members", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (MessageBox.Show("Remove this member from the project?", "Confirm Removal", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
        {
            return;
        }
        await LoadSafelyAsync(async token =>
        {
            await Api.DeleteProjectMemberAsync(memberId.Value, token);
            if (SelectedProject is { } project)
            {
                await LoadMembersAsync(project.ProjectId, token);
            }
        });
    }

    private async Task RefreshAsync()
    {
        await LoadSafelyAsync(async token =>
        {
            _projects = await Api.GetProjectsAsync(token);
            RenderProjects();
            await LoadSelectedProjectMembersAsync(token);
            if (_employeeDirectoryUnavailable)
            {
                StatusLabel.Text = $"Showing {_projectsGrid.Rows.Count} of {_projects.Count} projects. HR employee codes and names are unavailable; employee IDs are shown.";
                StatusLabel.ForeColor = Color.FromArgb(180, 115, 0);
            }
            else
            {
                StatusLabel.ForeColor = ProjectUi.MutedText;
                StatusLabel.Text = $"Showing {_projectsGrid.Rows.Count} of {_projects.Count} projects.";
            }
        });
    }

    private async Task LoadSelectedProjectMembersAsync()
    {
        if (IsDisposed || SelectedProject is not { } project)
        {
            _members = [];
            RenderMembers();
            return;
        }
        await LoadSafelyAsync(token => LoadMembersAsync(project.ProjectId, token));
    }

    private async Task LoadSelectedProjectMembersAsync(CancellationToken token)
    {
        if (SelectedProject is { } project)
        {
            await LoadMembersAsync(project.ProjectId, token);
        }
        else
        {
            _members = [];
            RenderMembers();
        }
    }

    private async Task LoadMembersAsync(Guid projectId, CancellationToken token)
    {
        var membersTask = Api.GetProjectMembersAsync(projectId, token);
        var ratesTask = Api.GetHourlyRatesAsync(token);
        await Task.WhenAll(membersTask, ratesTask);
        var members = await membersTask;
        var today = DateTime.Today;
        _hourlyRates = (await ratesTask)
            .Where(rate =>
                rate.EffectiveFrom.Date <= today
                && (!rate.EffectiveTo.HasValue || rate.EffectiveTo.Value.Date >= today))
            .GroupBy(rate => rate.ProjectMemberId)
            .ToDictionary(
                group => group.Key,
                group => group.OrderByDescending(rate => rate.EffectiveFrom).First().Rate);
        var employeePage = await TryGetEmployeeDirectoryAsync(token);
        _members = members;
        _employeeDirectoryUnavailable = employeePage is null;
        _employeeNames = employeePage?.ToDictionary(employee => employee.EmployeeId, employee => employee.FullName) ?? [];
        _employeeCodes = employeePage?.ToDictionary(employee => employee.EmployeeId, employee => employee.EmployeeCode) ?? [];
        RenderMembers();
    }

    private ProjectDto? SelectedProject =>
        _projectsGrid.CurrentRow?.Tag as ProjectDto;

    private void RenderProjects()
    {
        _renderingProjects = true;
        try
        {
            _projectsGrid.Rows.Clear();
            var search = _searchBox.Text.Trim();
            foreach (var project in _projects.Where(project =>
                search.Length == 0
                || project.ProjectName.Contains(search, StringComparison.OrdinalIgnoreCase)
                || project.ProjectId.ToString().Contains(search, StringComparison.OrdinalIgnoreCase)))
            {
                var row = _projectsGrid.Rows.Add(
                    ProjectUi.ShortId(project.ProjectId),
                    project.ProjectName,
                    ProjectUi.Date(project.StartDate),
                    ProjectUi.Date(project.EndDate),
                    ProjectUi.Status(project.Status),
                    ProjectUi.Money(project.Budget));
                _projectsGrid.Rows[row].Tag = project;
            }
            if (_projectsGrid.Rows.Count > 0)
            {
                _projectsGrid.Rows[0].Selected = true;
            }
        }
        finally
        {
            _renderingProjects = false;
        }
    }

    private void RenderMembers()
    {
        _membersGrid.Rows.Clear();
        foreach (var member in _members)
        {
            var row = _membersGrid.Rows.Add(
                ProjectUi.ShortId(member.EmployeeId),
                _employeeCodes.GetValueOrDefault(member.EmployeeId, "—"),
                _employeeNames.GetValueOrDefault(member.EmployeeId, "—"),
                member.RoleName,
                _hourlyRates.TryGetValue(member.ProjectMemberId, out var hourlyRate)
                    ? ProjectUi.Money(hourlyRate)
                    : "—",
                ProjectUi.Date(member.JoinDate),
                ProjectUi.Date(member.LeaveDate));
            _membersGrid.Rows[row].Tag = member.ProjectMemberId;
        }
    }
}

internal sealed class TasksPageControl : ProjectPageControl
{
    private readonly ComboBox _projectCombo = ProjectUi.CreateComboBox();
    private readonly TextBox _searchBox = ProjectUi.CreateSearchBox("Search tasks");
    private readonly DataGridView _tasksGrid = ProjectUi.CreateGrid();
    private readonly DataGridView _timesheetsGrid = ProjectUi.CreateGrid();
    private readonly Label _totalHours = ProjectUi.CreateLabel("Total Hours: 0");
    private List<ProjectDto> _projects = [];
    private List<TaskDto> _tasks = [];
    private List<TimesheetDto> _timesheets = [];
    private List<HourlyRateDto> _hourlyRates = [];
    private Dictionary<Guid, EmployeeDirectoryDto> _employeeDirectory = [];
    private bool _employeeDirectoryUnavailable;
    private bool _updatingProjects;

    internal TasksPageControl(ProjectApiClient api) : base(api)
    {
        BuildLayout();
        _projectCombo.SelectedIndexChanged += async (_, _) =>
        {
            if (!_updatingProjects)
            {
                await LoadSelectedProjectAsync();
            }
        };
        _searchBox.TextChanged += (_, _) => RenderTasks();
        _tasksGrid.SelectionChanged += (_, _) => RenderTimesheets();
        Load += async (_, _) => await RefreshAsync();
    }

    private void BuildLayout()
    {
        ProjectUi.AddColumn(_tasksGrid, "Code", "Task ID", 0.75F);
        ProjectUi.AddColumn(_tasksGrid, "Name", "Task Name", 1.8F);
        ProjectUi.AddColumn(_tasksGrid, "Start", "Start Date", 1F);
        ProjectUi.AddColumn(_tasksGrid, "Due", "Due Date", 1F);
        ProjectUi.AddColumn(_tasksGrid, "Status", "Status", 1F);
        ProjectUi.AddColumn(_tasksGrid, "Priority", "Priority", 0.9F);
        ProjectUi.AddColumn(_timesheetsGrid, "EmployeeId", "Employee ID", 0.8F);
        ProjectUi.AddColumn(_timesheetsGrid, "EmployeeCode", "Employee Code", 1F);
        ProjectUi.AddColumn(_timesheetsGrid, "Name", "Employee", 1.3F);
        ProjectUi.AddColumn(_timesheetsGrid, "Date", "Work Date", 1F);
        ProjectUi.AddColumn(_timesheetsGrid, "HourlyRate", "Hourly Rate on Date", 1.2F);
        ProjectUi.AddColumn(_timesheetsGrid, "Hours", "Hours", 0.7F);
        ProjectUi.AddColumn(_timesheetsGrid, "Status", "Status", 1F);
        ProjectUi.AddColumn(_timesheetsGrid, "Approver", "Approved By", 1F);

        var layout = CreateLayout(46, 28, -34, 42, 30, -34, 42, 24);
        var toolbar = CreateToolbar();
        ConfigureCombo(_projectCombo, 330);
        ConfigureSearch(_searchBox, 190);
        var refresh = ProjectUi.CreateButton("⟳ Refresh");
        ConfigureButton(refresh, 110);
        refresh.Click += async (_, _) => await RefreshAsync();
        toolbar.Controls.Add(ProjectUi.CreateLabel("Project"));
        toolbar.Controls.Add(_projectCombo);
        toolbar.Controls.Add(_searchBox);
        toolbar.Controls.Add(refresh);

        var taskActions = CreateToolbar();
        taskActions.FlowDirection = FlowDirection.RightToLeft;
        AddActionButton(taskActions, "+ Add", 100, () => EditTaskAsync(null));
        AddActionButton(taskActions, "✎ Edit", 92, UpdateTaskAsync);
        AddActionButton(taskActions, "▤ Delete", 92, DeleteTaskAsync);

        var timesheetHeader = new Panel { Dock = DockStyle.Fill };
        timesheetHeader.Controls.Add(ProjectUi.CreateSectionTitle("Task Timesheets"));
        _totalHours.Location = new Point(0, 2);
        _totalHours.AutoSize = true;
        _totalHours.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        timesheetHeader.Controls.Add(_totalHours);
        timesheetHeader.Resize += (_, _) => _totalHours.Location = new Point(Math.Max(0, timesheetHeader.ClientSize.Width - _totalHours.Width), 3);

        var timesheetActions = CreateToolbar();
        timesheetActions.FlowDirection = FlowDirection.RightToLeft;
        AddActionButton(timesheetActions, "+ Add", 100, CreateTimesheetAsync);
        AddActionButton(timesheetActions, "✎ Edit", 92, UpdateTimesheetAsync);
        AddActionButton(timesheetActions, "▤ Delete", 92, DeleteTimesheetAsync);

        layout.Controls.Add(toolbar, 0, 0);
        layout.Controls.Add(ProjectUi.CreateSectionTitle("Task List"), 0, 1);
        layout.Controls.Add(_tasksGrid, 0, 2);
        layout.Controls.Add(taskActions, 0, 3);
        layout.Controls.Add(timesheetHeader, 0, 4);
        layout.Controls.Add(_timesheetsGrid, 0, 5);
        layout.Controls.Add(timesheetActions, 0, 6);
        layout.Controls.Add(StatusLabel, 0, 7);
        Controls.Add(layout);
    }

    private async Task RefreshAsync()
    {
        await LoadSafelyAsync(async token =>
        {
            var selectedId = SelectedProjectId(_projectCombo);
            _projects = await Api.GetProjectsAsync(token);
            _updatingProjects = true;
            AddProjectChoices(_projectCombo, _projects);
            if (selectedId.HasValue)
            {
                _projectCombo.SelectedItem = _projectCombo.Items
                    .Cast<ProjectChoice>()
                    .FirstOrDefault(choice => choice.ProjectId == selectedId.Value);
            }
            _updatingProjects = false;
            await LoadSelectedProjectAsync(token);
        });
    }

    private async Task LoadSelectedProjectAsync()
    {
        if (!IsDisposed)
        {
            await LoadSafelyAsync(LoadSelectedProjectAsync);
        }
    }

    private async Task LoadSelectedProjectAsync(CancellationToken token)
    {
        var projectId = SelectedProjectId(_projectCombo);
        if (!projectId.HasValue)
        {
            _tasks = [];
            _timesheets = [];
            _hourlyRates = [];
            _employeeDirectory = [];
            _employeeDirectoryUnavailable = false;
            RenderTasks();
            RenderTimesheets();
            return;
        }

        var tasksRequest = Api.GetTasksAsync(token);
        var timesheetsRequest = Api.GetTimesheetsAsync(projectId, null, token);
        var ratesRequest = Api.GetHourlyRatesAsync(token);
        await Task.WhenAll(tasksRequest, timesheetsRequest, ratesRequest);
        _tasks = (await tasksRequest).Where(task => task.ProjectId == projectId.Value).ToList();
        _timesheets = await timesheetsRequest;
        _hourlyRates = await ratesRequest;
        var employees = await TryGetEmployeeDirectoryAsync(token);
        _employeeDirectory = employees?.ToDictionary(employee => employee.EmployeeId) ?? [];
        _employeeDirectoryUnavailable = employees is null;
        RenderTasks();
        RenderTimesheets();
        StatusLabel.ForeColor = ProjectUi.MutedText;
        StatusLabel.Text = _employeeDirectoryUnavailable
            ? $"Showing {_tasks.Count} tasks. HR employee codes are unavailable; employee IDs are shown."
            : $"Showing {_tasks.Count} tasks.";
    }

    private void RenderTasks()
    {
        _tasksGrid.Rows.Clear();
        var search = _searchBox.Text.Trim();
        foreach (var task in _tasks.Where(task =>
            search.Length == 0
            || task.TaskName.Contains(search, StringComparison.OrdinalIgnoreCase)
            || task.TaskId.ToString().Contains(search, StringComparison.OrdinalIgnoreCase)))
        {
            var row = _tasksGrid.Rows.Add(
                ProjectUi.ShortId(task.TaskId),
                task.TaskName,
                ProjectUi.Date(task.StartDate),
                ProjectUi.Date(task.DueDate),
                ProjectUi.Status(task.Status),
                ProjectUi.Priority(task.Priority));
            _tasksGrid.Rows[row].Tag = task;
        }
        if (_tasksGrid.Rows.Count > 0)
        {
            _tasksGrid.Rows[0].Selected = true;
        }
    }

    private void RenderTimesheets()
    {
        _timesheetsGrid.Rows.Clear();
        var taskId = (_tasksGrid.CurrentRow?.Tag as TaskDto)?.TaskId;
        var rows = _timesheets.Where(sheet => !taskId.HasValue || sheet.TaskId == taskId.Value).ToArray();
        foreach (var sheet in rows)
        {
            var row = _timesheetsGrid.Rows.Add(
                ProjectUi.ShortId(sheet.EmployeeId),
                _employeeDirectory.GetValueOrDefault(sheet.EmployeeId)?.EmployeeCode ?? "—",
                _employeeDirectory.GetValueOrDefault(sheet.EmployeeId)?.FullName ?? "—",
                ProjectUi.Date(sheet.WorkDate),
                HourlyRateAt(sheet),
                sheet.HoursWorked.ToString("0.##"),
                ProjectUi.Status(sheet.Status),
                sheet.ApprovedBy.HasValue ? ProjectUi.ShortId(sheet.ApprovedBy.Value) : "—");
            _timesheetsGrid.Rows[row].Tag = sheet;
        }
        _totalHours.Text = $"Total Hours: {rows.Sum(sheet => sheet.HoursWorked):0.##}";
    }

    private string HourlyRateAt(TimesheetDto sheet)
    {
        var applicableRates = _hourlyRates
            .Where(rate =>
                rate.ProjectMemberId == sheet.ProjectMemberId &&
                rate.EffectiveFrom.Date <= sheet.WorkDate.Date &&
                (!rate.EffectiveTo.HasValue ||
                 rate.EffectiveTo.Value.Date >= sheet.WorkDate.Date))
            .ToArray();

        return applicableRates.Length switch
        {
            0 => "—",
            1 => ProjectUi.Money(applicableRates[0].Rate),
            _ => "Multiple"
        };
    }

    private async Task EditTaskAsync(TaskDto? task)
    {
        if (SelectedProjectId(_projectCombo) is not { } projectId)
        {
            MessageBox.Show("Select a project first.", "Tasks", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        using var dialog = new TaskEditorDialog(projectId, task);
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }
        await LoadSafelyAsync(async token =>
        {
            if (task is null)
            {
                await Api.CreateTaskAsync(dialog.Task, token);
            }
            else
            {
                await Api.UpdateTaskAsync(task.TaskId, dialog.Task, token);
            }
            await LoadSelectedProjectAsync(token);
        });
    }

    private Task UpdateTaskAsync()
    {
        var task = _tasksGrid.CurrentRow?.Tag as TaskDto;
        if (task is null)
        {
            MessageBox.Show("Select a task first.", "Tasks", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return Task.CompletedTask;
        }
        return EditTaskAsync(task);
    }

    private async Task DeleteTaskAsync()
    {
        if (_tasksGrid.CurrentRow?.Tag is not TaskDto task)
        {
            MessageBox.Show("Select a task first.", "Tasks", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (MessageBox.Show($"Delete task \"{task.TaskName}\"?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
        {
            return;
        }
        await LoadSafelyAsync(async token =>
        {
            await Api.DeleteTaskAsync(task.TaskId, token);
            await LoadSelectedProjectAsync(token);
        });
    }

    private async Task CreateTimesheetAsync()
    {
        var task = _tasksGrid.CurrentRow?.Tag as TaskDto;
        if (task is null)
        {
            MessageBox.Show("Select a task first.", "Timesheets", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        await LoadSafelyAsync(async token =>
        {
            var members = await Api.GetProjectMembersAsync(task.ProjectId, token);
            var employees = await TryGetEmployeeDirectoryAsync(token);
            var employeeDirectory = employees?.ToDictionary(employee => employee.EmployeeId);
            using var dialog = new TimesheetEditorDialog(task, members, null, employeeDirectory);
            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }
            await Api.CreateTimesheetAsync(dialog.Request, token);
            await LoadSelectedProjectAsync(token);
        });
    }

    private async Task UpdateTimesheetAsync()
    {
        if (_timesheetsGrid.CurrentRow?.Tag is not TimesheetDto sheet)
        {
            MessageBox.Show("Select a timesheet first.", "Timesheets", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        using var dialog = new TimesheetEditorDialog(null, [], sheet);
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }
        await LoadSafelyAsync(async token =>
        {
            await Api.UpdateTimesheetAsync(sheet.TimesheetId, dialog.UpdateRequest, token);
            await LoadSelectedProjectAsync(token);
        });
    }

    private async Task DeleteTimesheetAsync()
    {
        if (_timesheetsGrid.CurrentRow?.Tag is not TimesheetDto sheet)
        {
            MessageBox.Show("Select a timesheet first.", "Timesheets", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (MessageBox.Show("Delete the selected timesheet?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
        {
            return;
        }
        await LoadSafelyAsync(async token =>
        {
            await Api.DeleteTimesheetAsync(sheet.TimesheetId, token);
            await LoadSelectedProjectAsync(token);
        });
    }

    private static void AddActionButton(FlowLayoutPanel panel, string title, int width, Func<Task> action)
    {
        var button = ProjectUi.CreateButton(title);
        ConfigureButton(button, width);
        button.Click += async (_, _) => await action();
        panel.Controls.Add(button);
    }
}

internal sealed class TimesheetApprovalPageControl : ProjectPageControl
{
    private readonly ComboBox _projectCombo = ProjectUi.CreateComboBox();
    private readonly ComboBox _statusCombo = ProjectUi.CreateComboBox();
    private readonly TextBox _searchBox = ProjectUi.CreateSearchBox("Search by employee name or ID");
    private readonly DataGridView _grid = ProjectUi.CreateGrid();
    private readonly TextBox _reasonBox = ProjectUi.CreateSearchBox("Enter a rejection reason");
    private readonly Label _detailLabel = ProjectUi.CreateLabel("Select a timesheet to view its details.");
    private List<ProjectDto> _projects = [];
    private List<TimesheetDto> _timesheets = [];
    private Dictionary<Guid, EmployeeDirectoryDto> _employeeDirectory = [];
    private bool _employeeDirectoryUnavailable;
    private bool _updatingFilters;

    internal TimesheetApprovalPageControl(ProjectApiClient api) : base(api)
    {
        BuildLayout();
        _projectCombo.SelectedIndexChanged += async (_, _) =>
        {
            if (!_updatingFilters)
            {
                await RefreshTimesheetsAsync();
            }
        };
        _statusCombo.SelectedIndexChanged += async (_, _) =>
        {
            if (!_updatingFilters)
            {
                await RefreshTimesheetsAsync();
            }
        };
        _searchBox.TextChanged += (_, _) => RenderTimesheets();
        _grid.SelectionChanged += (_, _) => RenderDetail();
        Load += async (_, _) => await RefreshAsync();
    }

    private void BuildLayout()
    {
        ProjectUi.AddColumn(_grid, "EmployeeId", "Employee ID", 0.8F);
        ProjectUi.AddColumn(_grid, "EmployeeCode", "Employee Code", 1F);
        ProjectUi.AddColumn(_grid, "Name", "Employee", 1.3F);
        ProjectUi.AddColumn(_grid, "Date", "Work Date", 1F);
        ProjectUi.AddColumn(_grid, "Project", "Project", 1.7F);
        ProjectUi.AddColumn(_grid, "Task", "Task", 1.4F);
        ProjectUi.AddColumn(_grid, "Hours", "Hours", 0.6F);
        ProjectUi.AddColumn(_grid, "Status", "Status", 0.8F);

        var layout = CreateLayout(46, 28, -48, -40, 42, 24);
        var toolbar = CreateToolbar();
        ConfigureCombo(_projectCombo, 310);
        ConfigureCombo(_statusCombo, 150);
        ConfigureSearch(_searchBox, 190);
        _updatingFilters = true;
        _statusCombo.Items.AddRange(["Pending", "Approved", "Rejected", "All"]);
        _statusCombo.SelectedIndex = 0;
        _updatingFilters = false;
        var refresh = ProjectUi.CreateButton("⟳ Refresh");
        ConfigureButton(refresh, 110);
        refresh.Click += async (_, _) => await RefreshAsync();
        toolbar.Controls.Add(_projectCombo);
        toolbar.Controls.Add(_statusCombo);
        toolbar.Controls.Add(_searchBox);
        toolbar.Controls.Add(refresh);

        var details = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 3,
            Padding = new Padding(0, 8, 0, 8),
            BackColor = ProjectUi.Background
        };
        details.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
        details.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
        details.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
        details.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        details.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));
        details.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        details.Controls.Add(ProjectUi.CreateSectionTitle("Timesheet Details"), 0, 0);
        details.SetColumnSpan(details.GetControlFromPosition(0, 0)!, 3);
        _detailLabel.Dock = DockStyle.Fill;
        _detailLabel.AutoSize = false;
        _detailLabel.TextAlign = ContentAlignment.TopLeft;
        details.Controls.Add(_detailLabel, 0, 1);
        details.SetColumnSpan(_detailLabel, 3);
        details.Controls.Add(ProjectUi.CreateLabel("Rejection Reason"), 0, 2);
        _reasonBox.Dock = DockStyle.Fill;
        details.Controls.Add(_reasonBox, 1, 2);

        var actions = CreateToolbar();
        actions.FlowDirection = FlowDirection.RightToLeft;
        var approve = ProjectUi.CreateButton("✓ Approve", primary: true);
        ConfigureButton(approve, 120);
        approve.Click += async (_, _) => await ReviewSelectedAsync(approved: true);
        var reject = ProjectUi.CreateButton("× Reject");
        ConfigureButton(reject, 135);
        reject.Click += async (_, _) => await ReviewSelectedAsync(approved: false);
        actions.Controls.Add(approve);
        actions.Controls.Add(reject);

        layout.Controls.Add(toolbar, 0, 0);
        layout.Controls.Add(ProjectUi.CreateSectionTitle("Timesheets Pending Approval"), 0, 1);
        layout.Controls.Add(_grid, 0, 2);
        layout.Controls.Add(details, 0, 3);
        layout.Controls.Add(actions, 0, 4);
        layout.Controls.Add(StatusLabel, 0, 5);
        Controls.Add(layout);
    }

    private async Task RefreshAsync()
    {
        await LoadSafelyAsync(async token =>
        {
            _projects = await Api.GetProjectsAsync(token);
            _updatingFilters = true;
            AddProjectChoices(_projectCombo, _projects, includeAll: true);
            _updatingFilters = false;
            await RefreshTimesheetsAsync(token);
        });
    }

    private async Task RefreshTimesheetsAsync()
    {
        if (!IsDisposed)
        {
            await LoadSafelyAsync(RefreshTimesheetsAsync);
        }
    }

    private async Task RefreshTimesheetsAsync(CancellationToken token)
    {
        var status = _statusCombo.SelectedIndex switch
        {
            0 => "Pending",
            1 => "Approved",
            2 => "Rejected",
            _ => null
        };
        _timesheets = await Api.GetTimesheetsAsync(SelectedProjectId(_projectCombo), status, token);
        var employees = await TryGetEmployeeDirectoryAsync(token);
        _employeeDirectory = employees?.ToDictionary(employee => employee.EmployeeId) ?? [];
        _employeeDirectoryUnavailable = employees is null;
        RenderTimesheets();
        StatusLabel.ForeColor = ProjectUi.MutedText;
        StatusLabel.Text = _employeeDirectoryUnavailable
            ? $"Showing {_grid.Rows.Count} timesheets. HR employee codes are unavailable; employee IDs are shown."
            : $"Showing {_grid.Rows.Count} timesheets.";
    }

    private void RenderTimesheets()
    {
        _grid.Rows.Clear();
        var search = _searchBox.Text.Trim();
        foreach (var sheet in _timesheets.Where(sheet =>
            search.Length == 0
            || sheet.EmployeeId.ToString().Contains(search, StringComparison.OrdinalIgnoreCase)
            || ProjectUi.ShortId(sheet.EmployeeId).Contains(search, StringComparison.OrdinalIgnoreCase)
            || _employeeDirectory.GetValueOrDefault(sheet.EmployeeId)?.EmployeeCode.Contains(search, StringComparison.OrdinalIgnoreCase) == true
            || _employeeDirectory.GetValueOrDefault(sheet.EmployeeId)?.FullName.Contains(search, StringComparison.OrdinalIgnoreCase) == true))
        {
            var row = _grid.Rows.Add(
                ProjectUi.ShortId(sheet.EmployeeId),
                _employeeDirectory.GetValueOrDefault(sheet.EmployeeId)?.EmployeeCode ?? "—",
                _employeeDirectory.GetValueOrDefault(sheet.EmployeeId)?.FullName ?? "—",
                ProjectUi.Date(sheet.WorkDate),
                sheet.ProjectName,
                sheet.TaskName,
                sheet.HoursWorked.ToString("0.##"),
                ProjectUi.Status(sheet.Status));
            _grid.Rows[row].Tag = sheet;
        }
        if (_grid.Rows.Count > 0)
        {
            _grid.Rows[0].Selected = true;
        }
        RenderDetail();
    }

    private void RenderDetail()
    {
        if (_grid.CurrentRow?.Tag is not TimesheetDto sheet)
        {
            _detailLabel.Text = "Select a timesheet to view its details.";
            return;
        }
        var employee = _employeeDirectory.GetValueOrDefault(sheet.EmployeeId);
        _detailLabel.Text =
            $"Employee: {employee?.EmployeeCode ?? ProjectUi.ShortId(sheet.EmployeeId)} - {employee?.FullName ?? "—"}    Work Date: {ProjectUi.Date(sheet.WorkDate)}    Status: {ProjectUi.Status(sheet.Status)}{Environment.NewLine}" +
            $"Project: {sheet.ProjectName}    Task: {sheet.TaskName}    Hours Submitted: {sheet.HoursWorked:0.##}{Environment.NewLine}" +
            $"Notes: {sheet.Description ?? "—"}";
        if (!string.IsNullOrEmpty(sheet.RejectReason))
        {
            _detailLabel.Text += $"{Environment.NewLine}Rejection Reason: {sheet.RejectReason}";
        }
    }

    private async Task ReviewSelectedAsync(bool approved)
    {
        if (_grid.CurrentRow?.Tag is not TimesheetDto sheet)
        {
            MessageBox.Show("Select a timesheet first.", "Timesheet Approval", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (!approved && string.IsNullOrWhiteSpace(_reasonBox.Text))
        {
            MessageBox.Show("Enter a rejection reason.", "Timesheet Approval", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (!approved && _reasonBox.Text.Trim().Length > 500)
        {
            MessageBox.Show("The rejection reason cannot exceed 500 characters.", "Timesheet Approval", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        using var dialog = new ApproverIdDialog(
            () => Api.GetAllEmployeesAsync(LifetimeToken));
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }
        await LoadSafelyAsync(async token =>
        {
            var approverId = await ResolveEmployeeIdAsync(dialog.EmployeeIdentifier, token);
            if (!approverId.HasValue)
            {
                MessageBox.Show(
                    this,
                    $"No active HR employee matches \"{dialog.EmployeeIdentifier}\".",
                    "Timesheet Approval",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            await Api.ReviewTimesheetAsync(
                sheet.TimesheetId,
                approved,
                approverId.Value,
                approved ? null : _reasonBox.Text,
                token);
            await RefreshTimesheetsAsync(token);
        });
    }
}

internal sealed class LaborCostsPageControl : ProjectPageControl
{
    private readonly ComboBox _projectCombo = ProjectUi.CreateComboBox();
    private readonly DateTimePicker _fromDate = CreateDatePicker(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1));
    private readonly DateTimePicker _toDate = CreateDatePicker(DateTime.Today);
    private readonly DataGridView _grid = ProjectUi.CreateGrid();
    private readonly Label _hoursCard = new();
    private readonly Label _costCard = new();
    private readonly Label _budgetCard = new();
    private readonly Label _remainingCard = new();
    private List<ProjectDto> _projects = [];
    private List<ApprovedPayrollDataDto> _payroll = [];
    private Dictionary<Guid, string> _employeeNames = [];
    private Dictionary<Guid, string> _employeeCodes = [];
    private Dictionary<Guid, string> _employeeRoles = [];

    internal LaborCostsPageControl(ProjectApiClient api) : base(api)
    {
        BuildLayout();
        Load += async (_, _) => await RefreshAsync();
    }

    private void BuildLayout()
    {
        ProjectUi.AddColumn(_grid, "EmployeeId", "Employee ID", 0.8F);
        ProjectUi.AddColumn(_grid, "EmployeeCode", "Employee Code", 1F);
        ProjectUi.AddColumn(_grid, "Name", "Full Name", 1.8F);
        ProjectUi.AddColumn(_grid, "Role", "Project Role", 1.4F);
        ProjectUi.AddColumn(_grid, "Hours", "Hours", 0.8F);
        ProjectUi.AddColumn(_grid, "Rate", "Hourly Rate", 1F);
        ProjectUi.AddColumn(_grid, "Cost", "Labor Cost", 1.2F);

        var layout = CreateLayout(46, 92, 40, 30, -1, 42, 24);
        var toolbar = CreateToolbar();
        ConfigureCombo(_projectCombo, 300);
        var load = ProjectUi.CreateButton("▤ View Report");
        ConfigureButton(load, 150);
        load.Click += async (_, _) => await RefreshPayrollAsync();
        toolbar.Controls.Add(_projectCombo);
        toolbar.Controls.Add(ProjectUi.CreateLabel("From"));
        toolbar.Controls.Add(_fromDate);
        toolbar.Controls.Add(ProjectUi.CreateLabel("To"));
        toolbar.Controls.Add(_toDate);
        toolbar.Controls.Add(load);

        var cards = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, RowCount = 1, BackColor = ProjectUi.Background };
        for (var i = 0; i < 4; i++)
        {
            cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        }
        AddMetricCard(cards, _hoursCard, "Total Hours", 0);
        AddMetricCard(cards, _costCard, "Labor Cost", 1);
        AddMetricCard(cards, _budgetCard, "Project Budget", 2);
        AddMetricCard(cards, _remainingCard, "Remaining Budget", 3);

        var usedBudget = new Label
        {
            Dock = DockStyle.Fill,
            Text = "Budget Utilization",
            ForeColor = ProjectUi.MutedText,
            TextAlign = ContentAlignment.MiddleLeft
        };
        layout.Controls.Add(toolbar, 0, 0);
        layout.Controls.Add(cards, 0, 1);
        layout.Controls.Add(usedBudget, 0, 2);
        layout.Controls.Add(ProjectUi.CreateSectionTitle("Member Cost Details (Approved Timesheets Only)"), 0, 3);
        layout.Controls.Add(_grid, 0, 4);

        var actions = CreateToolbar();
        actions.FlowDirection = FlowDirection.RightToLeft;
        var export = ProjectUi.CreateButton("↓ Export CSV");
        ConfigureButton(export, 145);
        export.Click += (_, _) => ExportCsv();
        var print = ProjectUi.CreateButton("▤ Print");
        ConfigureButton(print, 90);
        print.Click += (_, _) => PrintReport();
        actions.Controls.Add(print);
        actions.Controls.Add(export);
        layout.Controls.Add(actions, 0, 5);
        layout.Controls.Add(StatusLabel, 0, 6);
        Controls.Add(layout);
        UpdateMetrics();
    }

    private static void AddMetricCard(TableLayoutPanel parent, Label label, string title, int column)
    {
        var card = new Panel { Dock = DockStyle.Fill, BackColor = ProjectUi.Surface, Margin = new Padding(0, 0, 10, 0), Padding = new Padding(14, 8, 8, 6) };
        var caption = ProjectUi.CreateLabel(title, muted: true);
        caption.Location = new Point(14, 7);
        label.Location = new Point(14, 32);
        label.AutoSize = true;
        label.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        label.ForeColor = ProjectUi.Text;
        card.Controls.Add(caption);
        card.Controls.Add(label);
        parent.Controls.Add(card, column, 0);
    }

    private async Task RefreshAsync()
    {
        await LoadSafelyAsync(async token =>
        {
            _projects = await Api.GetProjectsAsync(token);
            AddProjectChoices(_projectCombo, _projects);
            await RefreshPayrollAsync(token);
        });
    }

    private async Task RefreshPayrollAsync()
    {
        if (!IsDisposed)
        {
            await LoadSafelyAsync(RefreshPayrollAsync);
        }
    }

    private async Task RefreshPayrollAsync(CancellationToken token)
    {
        if (_fromDate.Value.Date > _toDate.Value.Date)
        {
            ProjectUi.SetError(StatusLabel, "The start date must be on or before the end date.");
            return;
        }
        var selectedProjectId = SelectedProjectId(_projectCombo);
        var payrollRequest = Api.GetApprovedPayrollDataAsync(
            DateOnly.FromDateTime(_fromDate.Value.Date),
            DateOnly.FromDateTime(_toDate.Value.Date),
            token,
            selectedProjectId);
        var membersRequest = Api.GetProjectMembersAsync(selectedProjectId, token);
        await Task.WhenAll(payrollRequest, membersRequest);
        var response = await payrollRequest;
        var members = await membersRequest;
        var employees = await TryGetEmployeeDirectoryAsync(token);
        _payroll = response
            .Where(item => !selectedProjectId.HasValue || item.ProjectId == selectedProjectId.Value)
            .ToList();
        _employeeNames = employees?.ToDictionary(employee => employee.EmployeeId, employee => employee.FullName) ?? [];
        _employeeCodes = employees?.ToDictionary(employee => employee.EmployeeId, employee => employee.EmployeeCode) ?? [];
        _employeeRoles = members
            .GroupBy(member => member.EmployeeId)
            .ToDictionary(group => group.Key, group => group.First().RoleName);
        RenderPayroll();
        UpdateMetrics();
        if (employees is null)
        {
            StatusLabel.ForeColor = Color.FromArgb(180, 115, 0);
            StatusLabel.Text = $"{_payroll.Count} approved timesheet entries. HR employee codes and names are unavailable; employee IDs are shown.";
        }
        else
        {
            StatusLabel.ForeColor = ProjectUi.MutedText;
            StatusLabel.Text = $"{_payroll.Count} approved timesheet entries in the selected period.";
        }
    }

    private void RenderPayroll()
    {
        _grid.Rows.Clear();
        foreach (var memberGroup in _payroll.GroupBy(item => item.EmployeeId).OrderBy(group => ProjectUi.ShortId(group.Key)))
        {
            var hours = memberGroup.Sum(item => item.HoursWorked);
            var cost = memberGroup.Sum(item => item.TotalProjectIncome);
            var rate = hours == 0 ? 0 : memberGroup.Sum(item => item.HoursWorked * item.HourlyRate) / hours;
            _grid.Rows.Add(
                ProjectUi.ShortId(memberGroup.Key),
                _employeeCodes.GetValueOrDefault(memberGroup.Key, "—"),
                _employeeNames.GetValueOrDefault(memberGroup.Key, "—"),
                _employeeRoles.GetValueOrDefault(memberGroup.Key, "—"),
                hours.ToString("0.##"),
                ProjectUi.Money(rate),
                ProjectUi.Money(cost));
        }
        _grid.Rows.Add(
            "Total",
            "",
            "",
            "",
            _payroll.Sum(item => item.HoursWorked).ToString("0.##"),
            "",
            ProjectUi.Money(_payroll.Sum(item => item.TotalProjectIncome)));
    }

    private void UpdateMetrics()
    {
        var projectId = SelectedProjectId(_projectCombo);
        var budget = _projects.FirstOrDefault(project => project.ProjectId == projectId)?.Budget ?? 0;
        var cost = _payroll.Sum(item => item.TotalProjectIncome);
        _hoursCard.Text = _payroll.Sum(item => item.HoursWorked).ToString("0.##");
        _costCard.Text = ProjectUi.Money(cost);
        _budgetCard.Text = ProjectUi.Money(budget);
        _remainingCard.Text = ProjectUi.Money(budget - cost);
        _remainingCard.ForeColor = budget >= cost ? Color.FromArgb(0, 200, 30) : Color.FromArgb(255, 100, 100);
    }

    private void ExportCsv()
    {
        using var dialog = new SaveFileDialog
        {
            Filter = "CSV file (*.csv)|*.csv",
            FileName = "labor-cost-report.csv",
            Title = "Export Labor Cost Report"
        };
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }
        var lines = new List<string> { "EmployeeId,EmployeeCode,FullName,Hours,HourlyRate,TotalCost" };
        lines.AddRange(_payroll
            .GroupBy(item => item.EmployeeId)
            .Select(group =>
                $"{EscapeCsv(ProjectUi.ShortId(group.Key))},{EscapeCsv(_employeeCodes.GetValueOrDefault(group.Key, "—"))},{EscapeCsv(_employeeNames.GetValueOrDefault(group.Key, "—"))},{group.Sum(item => item.HoursWorked):0.##},{group.Sum(item => item.HoursWorked * item.HourlyRate) / Math.Max(1, group.Sum(item => item.HoursWorked)):0.##},{group.Sum(item => item.TotalProjectIncome):0.##}"));
        try
        {
            File.WriteAllLines(dialog.FileName, lines, System.Text.Encoding.UTF8);
        }
        catch (IOException ex)
        {
            MessageBox.Show($"Could not write the report: {ex.Message}", "Export Report", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (UnauthorizedAccessException ex)
        {
            MessageBox.Show($"You do not have permission to write this file: {ex.Message}", "Export Report", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void PrintReport()
    {
        using var document = new System.Drawing.Printing.PrintDocument();
        var groups = _payroll.GroupBy(item => item.EmployeeId).ToArray();
        var rowIndex = 0;
        document.PrintPage += (_, e) =>
        {
            var graphics = e.Graphics ?? throw new InvalidOperationException("Printer graphics are unavailable.");
            using var headingFont = new Font("Segoe UI", 14F, FontStyle.Bold);
            using var bodyFont = new Font("Segoe UI", 9F);
            using var brush = new SolidBrush(Color.Black);
            var y = 40F;
            graphics.DrawString("Labor Cost Report (Approved Timesheets)", headingFont, brush, 40, y);
            y += 38;
            graphics.DrawString($"From {ProjectUi.Date(_fromDate.Value)} to {ProjectUi.Date(_toDate.Value)}", bodyFont, brush, 40, y);
            y += 28;
            graphics.DrawString("Employee ID    Employee Code       Full Name                         Role                    Hours       Cost", bodyFont, brush, 40, y);
            y += 22;
            while (rowIndex < groups.Length && y < e.MarginBounds.Bottom - 48)
            {
                var group = groups[rowIndex++];
                var employeeCode = _employeeCodes.GetValueOrDefault(group.Key, "—");
                var name = _employeeNames.GetValueOrDefault(group.Key, "—");
                var role = _employeeRoles.GetValueOrDefault(group.Key, "—");
                var hours = group.Sum(item => item.HoursWorked);
                var cost = group.Sum(item => item.TotalProjectIncome);
                graphics.DrawString(
                    $"{ProjectUi.ShortId(group.Key),-12} {employeeCode,-18} {name,-28} {role,-24} {hours,6:0.##} {ProjectUi.Money(cost),16}",
                    bodyFont,
                    brush,
                    40,
                    y);
                y += 22;
            }
            e.HasMorePages = rowIndex < groups.Length;
            if (!e.HasMorePages)
            {
                y += 8;
                graphics.DrawString(
                    $"Total Hours: {_payroll.Sum(item => item.HoursWorked):0.##}     Total Cost: {ProjectUi.Money(_payroll.Sum(item => item.TotalProjectIncome))}",
                    bodyFont,
                    brush,
                    40,
                    y);
            }
        };
        using var dialog = new PrintDialog { Document = document };
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            try
            {
                document.Print();
            }
            catch (System.Drawing.Printing.InvalidPrinterException ex)
            {
                MessageBox.Show($"Could not print the report: {ex.Message}", "Print Report", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private static string EscapeCsv(string value) => $"\"{value.Replace("\"", "\"\"")}\"";
}

internal sealed class ProjectStatisticsPageControl : ProjectPageControl
{
    private readonly ComboBox _projectCombo = ProjectUi.CreateComboBox();
    private readonly DateTimePicker _fromDate = CreateDatePicker(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1));
    private readonly DateTimePicker _toDate = CreateDatePicker(DateTime.Today);
    private readonly Label _runningProjects = new();
    private readonly Label _overdueTasks = new();
    private readonly Label _pendingTimesheets = new();
    private readonly Panel _costPanel = new();
    private readonly Panel _taskPanel = new();
    private readonly Panel _memberPanel = new();
    private List<ProjectDto> _projects = [];
    private List<TaskDto> _tasks = [];
    private List<TimesheetDto> _pending = [];
    private List<ApprovedPayrollDataDto> _payroll = [];
    private Dictionary<Guid, string> _employeeCodes = [];
    private bool _employeeDirectoryUnavailable;

    internal ProjectStatisticsPageControl(ProjectApiClient api) : base(api)
    {
        BuildLayout();
        Load += async (_, _) => await RefreshAsync();
    }

    private void BuildLayout()
    {
        var layout = CreateLayout(46, 82, -55, -42, 24);
        var toolbar = CreateToolbar();
        ConfigureCombo(_projectCombo, 300);
        var view = ProjectUi.CreateButton("▤ View");
        ConfigureButton(view, 100);
        view.Click += async (_, _) => await RefreshStatsAsync();
        toolbar.Controls.Add(_projectCombo);
        toolbar.Controls.Add(ProjectUi.CreateLabel("From"));
        toolbar.Controls.Add(_fromDate);
        toolbar.Controls.Add(ProjectUi.CreateLabel("To"));
        toolbar.Controls.Add(_toDate);
        toolbar.Controls.Add(view);

        var metrics = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1, BackColor = ProjectUi.Background };
        for (var i = 0; i < 3; i++)
        {
            metrics.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
        }
        AddMetric(metrics, _runningProjects, "Active Projects", 0, ProjectUi.Text);
        AddMetric(metrics, _overdueTasks, "Overdue Tasks", 1, Color.FromArgb(190, 45, 45));
        AddMetric(metrics, _pendingTimesheets, "Pending Timesheets", 2, Color.FromArgb(180, 115, 0));

        var upperCharts = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, BackColor = ProjectUi.Background };
        upperCharts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        upperCharts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        AddChartPanel(upperCharts, _costPanel, "Monthly Labor Costs", 0);
        AddChartPanel(upperCharts, _taskPanel, "Task Progress", 1);
        AddChartPanel(layout, _memberPanel, "Approved Hours by Project Member", 0, 3);

        layout.Controls.Add(toolbar, 0, 0);
        layout.Controls.Add(metrics, 0, 1);
        layout.Controls.Add(upperCharts, 0, 2);
        layout.Controls.Add(StatusLabel, 0, 4);
        Controls.Add(layout);
    }

    private static void AddMetric(TableLayoutPanel parent, Label value, string title, int column, Color valueColor)
    {
        var card = new Panel { Dock = DockStyle.Fill, BackColor = ProjectUi.Surface, Padding = new Padding(14, 8, 8, 6), Margin = new Padding(0, 0, 10, 0) };
        var caption = ProjectUi.CreateLabel(title, muted: true);
        caption.Location = new Point(14, 8);
        value.Location = new Point(14, 33);
        value.AutoSize = true;
        value.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        value.ForeColor = valueColor;
        card.Controls.Add(caption);
        card.Controls.Add(value);
        parent.Controls.Add(card, column, 0);
    }

    private static void AddChartPanel(Control parent, Panel target, string title, int column, int row = 0)
    {
        var container = new Panel { Dock = DockStyle.Fill, BackColor = ProjectUi.Background, Padding = new Padding(6), Margin = new Padding(0, 8, 10, 8) };
        container.Paint += (_, e) => ControlPaint.DrawBorder(e.Graphics, container.ClientRectangle, ProjectUi.Border, ButtonBorderStyle.Solid);
        var heading = ProjectUi.CreateSectionTitle(title);
        heading.Location = new Point(12, 4);
        heading.Width = 430;
        heading.Height = 28;
        target.Dock = DockStyle.Fill;
        target.BackColor = ProjectUi.Background;
        target.Padding = new Padding(12, 34, 12, 8);
        container.Controls.Add(target);
        container.Controls.Add(heading);
        if (parent is TableLayoutPanel layout)
        {
            layout.Controls.Add(container, column, row);
        }
        else
        {
            parent.Controls.Add(container);
        }
    }

    private async Task RefreshAsync()
    {
        await LoadSafelyAsync(async token =>
        {
            _projects = await Api.GetProjectsAsync(token);
            AddProjectChoices(_projectCombo, _projects);
            await RefreshStatsAsync(token);
        });
    }

    private async Task RefreshStatsAsync()
    {
        if (!IsDisposed)
        {
            await LoadSafelyAsync(RefreshStatsAsync);
        }
    }

    private async Task RefreshStatsAsync(CancellationToken token)
    {
        if (_fromDate.Value.Date > _toDate.Value.Date)
        {
            ProjectUi.SetError(StatusLabel, "The start date must be on or before the end date.");
            return;
        }
        var projectId = SelectedProjectId(_projectCombo);
        var tasksRequest = Api.GetTasksAsync(token);
        var pendingRequest = Api.GetTimesheetsAsync(projectId, "Pending", token);
        var payrollRequest = Api.GetApprovedPayrollDataAsync(
            DateOnly.FromDateTime(_fromDate.Value.Date),
            DateOnly.FromDateTime(_toDate.Value.Date),
            token,
            projectId);
        await Task.WhenAll(tasksRequest, pendingRequest, payrollRequest);
        _tasks = (await tasksRequest)
            .Where(task => !projectId.HasValue || task.ProjectId == projectId.Value)
            .ToList();
        _pending = await pendingRequest;
        _payroll = (await payrollRequest)
            .Where(item => !projectId.HasValue || item.ProjectId == projectId.Value)
            .ToList();
        var employees = await TryGetEmployeeDirectoryAsync(token);
        _employeeCodes = employees?.ToDictionary(employee => employee.EmployeeId, employee => employee.EmployeeCode) ?? [];
        _employeeDirectoryUnavailable = employees is null;
        RenderStats();
        StatusLabel.ForeColor = ProjectUi.MutedText;
        StatusLabel.Text = _employeeDirectoryUnavailable
            ? "Statistics are based on project, task, and approved timesheet data. HR employee codes are unavailable; employee IDs are shown."
            : "Statistics are based on project, task, and approved timesheet data.";
    }

    private void RenderStats()
    {
        _runningProjects.Text = _projects.Count(project =>
            IsStatus(ProjectUi.Status(project.Status), "Active", "In Progress", "Running")).ToString();
        _overdueTasks.Text = _tasks.Count(task =>
            task.DueDate.HasValue
            && task.DueDate.Value.Date < DateTime.Today
            && !IsStatus(ProjectUi.Status(task.Status), "Completed", "Done")).ToString();
        _pendingTimesheets.Text = _pending.Count.ToString();

        RenderCostBars();
        RenderTaskProgress();
        RenderMemberHours();
    }

    private void RenderCostBars()
    {
        _costPanel.Controls.Clear();
        var grouped = _payroll
            .GroupBy(item => new DateTime(item.WorkDate.Year, item.WorkDate.Month, 1))
            .OrderBy(group => group.Key)
            .TakeLast(6)
            .ToArray();
        var max = grouped.Select(group => group.Sum(item => item.TotalProjectIncome)).DefaultIfEmpty(1).Max();
        var baseline = Math.Max(90, _costPanel.ClientSize.Height - 45);
        var barWidth = grouped.Length == 0 ? 40 : Math.Max(32, (_costPanel.ClientSize.Width - 45) / (grouped.Length * 2));
        for (var i = 0; i < grouped.Length; i++)
        {
            var total = grouped[i].Sum(item => item.TotalProjectIncome);
            var height = Math.Max(4, (int)((baseline - 35) * total / max));
            var bar = new Panel
            {
                BackColor = Color.FromArgb(48, 126, 212),
                Width = barWidth,
                Height = height,
                Left = 18 + i * barWidth * 2,
                Top = baseline - height
            };
            var month = new Label
            {
                Text = grouped[i].Key.ToString("MM"),
                Width = barWidth,
                Height = 24,
                Left = bar.Left,
                Top = baseline,
                ForeColor = ProjectUi.MutedText,
                TextAlign = ContentAlignment.MiddleCenter
            };
            _costPanel.Controls.Add(bar);
            _costPanel.Controls.Add(month);
        }
    }

    private void RenderTaskProgress()
    {
        _taskPanel.Controls.Clear();
        var completed = _tasks.Count(task => IsStatus(ProjectUi.Status(task.Status), "Completed", "Done"));
        var overdue = _tasks.Count(task =>
            task.DueDate.HasValue
            && task.DueDate.Value.Date < DateTime.Today
            && !IsStatus(ProjectUi.Status(task.Status), "Completed", "Done"));
        var inProgress = _tasks.Count(task =>
            IsStatus(ProjectUi.Status(task.Status), "In Progress", "Doing")
            && !(task.DueDate.HasValue && task.DueDate.Value.Date < DateTime.Today));
        var notStarted = Math.Max(0, _tasks.Count - completed - inProgress - overdue);
        var segments = new[]
        {
            (Name: "Completed", Count: completed, Color: Color.FromArgb(0, 165, 25)),
            (Name: "In Progress", Count: inProgress, Color: Color.FromArgb(47, 126, 212)),
            (Name: "Not Started", Count: notStarted, Color: Color.FromArgb(255, 180, 0)),
            (Name: "Overdue", Count: overdue, Color: Color.FromArgb(205, 59, 59))
        };
        var total = Math.Max(1, _tasks.Count);
        var chart = new Panel
        {
            Dock = DockStyle.Left,
            Width = 160,
            BackColor = ProjectUi.Background
        };
        chart.Paint += (_, e) =>
        {
            var diameter = Math.Min(116, Math.Max(40, chart.ClientSize.Height - 18));
            var bounds = new Rectangle(
                (chart.ClientSize.Width - diameter) / 2,
                Math.Max(2, (chart.ClientSize.Height - diameter) / 2),
                diameter,
                diameter);
            var start = -90F;
            foreach (var segment in segments.Where(item => item.Count > 0))
            {
                var sweep = 360F * segment.Count / total;
                using var pen = new Pen(segment.Color, 18F);
                pen.StartCap = System.Drawing.Drawing2D.LineCap.Flat;
                pen.EndCap = System.Drawing.Drawing2D.LineCap.Flat;
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.DrawArc(pen, bounds, start, sweep);
                start += sweep;
            }
            var percent = _tasks.Count == 0 ? 0 : completed * 100 / _tasks.Count;
            using var font = new Font("Segoe UI", 13F, FontStyle.Bold);
            using var brush = new SolidBrush(ProjectUi.Text);
            var text = $"{percent}%";
            var measured = e.Graphics.MeasureString(text, font);
            e.Graphics.DrawString(text, font, brush,
                bounds.Left + (bounds.Width - measured.Width) / 2,
                bounds.Top + (bounds.Height - measured.Height) / 2 - 3);
        };
        _taskPanel.Controls.Add(chart);
        var legend = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Padding = new Padding(6, 8, 0, 0),
            BackColor = ProjectUi.Background
        };
        foreach (var segment in segments)
        {
            var item = new Label
            {
                Text = $"■  {segment.Name}: {segment.Count}",
                AutoSize = true,
                ForeColor = segment.Color,
                Margin = new Padding(0, 2, 0, 4)
            };
            legend.Controls.Add(item);
        }
        _taskPanel.Controls.Add(legend);
        if (_tasks.Count == 0)
        {
            legend.Controls.Add(ProjectUi.CreateLabel("No tasks available.", muted: true));
        }
    }

    private static bool IsStatus(string status, params string[] expected) =>
        expected.Any(value => string.Equals(status, value, StringComparison.OrdinalIgnoreCase));

    private void RenderMemberHours()
    {
        _memberPanel.Controls.Clear();
        var groups = _payroll.GroupBy(item => item.EmployeeId)
            .Select(group => (EmployeeId: group.Key, Hours: group.Sum(item => item.HoursWorked)))
            .OrderByDescending(item => item.Hours)
            .Take(6)
            .ToArray();
        var max = groups.Select(group => group.Hours).DefaultIfEmpty(1).Max();
        var y = 34;
        foreach (var item in groups)
        {
            var name = new Label
            {
                Text = _employeeCodes.GetValueOrDefault(item.EmployeeId, ProjectUi.ShortId(item.EmployeeId)),
                Left = 0,
                Top = y,
                Width = 160,
                Height = 25,
                ForeColor = ProjectUi.Text,
                TextAlign = ContentAlignment.MiddleLeft
            };
            var bar = new Panel
            {
                Left = 168,
                Top = y + 3,
                Height = 18,
                Width = Math.Max(18, (int)((_memberPanel.ClientSize.Width - 250) * item.Hours / max)),
                BackColor = Color.FromArgb(48, 126, 212)
            };
            var hours = new Label
            {
                Text = $"{item.Hours:0.##} hours",
                Left = bar.Right + 8,
                Top = y,
                Width = 75,
                Height = 25,
                ForeColor = ProjectUi.Text,
                TextAlign = ContentAlignment.MiddleLeft
            };
            _memberPanel.Controls.Add(name);
            _memberPanel.Controls.Add(bar);
            _memberPanel.Controls.Add(hours);
            y += 32;
        }
        if (groups.Length == 0)
        {
            _memberPanel.Controls.Add(ProjectUi.CreateLabel("No approved timesheets in this period."));
        }
    }
}

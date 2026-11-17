using System.Net;
using System.Text.Json;
using Admin.WinForms.BUS.Models.Project;

namespace Admin.WinForms.GUI.Controls.Project;

internal sealed class ProjectEditorDialog : Form
{
    private readonly TextBox _name = new();
    private readonly TextBox _description = new();
    private readonly DateTimePicker _startDate = ProjectPageControl.CreateDatePickerForDialog(DateTime.Today);
    private readonly DateTimePicker _endDate = ProjectPageControl.CreateDatePickerForDialog(DateTime.Today);
    private readonly ComboBox _status = ProjectUi.CreateComboBox();
    private readonly NumericUpDown _budget = new();

    internal ProjectDto Project { get; private set; } = new();

    internal ProjectEditorDialog(ProjectDto? project)
    {
        Text = project is null ? "Add Project" : "Edit Project";
        ProjectDialogLayout.Initialize(this, 540, 450);
        _name.Text = project?.ProjectName ?? string.Empty;
        _description.Text = project?.Description ?? string.Empty;
        _description.Multiline = true;
        _description.Height = 64;
        _startDate.Value = project?.StartDate.ToDateTime(TimeOnly.MinValue) ?? DateTime.Today;
        _endDate.Value = project?.EndDate?.ToDateTime(TimeOnly.MinValue) ?? DateTime.Today;
        _endDate.ShowCheckBox = true;
        _endDate.Checked = project?.EndDate.HasValue ?? false;
        _status.Items.AddRange(["Planning", "Active", "In Progress", "Completed", "On Hold"]);
        _status.SelectedItem = project?.Status;
        if (_status.SelectedIndex < 0)
        {
            _status.SelectedIndex = 0;
        }
        _budget.DecimalPlaces = 0;
        _budget.Maximum = 1_000_000_000_000;
        _budget.ThousandsSeparator = true;
        _budget.Value = Math.Clamp(project?.Budget ?? 0, _budget.Minimum, _budget.Maximum);

        var fields = ProjectDialogLayout.CreateFields();
        ProjectDialogLayout.AddField(fields, 0, "Project Name", _name);
        ProjectDialogLayout.AddField(fields, 1, "Description", _description);
        ProjectDialogLayout.AddField(fields, 2, "Start Date", _startDate);
        ProjectDialogLayout.AddField(fields, 3, "End Date", _endDate);
        ProjectDialogLayout.AddField(fields, 4, "Status", _status);
        ProjectDialogLayout.AddField(fields, 5, "Budget", _budget);
        ProjectDialogLayout.Attach(this, fields, 6);
        ProjectDialogLayout.AddButtons(this, Save);
    }

    private void Save(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_name.Text))
        {
            ProjectDialogLayout.ShowValidation(this, "Enter a project name.");
            return;
        }
        if (!_endDate.Checked)
        {
            ProjectDialogLayout.ShowValidation(this, "Enter the project end date.");
            return;
        }
        if (_endDate.Value.Date < _startDate.Value.Date)
        {
            ProjectDialogLayout.ShowValidation(this, "The end date cannot be earlier than the start date.");
            return;
        }
        Project = new ProjectDto
        {
            ProjectName = _name.Text.Trim(),
            Description = string.IsNullOrWhiteSpace(_description.Text) ? null : _description.Text.Trim(),
            StartDate = DateOnly.FromDateTime(_startDate.Value),
            EndDate = DateOnly.FromDateTime(_endDate.Value),
            Status = _status.SelectedItem?.ToString() ?? "Planning",
            Budget = _budget.Value
        };
        DialogResult = DialogResult.OK;
        Close();
    }
}

internal sealed class TaskEditorDialog : Form
{
    private readonly TextBox _name = new();
    private readonly TextBox _description = new();
    private readonly DateTimePicker _startDate = ProjectPageControl.CreateDatePickerForDialog(DateTime.Today);
    private readonly DateTimePicker _dueDate = ProjectPageControl.CreateDatePickerForDialog(DateTime.Today);
    private readonly ComboBox _status = ProjectUi.CreateComboBox();
    private readonly ComboBox _priority = ProjectUi.CreateComboBox();
    private readonly Guid _projectId;

    internal TaskDto Task { get; private set; } = new();

    internal TaskEditorDialog(Guid projectId, TaskDto? task)
    {
        _projectId = projectId;
        Text = task is null ? "Add Task" : "Edit Task";
        ProjectDialogLayout.Initialize(this, 540, 460);
        _name.Text = task?.TaskName ?? string.Empty;
        _description.Text = task?.Description ?? string.Empty;
        _description.Multiline = true;
        _description.Height = 58;
        _startDate.Value = task?.StartDate.Date ?? DateTime.Today;
        _dueDate.Value = task?.DueDate?.Date ?? DateTime.Today;
        _dueDate.ShowCheckBox = true;
        _dueDate.Checked = task?.DueDate.HasValue ?? false;
        _status.Items.AddRange(["Pending", "In Progress", "Completed", "On Hold"]);
        _status.SelectedItem = task?.Status;
        if (_status.SelectedIndex < 0)
        {
            _status.SelectedIndex = 0;
        }
        _priority.Items.AddRange(["High", "Medium", "Low"]);
        _priority.SelectedItem = task?.Priority;
        if (_priority.SelectedIndex < 0)
        {
            _priority.SelectedIndex = 1;
        }

        var fields = ProjectDialogLayout.CreateFields();
        ProjectDialogLayout.AddField(fields, 0, "Task Name", _name);
        ProjectDialogLayout.AddField(fields, 1, "Description", _description);
        ProjectDialogLayout.AddField(fields, 2, "Start Date", _startDate);
        ProjectDialogLayout.AddField(fields, 3, "Due Date", _dueDate);
        ProjectDialogLayout.AddField(fields, 4, "Status", _status);
        ProjectDialogLayout.AddField(fields, 5, "Priority", _priority);
        ProjectDialogLayout.Attach(this, fields, 6);
        ProjectDialogLayout.AddButtons(this, Save);
    }

    private void Save(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_name.Text))
        {
            ProjectDialogLayout.ShowValidation(this, "Enter a task name.");
            return;
        }
        if (_dueDate.Checked && _dueDate.Value.Date < _startDate.Value.Date)
        {
            ProjectDialogLayout.ShowValidation(this, "The due date cannot be earlier than the start date.");
            return;
        }
        Task = new TaskDto
        {
            TaskName = _name.Text.Trim(),
            Description = string.IsNullOrWhiteSpace(_description.Text) ? null : _description.Text.Trim(),
            StartDate = _startDate.Value.Date,
            DueDate = _dueDate.Checked ? _dueDate.Value.Date : null,
            Status = _status.SelectedItem?.ToString() ?? "Pending",
            Priority = _priority.SelectedItem?.ToString() ?? "Medium",
            ProjectId = _projectId
        };
        DialogResult = DialogResult.OK;
        Close();
    }
}

internal sealed class ProjectMemberEditorDialog : Form
{
    private readonly Guid _projectId;
    private readonly DateOnly _projectEndDate;
    private readonly bool _isNewMember;
    private readonly IReadOnlyList<ProjectRoleDto> _roles;
    private readonly Func<Task<List<EmployeeDirectoryDto>>> _loadEmployees;
    private readonly TextBox _employeeId = new();
    private readonly TextBox _roleName = new();
    private readonly NumericUpDown _newRoleCoefficient = new();
    private readonly DateTimePicker _joinDate = ProjectPageControl.CreateDatePickerForDialog(DateTime.Today);
    private readonly DateTimePicker _leaveDate = ProjectPageControl.CreateDatePickerForDialog(DateTime.Today);
    private readonly DateTimePicker _hourlyRateEffectiveFrom = ProjectPageControl.CreateDatePickerForDialog(DateTime.Today);
    private readonly ComboBox _status = ProjectUi.CreateComboBox();
    private readonly NumericUpDown _hourlyRate = new();
    private Label? _newRoleCoefficientLabel;

    internal ProjectMemberDto Member { get; private set; } = new();
    internal string EmployeeIdentifier => _employeeId.Text.Trim();

    internal ProjectMemberEditorDialog(
        ProjectDto project,
        IReadOnlyList<ProjectRoleDto> roles,
        ProjectMemberDto? member,
        Func<Task<List<EmployeeDirectoryDto>>> loadEmployees)
    {
        _projectId = project.ProjectId;
        _projectEndDate = project.EndDate
            ?? throw new ArgumentException("Project end date is required.", nameof(project));
        _isNewMember = member is null;
        _roles = roles;
        _loadEmployees = loadEmployees;
        Text = _isNewMember ? "Add Project Member" : "Edit Project Member";
        ProjectDialogLayout.Initialize(this, 540, _isNewMember ? 510 : 560);
        _employeeId.Text = member?.EmployeeId.ToString() ?? string.Empty;
        _employeeId.PlaceholderText = "Employee code or GUID";
        _employeeId.ReadOnly = !_isNewMember;
        _roleName.PlaceholderText = "Type a role name";
        _roleName.MaxLength = 100;
        _roleName.Text = member?.RoleName ?? string.Empty;
        _newRoleCoefficient.DecimalPlaces = 4;
        _newRoleCoefficient.Maximum = 9.9999m;
        _newRoleCoefficient.Value = 1m;
        _hourlyRate.DecimalPlaces = 2;
        _hourlyRate.Maximum = 1_000_000_000_000m;
        _hourlyRate.ThousandsSeparator = true;
        _joinDate.Value = member?.JoinDate.Date ?? DateTime.Today;
        if (member is not null && member.JoinDate.Date > DateTime.Today)
        {
            _hourlyRateEffectiveFrom.Value = member.JoinDate.Date;
        }
        _joinDate.ValueChanged += (_, _) =>
        {
            if (_hourlyRateEffectiveFrom.Value.Date < _joinDate.Value.Date)
            {
                _hourlyRateEffectiveFrom.Value = _joinDate.Value.Date;
            }
        };
        _leaveDate.Value = member?.LeaveDate?.Date
            ?? _projectEndDate.ToDateTime(TimeOnly.MinValue);
        _status.Items.AddRange(["Active", "Inactive", "On Leave"]);
        _status.SelectedItem = member?.Status;
        if (_status.SelectedIndex < 0)
        {
            _status.SelectedIndex = 0;
        }

        var fields = ProjectDialogLayout.CreateFields();
        ProjectDialogLayout.AddField(
            fields,
            0,
            "Employee Code or GUID",
            ProjectDialogLayout.CreateBrowseField(_employeeId, BrowseEmployees, _isNewMember));
        ProjectDialogLayout.AddField(fields, 1, "Project Role", _roleName);
        _newRoleCoefficientLabel = ProjectDialogLayout.AddField(
            fields,
            2,
            "Role Coefficient",
            _newRoleCoefficient);
        ProjectDialogLayout.AddField(fields, 3, "Join Date", _joinDate);
        ProjectDialogLayout.AddField(fields, 4, "Leave Date", _leaveDate);
        ProjectDialogLayout.AddField(fields, 5, "Status", _status);
        ProjectDialogLayout.AddField(fields, 6, _isNewMember ? "Hourly Rate" : "New Rate (0 = skip)", _hourlyRate);
        if (!_isNewMember)
        {
            ProjectDialogLayout.AddField(fields, 7, "Rate Effective Date", _hourlyRateEffectiveFrom);
        }
        _roleName.TextChanged += (_, _) => UpdateNewRoleCoefficientVisibility(fields);
        UpdateNewRoleCoefficientVisibility(fields);
        ProjectDialogLayout.Attach(this, fields, _isNewMember ? 7 : 8);
        ProjectDialogLayout.AddButtons(this, Save);
    }

    private async void BrowseEmployees(object? sender, EventArgs e)
    {
        try
        {
            var employees = await _loadEmployees();
            var employeeId = ProjectLookupDialog.Select(
                this,
                "Select Employee",
                employees
                    .OrderBy(employee => employee.FullName, StringComparer.OrdinalIgnoreCase)
                    .Select(employee => new ProjectLookupItem(
                        employee.EmployeeId,
                        employee.EmployeeId,
                        employee.EmployeeCode,
                        employee.FullName,
                        "Active employee")));
            if (employeeId.HasValue)
            {
                _employeeId.Text = employeeId.Value.ToString();
            }
        }
        catch (HttpRequestException ex)
        {
            ProjectDialogLayout.ShowValidation(
                this,
                ex.StatusCode == HttpStatusCode.ServiceUnavailable
                    ? "The HR employee directory is unavailable."
                    : "Could not load employees. Check the HR API and try again.");
        }
        catch (JsonException)
        {
            ProjectDialogLayout.ShowValidation(this, "The HR employee directory returned invalid data.");
        }
        catch (TaskCanceledException)
        {
            ProjectDialogLayout.ShowValidation(this, "Loading employees timed out. Please try again.");
        }
    }

    private void Save(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EmployeeIdentifier))
        {
            ProjectDialogLayout.ShowValidation(this, "Enter an employee code or GUID.");
            return;
        }
        var roleName = _roleName.Text.Trim();
        if (string.IsNullOrWhiteSpace(roleName))
        {
            ProjectDialogLayout.ShowValidation(this, "Enter a project role.");
            return;
        }
        var existingRole = _roles.FirstOrDefault(role =>
            string.Equals(role.RoleName, roleName, StringComparison.OrdinalIgnoreCase));
        if (_isNewMember && _hourlyRate.Value <= 0)
        {
            ProjectDialogLayout.ShowValidation(this, "Enter an hourly rate greater than 0.");
            return;
        }
        if (_leaveDate.Value.Date < _joinDate.Value.Date ||
            DateOnly.FromDateTime(_leaveDate.Value) > _projectEndDate)
        {
            ProjectDialogLayout.ShowValidation(
                this,
                "The leave date must be on or after the join date and no later than the project end date.");
            return;
        }
        if (!_isNewMember && _hourlyRate.Value > 0 &&
            (_hourlyRateEffectiveFrom.Value.Date < _joinDate.Value.Date ||
             _hourlyRateEffectiveFrom.Value.Date > _leaveDate.Value.Date))
        {
            ProjectDialogLayout.ShowValidation(this, "The rate effective date must fall within the member's date range.");
            return;
        }
        Member = new ProjectMemberDto
        {
            ProjectId = _projectId,
            EmployeeId = Guid.TryParse(EmployeeIdentifier, out var employeeId) ? employeeId : Guid.Empty,
            ProjectRoleId = existingRole?.ProjectRoleId ?? Guid.Empty,
            RoleName = roleName,
            ProjectRoleName = roleName,
            NewRoleCoefficient = existingRole is null ? _newRoleCoefficient.Value : null,
            JoinDate = _joinDate.Value.Date,
            LeaveDate = _leaveDate.Value.Date,
            Status = _status.SelectedItem?.ToString() ?? "Active",
            InitialHourlyRate = _isNewMember || _hourlyRate.Value > 0 ? _hourlyRate.Value : null,
            HourlyRateEffectiveFrom = !_isNewMember && _hourlyRate.Value > 0
                ? _hourlyRateEffectiveFrom.Value.Date
                : null
        };
        DialogResult = DialogResult.OK;
        Close();
    }

    private void UpdateNewRoleCoefficientVisibility(TableLayoutPanel fields)
    {
        var roleName = _roleName.Text.Trim();
        var isExistingRole = _roles.Any(role =>
            string.Equals(role.RoleName, roleName, StringComparison.OrdinalIgnoreCase));
        var showCoefficient = roleName.Length > 0 && !isExistingRole;
        _newRoleCoefficient.Visible = showCoefficient;
        if (_newRoleCoefficientLabel is not null)
        {
            _newRoleCoefficientLabel.Visible = showCoefficient;
        }
        fields.RowStyles[2].Height = showCoefficient ? 48 : 0;
        fields.PerformLayout();
    }

}

internal sealed class TimesheetEditorDialog : Form
{
    private readonly TaskDto? _task;
    private readonly TimesheetDto? _sheet;
    private readonly ComboBox _member = ProjectUi.CreateComboBox();
    private readonly DateTimePicker _workDate = ProjectPageControl.CreateDatePickerForDialog(DateTime.Today);
    private readonly NumericUpDown _hours = new();
    private readonly TextBox _description = new();

    internal CreateTimesheetRequest Request { get; private set; } = new();
    internal UpdateTimesheetRequest UpdateRequest { get; private set; } = new();

    internal TimesheetEditorDialog(
        TaskDto? task,
        IReadOnlyList<ProjectMemberDto> members,
        TimesheetDto? sheet,
        IReadOnlyDictionary<Guid, EmployeeDirectoryDto>? employeeDirectory = null)
    {
        _task = task;
        _sheet = sheet;
        Text = sheet is null ? "Add Timesheet" : "Edit Timesheet";
        ProjectDialogLayout.Initialize(this, 540, sheet is null ? 350 : 300);
        _workDate.Value = sheet?.WorkDate.Date ?? DateTime.Today;
        _hours.DecimalPlaces = 2;
        _hours.Increment = 0.5M;
        _hours.Minimum = 0.01M;
        _hours.Maximum = 24;
        _hours.Value = sheet?.HoursWorked ?? 8;
        _workDate.Enabled = sheet is null;
        _description.Text = sheet?.Description ?? string.Empty;
        _description.Multiline = true;
        _description.Height = 58;

        var fields = ProjectDialogLayout.CreateFields();
        var row = 0;
        if (sheet is null)
        {
            _member.DisplayMember = nameof(MemberChoice.Name);
            foreach (var item in members)
            {
                var employee = employeeDirectory?.GetValueOrDefault(item.EmployeeId);
                var employeeName = employee is null
                    ? ProjectUi.ShortId(item.EmployeeId)
                    : $"{employee.EmployeeCode} - {employee.FullName}";
                _member.Items.Add(new MemberChoice(item.ProjectMemberId, $"{employeeName} - {item.RoleName}"));
            }
            if (_member.Items.Count > 0)
            {
                _member.SelectedIndex = 0;
            }
            ProjectDialogLayout.AddField(
                fields,
                row++,
                "Project Member",
                ProjectDialogLayout.CreateBrowseField(
                    _member,
                    (_, _) => BrowseProjectMembers(members, employeeDirectory),
                    enabled: members.Count > 0));
        }
        ProjectDialogLayout.AddField(fields, row++, "Work Date", _workDate);
        ProjectDialogLayout.AddField(fields, row++, "Hours Worked", _hours);
        ProjectDialogLayout.AddField(fields, row, "Notes", _description);
        ProjectDialogLayout.Attach(this, fields, row + 1);
        ProjectDialogLayout.AddButtons(this, Save);
    }

    private void BrowseProjectMembers(
        IReadOnlyList<ProjectMemberDto> members,
        IReadOnlyDictionary<Guid, EmployeeDirectoryDto>? employeeDirectory)
    {
        var choices = members
            .OrderBy(member => employeeDirectory?.GetValueOrDefault(member.EmployeeId)?.FullName ?? member.EmployeeId.ToString(),
                StringComparer.OrdinalIgnoreCase)
            .Select(member => new ProjectLookupItem(
                member.ProjectMemberId,
                member.EmployeeId,
                employeeDirectory?.GetValueOrDefault(member.EmployeeId)?.EmployeeCode ?? ProjectUi.ShortId(member.EmployeeId),
                employeeDirectory?.GetValueOrDefault(member.EmployeeId)?.FullName ?? ProjectUi.ShortId(member.EmployeeId),
                $"{member.RoleName} - {member.Status}"))
            .ToList();
        var projectMemberId = ProjectLookupDialog.Select(this, "Select Project Member", choices);
        if (!projectMemberId.HasValue)
        {
            return;
        }

        for (var index = 0; index < _member.Items.Count; index++)
        {
            if (_member.Items[index] is MemberChoice choice &&
                choice.ProjectMemberId == projectMemberId.Value)
            {
                _member.SelectedIndex = index;
                break;
            }
        }
    }

    private void Save(object? sender, EventArgs e)
    {
        if (_sheet is null)
        {
            if (_task is null || _member.SelectedItem is not MemberChoice member)
            {
                ProjectDialogLayout.ShowValidation(this, "Select a project member.");
                return;
            }
            Request = new CreateTimesheetRequest
            {
                ProjectMemberId = member.ProjectMemberId,
                TaskId = _task.TaskId,
                WorkDate = _workDate.Value.Date,
                HoursWorked = _hours.Value,
                Description = string.IsNullOrWhiteSpace(_description.Text) ? null : _description.Text.Trim()
            };
        }
        else
        {
            UpdateRequest = new UpdateTimesheetRequest
            {
                HoursWorked = _hours.Value,
                Description = string.IsNullOrWhiteSpace(_description.Text) ? null : _description.Text.Trim()
            };
        }
        DialogResult = DialogResult.OK;
        Close();
    }

    private sealed record MemberChoice(Guid ProjectMemberId, string Name);
}

internal sealed class ApproverIdDialog : Form
{
    private readonly TextBox _approverId = new();
    private readonly Func<Task<List<EmployeeDirectoryDto>>> _loadEmployees;

    internal string EmployeeIdentifier => _approverId.Text.Trim();

    internal ApproverIdDialog(Func<Task<List<EmployeeDirectoryDto>>> loadEmployees)
    {
        _loadEmployees = loadEmployees;
        Text = "Timesheet Approver";
        ProjectDialogLayout.Initialize(this, 500, 170);
        _approverId.PlaceholderText = "Employee code, e.g. E2E1025";
        var fields = ProjectDialogLayout.CreateFields();
        ProjectDialogLayout.AddField(
            fields,
            0,
            "Employee ID",
            ProjectDialogLayout.CreateBrowseField(_approverId, BrowseEmployees));
        ProjectDialogLayout.Attach(this, fields, 1);
        ProjectDialogLayout.AddButtons(this, Save);
    }

    private async void BrowseEmployees(object? sender, EventArgs e)
    {
        try
        {
            var employees = await _loadEmployees();
            var employeeId = ProjectLookupDialog.Select(
                this,
                "Select Approver",
                employees
                    .OrderBy(employee => employee.FullName, StringComparer.OrdinalIgnoreCase)
                    .Select(employee => new ProjectLookupItem(
                        employee.EmployeeId,
                        employee.EmployeeId,
                        employee.EmployeeCode,
                        employee.FullName,
                        "Active employee")));
            if (employeeId.HasValue)
            {
                _approverId.Text = employeeId.Value.ToString();
            }
        }
        catch (HttpRequestException ex)
        {
            ProjectDialogLayout.ShowValidation(
                this,
                ex.StatusCode == HttpStatusCode.ServiceUnavailable
                    ? "The HR employee directory is unavailable."
                    : "Could not load employees. Check the HR API and try again.");
        }
        catch (JsonException)
        {
            ProjectDialogLayout.ShowValidation(this, "The HR employee directory returned invalid data.");
        }
        catch (TaskCanceledException)
        {
            ProjectDialogLayout.ShowValidation(this, "Loading employees timed out. Please try again.");
        }
    }

    private void Save(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EmployeeIdentifier))
        {
            ProjectDialogLayout.ShowValidation(this, "Enter an employee ID.");
            return;
        }
        DialogResult = DialogResult.OK;
        Close();
    }
}

internal sealed record ProjectLookupItem(
    Guid Id,
    Guid EmployeeId,
    string EmployeeCode,
    string FullName,
    string Details);

internal sealed class ProjectLookupDialog : Form
{
    private readonly IReadOnlyList<ProjectLookupItem> _items;
    private readonly TextBox _search = ProjectUi.CreateSearchBox("Search name, code, or ID");
    private readonly ListView _list = new();
    private readonly Label _summary = new();
    private readonly Button _select = ProjectUi.CreateButton("Select", primary: true);

    private ProjectLookupDialog(string title, IReadOnlyList<ProjectLookupItem> items)
    {
        _items = items;
        Text = title;
        ProjectDialogLayout.Initialize(this, 780, 520);
        MinimizeBox = false;

        _list.View = View.Details;
        _list.Dock = DockStyle.Fill;
        _list.FullRowSelect = true;
        _list.HideSelection = false;
        _list.MultiSelect = false;
        _list.GridLines = true;
        _list.Columns.Add("Employee", 220);
        _list.Columns.Add("Code", 110);
        _list.Columns.Add("Employee ID", 270);
        _list.Columns.Add("Project Member / Status", 150);
        _list.SelectedIndexChanged += (_, _) => _select.Enabled = _list.SelectedItems.Count > 0;
        _list.DoubleClick += (_, _) => SelectCurrent();

        _summary.Dock = DockStyle.Fill;
        _summary.TextAlign = ContentAlignment.MiddleLeft;
        _summary.ForeColor = ProjectUi.MutedText;

        var searchPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 58,
            ColumnCount = 1,
            Padding = new Padding(12, 8, 12, 6),
            BackColor = ProjectUi.Background
        };
        searchPanel.Controls.Add(_search, 0, 0);

        var content = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(12, 0, 12, 8),
            BackColor = ProjectUi.Background
        };
        content.Controls.Add(_list);

        var footer = new TableLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 54,
            ColumnCount = 2,
            Padding = new Padding(12, 5, 12, 5),
            BackColor = ProjectUi.Background
        };
        footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        footer.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 190));
        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false
        };
        var cancel = ProjectUi.CreateButton("Cancel");
        cancel.Width = 86;
        cancel.DialogResult = DialogResult.Cancel;
        _select.Width = 86;
        _select.Enabled = false;
        _select.Click += (_, _) => SelectCurrent();
        buttons.Controls.Add(cancel);
        buttons.Controls.Add(_select);
        footer.Controls.Add(_summary, 0, 0);
        footer.Controls.Add(buttons, 1, 0);

        Controls.Add(content);
        Controls.Add(searchPanel);
        Controls.Add(footer);
        AcceptButton = _select;
        CancelButton = cancel;
        _search.TextChanged += (_, _) => UpdateRows();
        UpdateRows();
    }

    internal static Guid? Select(
        IWin32Window owner,
        string title,
        IEnumerable<ProjectLookupItem> items)
    {
        using var dialog = new ProjectLookupDialog(title, items.ToList());
        return dialog.ShowDialog(owner) == DialogResult.OK &&
               dialog._list.SelectedItems.Count > 0 &&
               dialog._list.SelectedItems[0].Tag is Guid id
            ? id
            : null;
    }

    private void UpdateRows()
    {
        var query = _search.Text.Trim();
        var visibleItems = _items.Where(item =>
            query.Length == 0 ||
            $"{item.FullName} {item.EmployeeCode} {item.EmployeeId} {item.Details}"
                .Contains(query, StringComparison.OrdinalIgnoreCase));

        _list.BeginUpdate();
        _list.Items.Clear();
        foreach (var item in visibleItems)
        {
            var row = new ListViewItem(item.FullName);
            row.SubItems.Add(item.EmployeeCode);
            row.SubItems.Add(item.EmployeeId.ToString());
            row.SubItems.Add(item.Details);
            row.Tag = item.Id;
            _list.Items.Add(row);
        }
        _list.EndUpdate();
        _summary.Text = $"{_list.Items.Count} employee/member(s)";
        _select.Enabled = false;
    }

    private void SelectCurrent()
    {
        if (_list.SelectedItems.Count > 0)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}

internal static class ProjectDialogLayout
{
    internal static void Initialize(Form form, int width, int height)
    {
        form.ClientSize = new Size(width, height);
        form.FormBorderStyle = FormBorderStyle.FixedDialog;
        form.StartPosition = FormStartPosition.CenterParent;
        form.MaximizeBox = false;
        form.MinimizeBox = false;
        form.BackColor = ProjectUi.Background;
        form.ForeColor = ProjectUi.Text;
        form.Font = new Font("Segoe UI", 10F);
    }

    internal static TableLayoutPanel CreateFields()
    {
        return new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            BackColor = ProjectUi.Background,
            Padding = new Padding(18, 12, 18, 0),
            AutoScroll = true
        };
    }

    internal static Label AddField(TableLayoutPanel layout, int row, string label, Control input)
    {
        while (layout.RowCount <= row)
        {
            layout.RowCount++;
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        }
        layout.ColumnStyles.Clear();
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowStyles[row].Height = input.Height > 48 ? input.Height + 8 : 48;
        var fieldLabel = new Label
        {
            Text = label,
            ForeColor = ProjectUi.Text,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };
        layout.Controls.Add(fieldLabel, 0, row);
        input.Dock = DockStyle.Fill;
        input.Margin = new Padding(0, 5, 0, 5);
        if (input is TextBox textBox)
        {
            textBox.BackColor = ProjectUi.Background;
            textBox.ForeColor = ProjectUi.Text;
            textBox.BorderStyle = BorderStyle.FixedSingle;
        }
        if (input is ComboBox combo)
        {
            combo.BackColor = ProjectUi.Background;
            combo.ForeColor = ProjectUi.Text;
        }
        if (input is DateTimePicker datePicker)
        {
            datePicker.Dock = DockStyle.Left;
        }
        if (input is NumericUpDown numeric)
        {
            numeric.BackColor = ProjectUi.Background;
            numeric.ForeColor = ProjectUi.Text;
        }
        layout.Controls.Add(input, 1, row);
        return fieldLabel;
    }

    internal static Control CreateBrowseField(
        Control input,
        EventHandler browse,
        bool enabled = true)
    {
        var field = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = Padding.Empty,
            Padding = Padding.Empty,
            BackColor = ProjectUi.Background
        };
        field.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        field.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 42));
        input.Dock = DockStyle.Fill;
        input.Margin = new Padding(0, 5, 6, 5);
        field.Controls.Add(input, 0, 0);

        var button = ProjectUi.CreateButton("...");
        button.Width = 36;
        button.Dock = DockStyle.Fill;
        button.Margin = new Padding(0, 5, 0, 5);
        button.Enabled = enabled;
        button.AccessibleName = "Browse employees or project members";
        button.Click += browse;
        field.Controls.Add(button, 1, 0);
        return field;
    }

    internal static void Attach(Form form, TableLayoutPanel fields, int rowCount)
    {
        fields.RowCount = Math.Max(fields.RowCount, rowCount);
        form.Controls.Add(fields);
    }

    internal static void AddButtons(Form form, EventHandler save)
    {
        var footer = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 54,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            Padding = new Padding(0, 6, 12, 0),
            BackColor = ProjectUi.Background
        };
        var ok = ProjectUi.CreateButton("Save", primary: true);
        ok.Width = 98;
        ok.Height = 36;
        ok.Click += save;
        var cancel = ProjectUi.CreateButton("Cancel");
        cancel.Width = 98;
        cancel.Height = 36;
        cancel.DialogResult = DialogResult.Cancel;
        footer.Controls.Add(cancel);
        footer.Controls.Add(ok);
        form.AcceptButton = ok;
        form.CancelButton = cancel;
        form.Controls.Add(footer);
    }

    internal static void ShowValidation(IWin32Window owner, string message)
    {
        MessageBox.Show(owner, message, "Invalid Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}

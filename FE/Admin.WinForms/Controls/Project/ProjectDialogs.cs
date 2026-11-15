using Admin.WinForms.Models.Project;

namespace Admin.WinForms.Controls.Project;

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

    internal ProjectMemberEditorDialog(ProjectDto project, IReadOnlyList<ProjectRoleDto> roles, ProjectMemberDto? member)
    {
        _projectId = project.ProjectId;
        _projectEndDate = project.EndDate
            ?? throw new ArgumentException("Project end date is required.", nameof(project));
        _isNewMember = member is null;
        _roles = roles;
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
        ProjectDialogLayout.AddField(fields, 0, "Employee Code or GUID", _employeeId);
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

    internal TimesheetEditorDialog(TaskDto? task, IReadOnlyList<ProjectMemberDto> members, TimesheetDto? sheet)
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
                _member.Items.Add(new MemberChoice(item.ProjectMemberId, $"{ProjectUi.ShortId(item.EmployeeId)} - {item.RoleName}"));
            }
            if (_member.Items.Count > 0)
            {
                _member.SelectedIndex = 0;
            }
            ProjectDialogLayout.AddField(fields, row++, "Project Member", _member);
        }
        ProjectDialogLayout.AddField(fields, row++, "Work Date", _workDate);
        ProjectDialogLayout.AddField(fields, row++, "Hours Worked", _hours);
        ProjectDialogLayout.AddField(fields, row, "Notes", _description);
        ProjectDialogLayout.Attach(this, fields, row + 1);
        ProjectDialogLayout.AddButtons(this, Save);
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

    internal string EmployeeIdentifier => _approverId.Text.Trim();

    internal ApproverIdDialog()
    {
        Text = "Timesheet Approver";
        ProjectDialogLayout.Initialize(this, 500, 170);
        _approverId.PlaceholderText = "Employee code, e.g. E2E1025";
        var fields = ProjectDialogLayout.CreateFields();
        ProjectDialogLayout.AddField(fields, 0, "Employee ID", _approverId);
        ProjectDialogLayout.Attach(this, fields, 1);
        ProjectDialogLayout.AddButtons(this, Save);
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

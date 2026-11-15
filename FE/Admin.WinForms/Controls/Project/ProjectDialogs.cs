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
        _startDate.Value = project?.StartDate.Date ?? DateTime.Today;
        _endDate.Value = project?.EndDate?.Date ?? DateTime.Today;
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
        if (_endDate.Checked && _endDate.Value.Date < _startDate.Value.Date)
        {
            ProjectDialogLayout.ShowValidation(this, "The end date cannot be earlier than the start date.");
            return;
        }
        Project = new ProjectDto
        {
            ProjectName = _name.Text.Trim(),
            Description = string.IsNullOrWhiteSpace(_description.Text) ? null : _description.Text.Trim(),
            StartDate = _startDate.Value.Date,
            EndDate = _endDate.Checked ? _endDate.Value.Date : null,
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
    private readonly TextBox _employeeId = new();
    private readonly ComboBox _role = ProjectUi.CreateComboBox();
    private readonly DateTimePicker _joinDate = ProjectPageControl.CreateDatePickerForDialog(DateTime.Today);
    private readonly DateTimePicker _leaveDate = ProjectPageControl.CreateDatePickerForDialog(DateTime.Today);
    private readonly ComboBox _status = ProjectUi.CreateComboBox();

    internal ProjectMemberDto Member { get; private set; } = new();

    internal ProjectMemberEditorDialog(Guid projectId, IReadOnlyList<ProjectRoleDto> roles, ProjectMemberDto? member)
    {
        _projectId = projectId;
        Text = member is null ? "Add Project Member" : "Edit Project Member";
        ProjectDialogLayout.Initialize(this, 540, 390);
        _employeeId.Text = member?.EmployeeId.ToString() ?? string.Empty;
        _employeeId.PlaceholderText = "Employee GUID from HR";
        _role.DisplayMember = nameof(ProjectRoleDto.RoleName);
        foreach (var role in roles)
        {
            _role.Items.Add(role);
        }
        _role.SelectedItem = roles.FirstOrDefault(role => role.ProjectRoleId == member?.ProjectRoleId)
            ?? _role.Items.Cast<ProjectRoleDto>().FirstOrDefault();
        _joinDate.Value = member?.JoinDate.Date ?? DateTime.Today;
        _leaveDate.Value = member?.LeaveDate?.Date ?? DateTime.Today;
        _leaveDate.ShowCheckBox = true;
        _leaveDate.Checked = member?.LeaveDate.HasValue ?? false;
        _status.Items.AddRange(["Active", "Inactive", "On Leave"]);
        _status.SelectedItem = member?.Status;
        if (_status.SelectedIndex < 0)
        {
            _status.SelectedIndex = 0;
        }

        var fields = ProjectDialogLayout.CreateFields();
        ProjectDialogLayout.AddField(fields, 0, "Employee ID (GUID)", _employeeId);
        ProjectDialogLayout.AddField(fields, 1, "Project Role", _role);
        ProjectDialogLayout.AddField(fields, 2, "Join Date", _joinDate);
        ProjectDialogLayout.AddField(fields, 3, "Leave Date", _leaveDate);
        ProjectDialogLayout.AddField(fields, 4, "Status", _status);
        ProjectDialogLayout.Attach(this, fields, 5);
        ProjectDialogLayout.AddButtons(this, Save);
    }

    private void Save(object? sender, EventArgs e)
    {
        if (!Guid.TryParse(_employeeId.Text, out var employeeId) || employeeId == Guid.Empty)
        {
            ProjectDialogLayout.ShowValidation(this, "Enter a valid employee GUID.");
            return;
        }
        if (_role.SelectedItem is not ProjectRoleDto role)
        {
            ProjectDialogLayout.ShowValidation(this, "Select a configured project role.");
            return;
        }
        if (_leaveDate.Checked && _leaveDate.Value.Date < _joinDate.Value.Date)
        {
            ProjectDialogLayout.ShowValidation(this, "The leave date cannot be earlier than the join date.");
            return;
        }
        Member = new ProjectMemberDto
        {
            ProjectId = _projectId,
            EmployeeId = employeeId,
            ProjectRoleId = role.ProjectRoleId,
            JoinDate = _joinDate.Value.Date,
            LeaveDate = _leaveDate.Checked ? _leaveDate.Value.Date : null,
            Status = _status.SelectedItem?.ToString() ?? "Active"
        };
        DialogResult = DialogResult.OK;
        Close();
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

    internal Guid ApproverId { get; private set; }

    internal ApproverIdDialog()
    {
        Text = "Timesheet Approver";
        ProjectDialogLayout.Initialize(this, 500, 170);
        _approverId.PlaceholderText = "Approver employee GUID";
        var fields = ProjectDialogLayout.CreateFields();
        ProjectDialogLayout.AddField(fields, 0, "Approver ID", _approverId);
        ProjectDialogLayout.Attach(this, fields, 1);
        ProjectDialogLayout.AddButtons(this, Save);
    }

    private void Save(object? sender, EventArgs e)
    {
        if (!Guid.TryParse(_approverId.Text, out var approverId) || approverId == Guid.Empty)
        {
            ProjectDialogLayout.ShowValidation(this, "Enter a valid approver GUID.");
            return;
        }
        ApproverId = approverId;
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

    internal static void AddField(TableLayoutPanel layout, int row, string label, Control input)
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
        layout.Controls.Add(new Label
        {
            Text = label,
            ForeColor = ProjectUi.Text,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, row);
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

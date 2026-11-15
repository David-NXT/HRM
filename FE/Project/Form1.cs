using System.Net;
using System.Text.Json;
using Project.DTOs;
using Project.Services;

namespace Project
{
    public partial class Form1 : Form
    {
        private readonly ProjectApiService _projectApi;
        private List<ProjectDto> _projects = [];

        public Form1() : this(Program.ProjectApi)
        {
        }

        internal Form1(ProjectApiService projectApi)
        {
            _projectApi = projectApi;
            InitializeComponent();

            txtSearch.TextChanged += (_, _) => ApplyFilters();
            cboStatus.SelectedIndexChanged += (_, _) => ApplyFilters();
            Shown += async (_, _) => await LoadProjectsAsync();
        }

        private async Task<bool> LoadProjectsAsync()
        {
            SetBusy(true);
            try
            {
                _projects = await _projectApi.GetAllAsync();
                ApplyFilters();
                return true;
            }
            catch (HttpRequestException ex)
            {
                ShowRequestError(ex);
                return false;
            }
            catch (JsonException)
            {
                ShowErrorNotification("The project information could not be read. Please refresh and try again.");
                return false;
            }
            catch (InvalidDataException)
            {
                ShowErrorNotification("No project information was received. Please refresh and try again.");
                return false;
            }
            catch (TaskCanceledException)
            {
                ShowErrorNotification("The request took too long. Please try again.");
                return false;
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void ApplyFilters()
        {
            var search = txtSearch.Text.Trim();
            var status = cboStatus.SelectedItem?.ToString();

            lvProjects.BeginUpdate();
            lvProjects.Items.Clear();

            foreach (var project in _projects.Where(project =>
                         (string.IsNullOrEmpty(search)
                          || project.ProjectName.Contains(search, StringComparison.OrdinalIgnoreCase)
                          || (project.Description?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false)
                          || project.ProjectId.ToString().Contains(search, StringComparison.OrdinalIgnoreCase))
                         && (string.IsNullOrEmpty(status)
                             || status == "All"
                             || string.Equals(project.Status, status, StringComparison.OrdinalIgnoreCase))))
            {
                var item = new ListViewItem(project.ProjectId.ToString("N")[..8]);
                item.SubItems.Add(project.ProjectName);
                item.SubItems.Add(project.StartDate.ToString("MM/dd/yyyy"));
                item.SubItems.Add(project.EndDate?.ToString("MM/dd/yyyy") ?? "Not set");
                item.SubItems.Add(project.Status);
                item.SubItems.Add(project.Budget.ToString("N0"));
                item.SubItems.Add(project.Description ?? string.Empty);
                item.Tag = project;
                lvProjects.Items.Add(item);
            }

            lvProjects.EndUpdate();
            lblTotal.Text = $"Showing: {lvProjects.Items.Count} / {_projects.Count} projects";
            UpdateSelectionButtons();
        }

        private void SetBusy(bool isBusy)
        {
            btnRefresh.Enabled = !isBusy;
            btnAdd.Enabled = !isBusy;
            btnEdit.Enabled = !isBusy && lvProjects.SelectedItems.Count > 0;
            btnDelete.Enabled = !isBusy && lvProjects.SelectedItems.Count > 0;
            UseWaitCursor = isBusy;
        }

        private void UpdateSelectionButtons()
        {
            var hasSelection = lvProjects.SelectedItems.Count > 0;
            btnEdit.Enabled = hasSelection;
            btnDelete.Enabled = hasSelection;
        }

        private static void ShowRequestError(HttpRequestException ex)
        {
            var message = ex.StatusCode switch
            {
                HttpStatusCode.BadRequest => "Some project details are invalid. Please review them and try again.",
                HttpStatusCode.Unauthorized => "Your session has expired. Please sign in again.",
                HttpStatusCode.Forbidden => "You do not have permission to perform this action.",
                HttpStatusCode.NotFound => "The selected project could not be found. Refresh the list and try again.",
                HttpStatusCode.Conflict => "The project has changed. Refresh the list and try again.",
                null => "Unable to connect right now. Please check your connection and try again.",
                _ => "We couldn't complete your request."
            };

            ShowErrorNotification(message);
        }

        private static void ShowErrorNotification(string message)
        {
            MessageBox.Show(message, "Notification", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private ProjectDto? GetSelectedProject()
        {
            return lvProjects.SelectedItems.Count > 0
                ? lvProjects.SelectedItems[0].Tag as ProjectDto
                : null;
        }

        private async void btnRefresh_Click(object? sender, EventArgs e)
        {
            await LoadProjectsAsync();
        }

        private async void btnAdd_Click(object? sender, EventArgs e)
        {
            using var editor = new ProjectEditorForm();
            if (editor.ShowDialog(this) != DialogResult.OK || editor.ProjectInput is null)
            {
                return;
            }

            await SaveProjectAsync(() => _projectApi.CreateAsync(editor.ProjectInput), "Project created successfully.");
        }

        private async void btnEdit_Click(object? sender, EventArgs e)
        {
            var project = GetSelectedProject();
            if (project is null)
            {
                return;
            }

            using var editor = new ProjectEditorForm(project);
            if (editor.ShowDialog(this) != DialogResult.OK || editor.ProjectInput is null)
            {
                return;
            }

            await SaveProjectAsync(
                () => _projectApi.UpdateAsync(project.ProjectId, editor.ProjectInput),
                "Project updated successfully.");
        }

        private async Task SaveProjectAsync(Func<Task> save, string successMessage)
        {
            SetBusy(true);
            try
            {
                await save();
                var refreshed = await LoadProjectsAsync();
                var message = refreshed
                    ? successMessage
                    : $"{successMessage}\n\nTry refreshing the project list.";
                MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (HttpRequestException ex)
            {
                ShowRequestError(ex);
            }
            catch (JsonException)
            {
                ShowErrorNotification("The project information could not be read. Please try again.");
            }
            catch (InvalidDataException)
            {
                ShowErrorNotification("No project information was received. Please refresh and try again.");
            }
            catch (TaskCanceledException)
            {
                ShowErrorNotification("The request took too long. Please try again.");
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async void btnDelete_Click(object? sender, EventArgs e)
        {
            var project = GetSelectedProject();
            if (project is null)
            {
                return;
            }

            var confirm = MessageBox.Show(
                $"Are you sure you want to delete project \"{project.ProjectName}\"?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
            {
                return;
            }

            await SaveProjectAsync(
                () => _projectApi.DeleteAsync(project.ProjectId),
                "Project deleted successfully.");
        }

        private void lvProjects_SelectedIndexChanged(object? sender, EventArgs e)
        {
            UpdateSelectionButtons();
        }
    }

    internal sealed class ProjectEditorForm : Form
    {
        private readonly TextBox _txtName = new();
        private readonly TextBox _txtDescription = new();
        private readonly DateTimePicker _dtpStart = new();
        private readonly DateTimePicker _dtpEnd = new();
        private readonly ComboBox _cboStatus = new();
        private readonly NumericUpDown _numBudget = new();

        public ProjectInputDto? ProjectInput { get; private set; }

        public ProjectEditorForm(ProjectDto? project = null)
        {
            Text = project is null ? "Add Project" : "Edit Project";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            ClientSize = new Size(520, 390);
            Font = new Font("Segoe UI", 9F);
            BackColor = Color.FromArgb(245, 247, 250);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(18),
                ColumnCount = 2,
                RowCount = 7
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 125));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            for (var row = 0; row < 6; row++)
            {
                layout.RowStyles.Add(new RowStyle(SizeType.Absolute, row == 1 ? 76 : 42));
            }
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            _txtName.MaxLength = 200;
            _txtDescription.Multiline = true;
            _txtDescription.ScrollBars = ScrollBars.Vertical;
            _dtpStart.Format = DateTimePickerFormat.Short;
            _dtpEnd.Format = DateTimePickerFormat.Short;
            _dtpEnd.ShowCheckBox = true;
            _cboStatus.DropDownStyle = ComboBoxStyle.DropDown;
            _cboStatus.Items.AddRange(new object[] { "Planning", "In Progress", "On Hold", "Completed", "Cancelled" });
            _numBudget.DecimalPlaces = 2;
            _numBudget.Maximum = decimal.MaxValue;
            _numBudget.ThousandsSeparator = true;
            _numBudget.Dock = DockStyle.Left;
            _numBudget.Width = 220;

            AddField(layout, "Project Name:", _txtName, 0);
            AddField(layout, "Description:", _txtDescription, 1);
            AddField(layout, "Start Date:", _dtpStart, 2);
            AddField(layout, "End Date:", _dtpEnd, 3);
            AddField(layout, "Status:", _cboStatus, 4);
            AddField(layout, "Budget:", _numBudget, 5);

            var buttons = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false
            };
            var btnSave = new Button
            {
                Text = "Save",
                Width = 90,
                Height = 32,
                BackColor = Color.FromArgb(25, 135, 84),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            var btnCancel = new Button
            {
                Text = "Cancel",
                Width = 90,
                Height = 32,
                DialogResult = DialogResult.Cancel
            };
            btnSave.Click += Save_Click;
            buttons.Controls.Add(btnSave);
            buttons.Controls.Add(btnCancel);
            layout.Controls.Add(buttons, 0, 6);
            layout.SetColumnSpan(buttons, 2);
            Controls.Add(layout);

            AcceptButton = btnSave;
            CancelButton = btnCancel;

            _dtpStart.Value = project?.StartDate ?? DateTime.Today;
            _dtpEnd.Value = project?.EndDate ?? DateTime.Today;
            _dtpEnd.Checked = project?.EndDate is not null;
            _txtName.Text = project?.ProjectName ?? string.Empty;
            _txtDescription.Text = project?.Description ?? string.Empty;
            _cboStatus.Text = project?.Status ?? "Planning";
            _numBudget.Value = Math.Clamp(project?.Budget ?? 0m, _numBudget.Minimum, _numBudget.Maximum);
        }

        private static void AddField(TableLayoutPanel layout, string labelText, Control control, int row)
        {
            layout.Controls.Add(new Label
            {
                Text = labelText,
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                TextAlign = ContentAlignment.MiddleLeft
            }, 0, row);
            control.Dock = DockStyle.Fill;
            layout.Controls.Add(control, 1, row);
        }

        private void Save_Click(object? sender, EventArgs e)
        {
            var name = _txtName.Text.Trim();
            var status = _cboStatus.Text.Trim();
            var endDate = _dtpEnd.Checked ? _dtpEnd.Value.Date : (DateTime?)null;

            if (string.IsNullOrWhiteSpace(name))
            {
                ShowValidationError("Project name is required.");
                _txtName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(status))
            {
                ShowValidationError("Status is required.");
                _cboStatus.Focus();
                return;
            }

            if (endDate.HasValue && endDate.Value < _dtpStart.Value.Date)
            {
                ShowValidationError("End date cannot be earlier than the start date.");
                _dtpEnd.Focus();
                return;
            }

            ProjectInput = new ProjectInputDto
            {
                ProjectName = name,
                Description = string.IsNullOrWhiteSpace(_txtDescription.Text) ? null : _txtDescription.Text.Trim(),
                StartDate = DateTime.SpecifyKind(_dtpStart.Value.Date, DateTimeKind.Utc),
                EndDate = endDate.HasValue
                    ? DateTime.SpecifyKind(endDate.Value, DateTimeKind.Utc)
                    : null,
                Status = status,
                Budget = _numBudget.Value
            };
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ShowValidationError(string message)
        {
            MessageBox.Show(message, "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}

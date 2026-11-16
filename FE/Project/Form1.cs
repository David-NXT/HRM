using System.Net;
using System.Globalization;
using System.Text.Json;
using Project.DTOs;
using Project.Services;

namespace Project
{
    public partial class Form1 : Form
    {
        private readonly ProjectApiService _projectApi;
        private readonly AttendanceApiService _attendanceApi;
        private List<EmployeeDirectoryDto> _employees = [];
        private List<ProjectDto> _projects = [];
        private List<ProjectMemberDto> _projectMembers = [];
        private List<TaskDto> _tasks = [];
        private bool _suspendSelectionEvents;

        public Form1() : this(Program.ProjectApi, Program.AttendanceApi)
        {
        }

        internal Form1(ProjectApiService projectApi, AttendanceApiService attendanceApi)
        {
            _projectApi = projectApi;
            _attendanceApi = attendanceApi;
            InitializeComponent();

            cmbEmployee.SelectedIndexChanged += async (_, _) =>
            {
                if (_suspendSelectionEvents)
                {
                    return;
                }

                UpdateProjectOptions();
                await LoadTimesheetsAsync();
            };
            cmbProject.SelectedIndexChanged += (_, _) => UpdateTaskOptions();
            dtpWorkDate.ValueChanged += (_, _) => ResetAttendanceStatus();
            txtHours.TextChanged += (_, _) => ResetAttendanceStatus();
            btnSubmit.Click += async (_, _) => await SubmitTimesheetAsync();
            btnRefresh.Click += async (_, _) => await LoadInitialDataAsync();
            Shown += async (_, _) => await LoadInitialDataAsync();
        }

        private async Task LoadInitialDataAsync()
        {
            SetBusy(true, keepHoursEditable: true);
            SetStatus("Đang tải dữ liệu nhân viên, dự án và công việc...", Color.FromArgb(70, 80, 95));

            try
            {
                _employees = await _projectApi.GetEmployeesAsync();
                _projects = await _projectApi.GetAllAsync();
                _projectMembers = await _projectApi.GetProjectMembersAsync();
                _tasks = await _projectApi.GetTasksAsync();

                _suspendSelectionEvents = true;
                cmbEmployee.DataSource = _employees;
                cmbEmployee.DisplayMember = nameof(EmployeeDirectoryDto.DisplayName);
                cmbProject.DataSource = null;
                cmbTask.DataSource = null;
                _suspendSelectionEvents = false;

                if (_employees.Count == 0)
                {
                    SetStatus("Không tìm thấy nhân viên đang hoạt động.", Color.Firebrick);
                    ClearTimesheets();
                    return;
                }

                UpdateProjectOptions();
                await LoadTimesheetsAsync();
                SetStatus("Chọn ngày, dự án, công việc và số giờ để khai báo.", Color.FromArgb(70, 80, 95));
            }
            catch (HttpRequestException ex)
            {
                _suspendSelectionEvents = false;
                ShowServiceError(ex);
            }
            catch (JsonException)
            {
                _suspendSelectionEvents = false;
                ShowError("Dữ liệu từ máy chủ không đúng định dạng. Vui lòng tải lại.");
            }
            catch (InvalidDataException ex)
            {
                _suspendSelectionEvents = false;
                ShowError(ex.Message);
            }
            catch (TaskCanceledException)
            {
                _suspendSelectionEvents = false;
                ShowError("Yêu cầu tải dữ liệu quá thời gian. Vui lòng thử lại.");
            }
            finally
            {
                _suspendSelectionEvents = false;
                SetBusy(false);
            }
        }

        private void UpdateProjectOptions()
        {
            var employee = cmbEmployee.SelectedItem as EmployeeDirectoryDto;
            var memberProjects = employee is null
                ? []
                : _projectMembers
                    .Where(member => member.EmployeeId == employee.EmployeeId
                                     && string.Equals(member.Status, "Active", StringComparison.OrdinalIgnoreCase))
                    .Select(member => member.ProjectId)
                    .ToHashSet();
            var availableProjects = _projects
                .Where(project => memberProjects.Contains(project.ProjectId))
                .ToList();

            _suspendSelectionEvents = true;
            cmbProject.DataSource = null;
            cmbProject.DisplayMember = nameof(ProjectDto.ProjectName);
            cmbProject.DataSource = availableProjects;
            _suspendSelectionEvents = false;

            UpdateTaskOptions();

            if (employee is not null && availableProjects.Count == 0)
            {
                SetStatus("Nhân viên chưa được phân vào dự án đang hoạt động.", Color.FromArgb(180, 110, 0));
            }
        }

        private void UpdateTaskOptions()
        {
            var project = cmbProject.SelectedItem as ProjectDto;
            var availableTasks = project is null
                ? []
                : _tasks.Where(task => task.ProjectId == project.ProjectId).ToList();

            _suspendSelectionEvents = true;
            cmbTask.DataSource = null;
            cmbTask.DisplayMember = nameof(TaskDto.TaskName);
            cmbTask.DataSource = availableTasks;
            _suspendSelectionEvents = false;
        }

        private async Task LoadTimesheetsAsync()
        {
            var employee = cmbEmployee.SelectedItem as EmployeeDirectoryDto;
            if (employee is null)
            {
                ClearTimesheets();
                return;
            }

            try
            {
                var timesheets = await _projectApi.GetTimesheetsAsync(employee.EmployeeId);
                lvTimesheets.BeginUpdate();
                lvTimesheets.Items.Clear();

                foreach (var timesheet in timesheets.OrderByDescending(item => item.WorkDate))
                {
                    var row = new ListViewItem(timesheet.WorkDate.ToString("dd/MM/yyyy"));
                    row.SubItems.Add(timesheet.ProjectName);
                    row.SubItems.Add(timesheet.TaskName);
                    row.SubItems.Add(timesheet.HoursWorked.ToString("0.##"));
                    row.SubItems.Add(GetStatusLabel(timesheet.Status));
                    row.SubItems.Add(timesheet.Description ?? string.Empty);
                    row.SubItems.Add(timesheet.RejectReason ?? string.Empty);
                    lvTimesheets.Items.Add(row);
                }

                lvTimesheets.EndUpdate();
                lblSummary.Text = $"Tổng số khai báo: {timesheets.Count}";
            }
            catch (HttpRequestException ex)
            {
                ShowServiceError(ex);
            }
            catch (JsonException)
            {
                ShowError("Không đọc được danh sách timesheet. Vui lòng tải lại.");
            }
            catch (InvalidDataException ex)
            {
                ShowError(ex.Message);
            }
            catch (TaskCanceledException)
            {
                ShowError("Yêu cầu tải timesheet quá thời gian. Vui lòng thử lại.");
            }
        }

        private async Task SubmitTimesheetAsync()
        {
            var employee = cmbEmployee.SelectedItem as EmployeeDirectoryDto;
            var project = cmbProject.SelectedItem as ProjectDto;
            var task = cmbTask.SelectedItem as TaskDto;
            if (employee is null || project is null || task is null)
            {
                ShowError("Vui lòng chọn nhân viên, dự án và công việc hợp lệ.");
                return;
            }

            var member = _projectMembers.FirstOrDefault(item =>
                item.EmployeeId == employee.EmployeeId
                && item.ProjectId == project.ProjectId
                && string.Equals(item.Status, "Active", StringComparison.OrdinalIgnoreCase));
            if (member is null)
            {
                ShowError("Nhân viên không có phân công hợp lệ trong dự án đã chọn.");
                return;
            }

            if (!TryParseHours(txtHours.Text, out var hoursWorked))
            {
                ShowError("Số giờ phải là số lớn hơn 0 và không vượt quá 24. Có thể dùng dấu chấm hoặc dấu phẩy thập phân.");
                txtHours.Focus();
                txtHours.SelectAll();
                return;
            }

            var workDate = DateOnly.FromDateTime(dtpWorkDate.Value);

            SetBusy(true);
            lblAttendanceHours.Text = "Đang kiểm tra...";
            SetStatus("Đang đối chiếu dữ liệu chấm công...", Color.FromArgb(70, 80, 95));
            try
            {
                var attendance = await _attendanceApi.GetWorkHoursAsync(employee.EmployeeId, workDate);
                if (attendance is null || attendance.RecordCount == 0)
                {
                    lblAttendanceHours.Text = "Không có dữ liệu";
                    SetStatus("Từ chối gửi: không tìm thấy dữ liệu chấm công ngày này.", Color.Firebrick);
                    return;
                }

                if (attendance.EmployeeId != employee.EmployeeId || attendance.Date != workDate)
                {
                    throw new InvalidDataException("Dữ liệu chấm công trả về không khớp nhân viên hoặc ngày.");
                }

                lblAttendanceHours.Text = $"{attendance.TotalWorkHours:0.##} giờ";
                if (hoursWorked != attendance.TotalWorkHours)
                {
                    SetStatus(
                        $"Từ chối gửi: khai báo {hoursWorked:0.##} giờ nhưng attendance ghi nhận {attendance.TotalWorkHours:0.##} giờ.",
                        Color.Firebrick);
                    MessageBox.Show(
                        $"Số giờ khai báo ({hoursWorked:0.##}) không khớp với attendance ({attendance.TotalWorkHours:0.##}).",
                        "Không thể gửi timesheet",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                await _projectApi.CreateTimesheetAsync(new CreateTimesheetDto
                {
                    ProjectMemberId = member.ProjectMemberId,
                    TaskId = task.TaskId,
                    WorkDate = DateTime.SpecifyKind(workDate.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc),
                    HoursWorked = hoursWorked,
                    Description = string.IsNullOrWhiteSpace(txtDescription.Text)
                        ? null
                        : txtDescription.Text.Trim()
                });

                SetStatus("Timesheet đã được gửi thành công sau khi khớp với attendance.", Color.DarkGreen);
                MessageBox.Show(
                    "Timesheet đã được gửi thành công.",
                    "Thành công",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                txtDescription.Clear();
                await LoadTimesheetsAsync();
            }
            catch (HttpRequestException ex)
            {
                ShowServiceError(ex);
            }
            catch (JsonException)
            {
                ShowError("Dữ liệu phản hồi không đúng định dạng. Vui lòng tải lại.");
            }
            catch (InvalidDataException ex)
            {
                ShowError(ex.Message);
            }
            catch (TaskCanceledException)
            {
                ShowError("Yêu cầu gửi timesheet quá thời gian. Vui lòng thử lại.");
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void ResetAttendanceStatus()
        {
            lblAttendanceHours.Text = "Chưa kiểm tra";
        }

        private static bool TryParseHours(string input, out decimal hours)
        {
            var normalized = input.Trim().Replace(',', '.');
            return decimal.TryParse(
                       normalized,
                       NumberStyles.AllowDecimalPoint,
                       CultureInfo.InvariantCulture,
                       out hours)
                   && hours > 0
                   && hours <= 24;
        }

        private void ClearTimesheets()
        {
            lvTimesheets.Items.Clear();
            lblSummary.Text = "Tổng số khai báo: 0";
        }

        private void SetBusy(bool busy, bool keepHoursEditable = false)
        {
            cmbEmployee.Enabled = !busy;
            cmbProject.Enabled = !busy;
            cmbTask.Enabled = !busy;
            dtpWorkDate.Enabled = !busy;
            txtHours.Enabled = !busy || keepHoursEditable;
            txtDescription.Enabled = !busy;
            btnSubmit.Enabled = !busy;
            btnRefresh.Enabled = !busy;
            UseWaitCursor = busy;
        }

        private void SetStatus(string message, Color color)
        {
            lblStatus.Text = message;
            lblStatus.ForeColor = color;
        }

        private static string GetStatusLabel(string status) => status switch
        {
            "Pending" => "Chờ duyệt",
            "Approved" => "Đã duyệt",
            "Rejected" => "Từ chối",
            _ => status
        };

        private void ShowServiceError(HttpRequestException exception)
        {
            var message = exception.StatusCode switch
            {
                HttpStatusCode.BadRequest => $"Dữ liệu khai báo không hợp lệ.\n{exception.Message}",
                HttpStatusCode.Conflict => $"Timesheet bị từ chối do không hợp lệ hoặc không khớp attendance.\n{exception.Message}",
                HttpStatusCode.NotFound => $"Không tìm thấy dữ liệu cần thiết.\n{exception.Message}",
                HttpStatusCode.ServiceUnavailable => "Dịch vụ attendance hoặc timesheet hiện không khả dụng. Timesheet chưa được gửi.",
                null => "Không thể kết nối máy chủ. Vui lòng kiểm tra kết nối và thử lại.",
                _ => $"Không thể hoàn tất yêu cầu.\n{exception.Message}"
            };
            ShowError(message);
        }

        private void ShowError(string message)
        {
            SetStatus(message, Color.Firebrick);
            MessageBox.Show(message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}

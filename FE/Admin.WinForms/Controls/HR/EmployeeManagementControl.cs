using Admin.WinForms.Forms.HR;
using Admin.WinForms.Models.HR;

namespace Admin.WinForms.Controls.HR;

public sealed partial class EmployeeManagementControl : HrPageControl
{
    protected override Label PageStatus => Status;

    private int _page = 1;
    private string _keyword = "";

    public EmployeeManagementControl()
    {
        InitializeComponent();
        WireEvents();
    }

    private void WireEvents()
    {
        _searchButton.Click += async (_, _) => await RunAsync(async () =>
        {
            _page = 1;
            _keyword = _search.Text.Trim();
            await ReloadAsync();
        });

        _refreshButton.Click += async (_, _) => await RunAsync(async () =>
        {
            _search.Clear();
            _keyword = "";
            _page = 1;
            await ReloadAsync();
        });

        _addButton.Click += async (_, _) => await RunAsync(() => EditAsync(null));

        _editButton.Click += async (_, _) => await RunAsync(async () =>
        {
            var selected = Selected<EmployeeRow>(_grid);
            var employee = await Api.GetAsync<EmployeeRow>($"employees/{selected.EmployeeId}", Token);
            await EditAsync(employee);
        });

        _lockButton.Click += async (_, _) => await RunAsync(LockAsync);
        _deleteButton.Click += async (_, _) => await RunAsync(DeleteAsync);
        _detailsButton.Click += async (_, _) => await RunAsync(DetailsAsync);
        _salaryButton.Click += async (_, _) => await RunAsync(UpdateSalaryAsync);
        _exportButton.Click += async (_, _) => await RunAsync(ExportAsync);

        _previous.Click += async (_, _) => await RunAsync(async () =>
        {
            _page--;
            await ReloadAsync();
        });

        _next.Click += async (_, _) => await RunAsync(async () =>
        {
            _page++;
            await ReloadAsync();
        });

        _search.KeyDown += async (_, e) =>
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;
            await RunAsync(async () =>
            {
                _page = 1;
                _keyword = _search.Text.Trim();
                await ReloadAsync();
            });
        };

        _grid.CellDoubleClick += async (_, e) =>
        {
            if (e.RowIndex >= 0) await RunAsync(DetailsAsync);
        };
    }

    protected override async Task ReloadAsync()
    {
        var result = await Api.EmployeesAsync(_keyword, Math.Max(1, _page), 20, Token);
        if (result.Items.Count == 0 && _page > 1)
        {
            _page = Math.Max(1, (int)Math.Ceiling(result.Total / 20d));
            result = await Api.EmployeesAsync(_keyword, _page, 20, Token);
        }

        Token.ThrowIfCancellationRequested();
        _page = result.Page;
        _grid.DataSource = result.Items;
        _previous.Enabled = _page > 1;
        _next.Enabled = (long)_page * 20 < result.Total;
        _summary.Text = $"Trang {_page}/{Math.Max(1, (int)Math.Ceiling(result.Total / 20d))} · Tổng {result.Total} nhân viên";
        Status.Text = result.Total == 0
            ? "Không tìm thấy nhân viên phù hợp."
            : "Chọn một dòng để sửa, xem, khóa hoặc xóa hồ sơ.";
    }

    private async Task ExportAsync()
    {
        Ensure(_search.Text.Trim() == _keyword,
            "Bạn đã đổi từ khóa. Hãy bấm Tìm kiếm trước để xuất đúng kết quả đang xem.");
        using var save = new SaveFileDialog
        {
            Title = "Xuất toàn bộ danh sách nhân viên theo kết quả tìm kiếm",
            Filter = "Excel Workbook (*.xlsx)|*.xlsx",
            DefaultExt = "xlsx",
            AddExtension = true,
            OverwritePrompt = true,
            FileName = $"DanhSachNhanVien_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
        };
        if (save.ShowDialog(this) != DialogResult.OK)
        {
            Status.Text = "Đã hủy xuất Excel.";
            return;
        }

        var bytes = await Api.DownloadAsync($"employees/export?keyword={Uri.EscapeDataString(_keyword)}", Token);
        await File.WriteAllBytesAsync(save.FileName, bytes, Token);
        Status.Text = $"Đã xuất Excel toàn bộ kết quả tìm kiếm: {save.FileName}";
    }

    private async Task EditAsync(EmployeeRow? employee)
    {
        var creating = employee is null;
        using var form = new HrEditorForm(creating ? "Tiếp nhận nhân viên" : "Sửa hồ sơ nhân viên");
        if (creating) form.TextField("code", "Mã nhân viên *", required: true, maxLength: 20);
        form.TextField("name", "Họ tên *", employee?.FullName, true, 150)
            .DateField("birth", "Ngày sinh *", employee?.DateOfBirth ?? new DateOnly(2000, 1, 1))
            .ChoiceField("gender", "Giới tính", new[] { "Male", "Female", "Other" }.Select(x => new Choice(x, x)), employee?.Gender ?? "Other")
            .TextField("email", "Email *", employee?.Email, true, 200)
            .TextField("phone", "Điện thoại *", employee?.Phone, true, 20)
            .TextField("citizen", "CCCD/CMND *", employee?.CitizenId, true, 12)
            .TextField("address", "Địa chỉ", employee?.Address, maxLength: 500);

        if (creating)
        {
            form.AmountField("baseSalary", "Lương cơ bản full-time (VND/tháng)")
                .AmountField("partTimeSalary", "Lương part-time (VND/giờ)");
            var departments = await Api.GetAsync<List<DepartmentRow>>("departments", Token);
            var positions = await Api.GetAsync<List<PositionRow>>("positions", Token);
            Token.ThrowIfCancellationRequested();
            Ensure(departments.Any(x => x.Status == "ACTIVE") && positions.Any(x => x.Status == "ACTIVE"),
                "Hãy tạo phòng ban và chức vụ đang hoạt động trong Organization trước khi tiếp nhận nhân viên.");
            form.ChoiceField("type", "Loại nhân viên", new[] { new Choice("FULL_TIME", "Full-time"), new Choice("PART_TIME", "Part-time") })
                .ChoiceField("department", "Phòng ban *", departments.Where(x => x.Status == "ACTIVE").Select(x => new Choice(x.DepartmentId, x.DisplayName)))
                .ChoiceField("position", "Chức vụ *", HrChoiceOptions.PositionChoices(positions, "FULL_TIME"))
                .OnChoiceChanged("type", value => form.SetChoices("position",
                    HrChoiceOptions.PositionChoices(positions, value as string, defaultStaffOnlyForPartTime: true)))
                .DateField("start", "Ngày bắt đầu *", DateOnly.FromDateTime(DateTime.Today))
                .TextField("notes", "Ghi chú công tác", maxLength: 1000, multiline: true);
        }

        form.OnSave(async (f, token) =>
        {
            var personal = new PersonalInfo(f.TextValue("name"), f.DateValue("birth"), (string)f.ChoiceValue("gender")!,
                f.TextValue("email"), f.TextValue("phone"), f.TextValue("citizen"), f.OptionalText("address"));
            if (creating)
            {
                var request = new CreateEmployee(f.TextValue("code"), personal.FullName, personal.DateOfBirth,
                    personal.Gender, personal.Email, personal.Phone, personal.CitizenId, personal.Address,
                    (string)f.ChoiceValue("type")!,
                    Equals(f.ChoiceValue("type"), "FULL_TIME") ? f.AmountValue("baseSalary") : null,
                    Equals(f.ChoiceValue("type"), "PART_TIME") ? f.AmountValue("partTimeSalary") : null);
                await Api.SendAsync<OnboardingResult>(HttpMethod.Post, "onboarding",
                    new OnboardEmployee(request, f.DateValue("start"), f.GuidValue("department"), f.GuidValue("position"), f.OptionalText("notes")), token);
            }
            else
            {
                await Api.SendAsync<EmployeeRow>(HttpMethod.Put, $"employees/{employee!.EmployeeId}", personal, token);
            }
        });

        if (form.ShowDialog(this) == DialogResult.OK) await ReloadAsync();
    }

    private async Task LockAsync()
    {
        var employee = Selected<EmployeeRow>(_grid);
        Ensure(employee.Status == "ACTIVE", "Chỉ khóa hồ sơ đang ACTIVE; nghỉ việc dùng quy trình bàn giao.");
        if (!Confirm($"Khóa hồ sơ {employee.EmployeeCode} — {employee.FullName}?"))
        {
            Status.Text = "Đã hủy thao tác.";
            return;
        }

        await Api.SendAsync<EmployeeRow>(HttpMethod.Post, $"employees/{employee.EmployeeId}/lock", null, Token);
        await ReloadAsync();
    }

    private async Task DeleteAsync()
    {
        var employee = Selected<EmployeeRow>(_grid);
        if (MessageBox.Show(this,
            $"Xóa vĩnh viễn nhân viên {employee.EmployeeCode} — {employee.FullName}?\n\n" +
            "Thao tác này không thể hoàn tác. Chỉ xóa được hồ sơ chưa có dữ liệu liên quan.",
            "Xác nhận xóa nhân viên", MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2) != DialogResult.Yes)
        {
            Status.Text = "Đã hủy thao tác xóa.";
            return;
        }

        await Api.DeleteEmployeeAsync(employee.EmployeeId, Token);
        await ReloadAsync();
        Status.Text = $"Đã xóa nhân viên {employee.EmployeeCode} — {employee.FullName}.";
    }

    private async Task UpdateSalaryAsync()
    {
        var employee = await Api.GetAsync<EmployeeRow>($"employees/{Selected<EmployeeRow>(_grid).EmployeeId}", Token);
        Token.ThrowIfCancellationRequested();
        var isPartTime = employee.EmploymentType == "PART_TIME";
        using var form = new HrEditorForm($"Cập nhật lương · {employee.DisplayName}");
        form.AmountField("salary", isPartTime ? "Đơn giá part-time (VND/giờ)" : "Lương cơ bản full-time (VND/tháng)",
                isPartTime ? employee.PartTimeHourlyRate.GetValueOrDefault() : employee.BaseSalary.GetValueOrDefault())
            .OnSave(async (f, token) => await Api.SendAsync<EmployeeRow>(HttpMethod.Put,
                $"employees/{employee.EmployeeId}/salary",
                new SalaryInput(isPartTime ? null : f.AmountValue("salary"), isPartTime ? f.AmountValue("salary") : null), token));
        if (form.ShowDialog(this) == DialogResult.OK) await ReloadAsync();
    }

    private async Task DetailsAsync()
    {
        var employee = await Api.GetAsync<EmployeeRow>($"employees/{Selected<EmployeeRow>(_grid).EmployeeId}", Token);
        Token.ThrowIfCancellationRequested();
        using var form = new EmployeeDetailsForm(employee);
        form.ShowDialog(this);
        Status.Text = "Đã xem hồ sơ.";
    }
}

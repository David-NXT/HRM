using Admin.WinForms.Forms.HR;
using Admin.WinForms.Models.HR;

namespace Admin.WinForms.Controls.HR;

public sealed class EmployeeManagementControl : HrPageControl
{
    private readonly TextBox _search = new() { Width = 300, PlaceholderText = "Mã, tên, email, điện thoại, CCCD", Margin = new Padding(3, 8, 8, 3) };
    private readonly DataGridView _grid = Table(("Mã NV", "EmployeeCode"), ("Họ tên", "FullName"),
        ("Loại", "EmploymentTypeLabel"), ("Phòng ban", "DepartmentName"), ("Chức vụ", "PositionName"),
        ("Email", "Email"), ("Trạng thái", "Status"));
    private readonly Label _summary = new() { AutoSize = true, Margin = new Padding(12, 12, 8, 3) };
    private readonly Button _previous = new() { Text = "Trang trước", AutoSize = true, Height = 34 };
    private readonly Button _next = new() { Text = "Trang sau", AutoSize = true, Height = 34 };
    private int _page = 1;
    private string _keyword = "";

    public EmployeeManagementControl()
    {
        Toolbar.Controls.Add(_search);
        _grid.Columns.Insert(3, new DataGridViewTextBoxColumn
        {
            HeaderText = "Lương cơ bản",
            DataPropertyName = "BaseSalaryLabel",
            Name = "BaseSalaryLabel",
            MinimumWidth = 120,
            SortMode = DataGridViewColumnSortMode.NotSortable
        });
        ActionButton("Tìm kiếm", async () => { _page = 1; _keyword = _search.Text.Trim(); await ReloadAsync(); }, true);
        ActionButton("Làm mới", async () => { _search.Clear(); _keyword = ""; _page = 1; await ReloadAsync(); });
        ActionButton("Thêm / Tiếp nhận", () => EditAsync(null));
        ActionButton("Sửa hồ sơ", async () => await EditAsync(await Api.GetAsync<EmployeeRow>($"employees/{Selected<EmployeeRow>(_grid).EmployeeId}", Token)));
        ActionButton("Khóa hồ sơ", LockAsync);
        ActionButton("Xem chi tiết", DetailsAsync);
        ActionButton("Cập nhật lương", UpdateSalaryAsync);
        Body.Controls.Add(_grid);
        Footer.Controls.Add(_previous);
        Footer.Controls.Add(_summary);
        Footer.Controls.Add(_next);
        _previous.Click += async (_, _) => await RunAsync(async () => { _page--; await ReloadAsync(); });
        _next.Click += async (_, _) => await RunAsync(async () => { _page++; await ReloadAsync(); });
        _search.KeyDown += async (_, e) =>
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;
            await RunAsync(async () => { _page = 1; _keyword = _search.Text.Trim(); await ReloadAsync(); });
        };
        _grid.CellDoubleClick += async (_, e) => { if (e.RowIndex >= 0) await RunAsync(DetailsAsync); };
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
        Status.Text = result.Total == 0 ? "Không tìm thấy nhân viên phù hợp." : "Chọn một dòng để sửa, xem hoặc khóa hồ sơ.";
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
            form.AmountField("salary", "Lương cơ bản (VND/tháng)");
            var departments = await Api.GetAsync<List<DepartmentRow>>("departments", Token);
            var positions = await Api.GetAsync<List<PositionRow>>("positions", Token);
            Token.ThrowIfCancellationRequested();
            Ensure(departments.Any(x => x.Status == "ACTIVE") && positions.Any(x => x.Status == "ACTIVE"),
                "Hãy tạo phòng ban và chức vụ đang hoạt động trong Organization trước khi tiếp nhận nhân viên.");
            form.ChoiceField("type", "Loại nhân viên", new[] { new Choice("FULL_TIME", "Full-time"), new Choice("PART_TIME", "Part-time") })
                .ChoiceField("department", "Phòng ban *", departments.Where(x => x.Status == "ACTIVE").Select(x => new Choice(x.DepartmentId, x.DisplayName)))
                .ChoiceField("position", "Chức vụ *", positions.Where(x => x.Status == "ACTIVE").Select(x => new Choice(x.PositionId, x.DisplayName)))
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
                    (string)f.ChoiceValue("type")!, f.AmountValue("salary"));
                await Api.SendAsync<OnboardingResult>(HttpMethod.Post, "onboarding",
                    new OnboardEmployee(request, f.DateValue("start"), f.GuidValue("department"), f.GuidValue("position"), f.OptionalText("notes")), token);
            }
            else await Api.SendAsync<EmployeeRow>(HttpMethod.Put, $"employees/{employee!.EmployeeId}", personal, token);
        });
        if (form.ShowDialog(this) == DialogResult.OK) await ReloadAsync();
    }

    private async Task LockAsync()
    {
        var employee = Selected<EmployeeRow>(_grid);
        Ensure(employee.Status == "ACTIVE", "Chỉ khóa hồ sơ đang ACTIVE; nghỉ việc dùng quy trình bàn giao.");
        if (!Confirm($"Khóa hồ sơ {employee.EmployeeCode} — {employee.FullName}?")) { Status.Text = "Đã hủy thao tác."; return; }
        await Api.SendAsync<EmployeeRow>(HttpMethod.Post, $"employees/{employee.EmployeeId}/lock", null, Token);
        await ReloadAsync();
    }

    private async Task UpdateSalaryAsync()
    {
        var employee = await Api.GetAsync<EmployeeRow>($"employees/{Selected<EmployeeRow>(_grid).EmployeeId}", Token);
        Token.ThrowIfCancellationRequested();
        using var form = new HrEditorForm($"Lương cơ bản · {employee.DisplayName}");
        form.AmountField("salary", "Lương cơ bản (VND/tháng)", employee.BaseSalary)
            .OnSave(async (f, token) => await Api.SendAsync<EmployeeRow>(HttpMethod.Put,
                $"employees/{employee.EmployeeId}/salary", new SalaryInput(f.AmountValue("salary")), token));
        if (form.ShowDialog(this) == DialogResult.OK) await ReloadAsync();
    }

    private async Task DetailsAsync()
    {
        var employee = await Api.GetAsync<EmployeeRow>($"employees/{Selected<EmployeeRow>(_grid).EmployeeId}", Token);
        Token.ThrowIfCancellationRequested();
        using var form = new HrEditorForm(employee.DisplayName);
        form.TextField("profile", "Thông tin hồ sơ", string.Join(Environment.NewLine,
            $"GUID: {employee.EmployeeId}", $"Loại: {employee.EmploymentTypeLabel}",
            $"Phòng ban: {employee.DepartmentName ?? "Chưa phân công"}", $"Chức vụ: {employee.PositionName ?? "Chưa phân công"}",
            $"Trạng thái: {employee.Status}", $"Ngày sinh: {employee.DateOfBirth:dd/MM/yyyy}", $"Giới tính: {employee.Gender}",
            $"Email: {employee.Email}", $"Điện thoại: {employee.Phone}", $"CCCD: {employee.CitizenId}", $"Địa chỉ: {employee.Address}"),
            maxLength: 5000, multiline: true, readOnly: true).ReadOnlyView();
        form.ShowDialog(this);
        Status.Text = "Đã xem hồ sơ.";
    }
}

using Admin.WinForms.Forms.HR;
using Admin.WinForms.Models.HR;

namespace Admin.WinForms.Controls.HR;

public sealed class ContractWorkControl : HrPageControl
{
    private readonly ComboBox _employees = Choices([]);
    private readonly DataGridView _contracts = Table(("Số hợp đồng", "ContractNumber"), ("Loại", "ContractType"),
        ("Bắt đầu", "StartDate"), ("Kết thúc", "EndDate"), ("Trạng thái", "Status"), ("Ghi chú", "Notes"));
    private readonly DataGridView _history = Table(("Phòng ban", "DepartmentName"), ("Chức vụ", "PositionName"),
        ("Từ ngày", "StartDate"), ("Đến ngày", "EndDate"), ("Ghi chú", "Notes"));
    private List<ContractRow> _contractRows = [];

    public ContractWorkControl()
    {
        Toolbar.Controls.Add(_employees);
        ActionButton("Làm mới", ReloadAsync);
        ActionButton("Thêm hợp đồng", () => EditContractAsync(false, false), true);
        ActionButton("Sửa hợp đồng", () => EditContractAsync(true, false));
        ActionButton("Gia hạn", () => EditContractAsync(false, true));
        ActionButton("Phân công / Điều chuyển", AssignmentAsync);
        var tabs = new TabControl { Dock = DockStyle.Fill };
        var contracts = new TabPage("Hợp đồng"); contracts.Controls.Add(_contracts);
        var history = new TabPage("Lịch sử công tác"); history.Controls.Add(_history);
        tabs.TabPages.AddRange([contracts, history]);
        Body.Controls.Add(tabs);
        _employees.SelectionChangeCommitted += async (_, _) => await RunAsync(LoadDetailsAsync);
    }

    private Guid EmployeeId => (_employees.SelectedItem as Choice)?.Value is Guid id ? id
        : throw new InvalidOperationException("Hãy chọn nhân viên.");

    protected override async Task ReloadAsync()
    {
        var oldId = (_employees.SelectedItem as Choice)?.Value;
        var employees = await Api.AllEmployeesAsync(Token);
        Token.ThrowIfCancellationRequested();
        _employees.Items.Clear();
        foreach (var row in employees) _employees.Items.Add(new Choice(row.EmployeeId, row.DisplayName));
        _employees.SelectedItem = _employees.Items.Cast<Choice>().FirstOrDefault(x => Equals(x.Value, oldId))
            ?? _employees.Items.Cast<Choice>().FirstOrDefault();
        if (_employees.Items.Count > 0) await LoadDetailsAsync();
        else { _contracts.DataSource = null; _history.DataSource = null; Status.Text = "Chưa có nhân viên."; }
    }

    private async Task LoadDetailsAsync()
    {
        var id = EmployeeId;
        var contracts = Api.GetAsync<List<ContractRow>>($"employees/{id}/contracts", Token);
        var history = Api.GetAsync<List<AssignmentRow>>($"employees/{id}/work-history", Token);
        await Task.WhenAll(contracts, history);
        Token.ThrowIfCancellationRequested();
        _contractRows = await contracts;
        _contracts.DataSource = _contractRows;
        _history.DataSource = await history;
        Status.Text = $"{_contractRows.Count} hợp đồng · {(await history).Count} giai đoạn công tác. Ngày tương lai chưa thay đổi phòng ban hiện tại.";
    }

    private async Task EditContractAsync(bool editing, bool renewing)
    {
        var id = EmployeeId;
        var selected = editing || renewing ? Selected<ContractRow>(_contracts) : null;
        if (selected is not null) Ensure(selected.Status == "ACTIVE", "Chỉ xử lý hợp đồng đang ACTIVE.");
        var start = renewing ? selected!.EndDate?.AddDays(1) ?? DateOnly.FromDateTime(DateTime.Today) : selected?.StartDate;
        using var form = new HrEditorForm(renewing ? "Gia hạn hợp đồng" : editing ? "Sửa hợp đồng" : "Thêm hợp đồng");
        form.TextField("number", "Số hợp đồng *", renewing ? null : selected?.ContractNumber, true, 50)
            .TextField("type", "Loại hợp đồng *", selected?.ContractType, true, 50)
            .DateField("start", "Ngày bắt đầu *", start)
            .DateField("end", "Ngày kết thúc (tùy chọn)", renewing ? null : selected?.EndDate, true)
            .TextField("notes", "Ghi chú", editing ? selected?.Notes : null, maxLength: 1000, multiline: true)
            .OnSave(async (f, token) =>
            {
                var end = f.OptionalDate("end");
                var begin = f.DateValue("start");
                Ensure(end is null || end > begin, "Ngày kết thúc phải sau ngày bắt đầu.");
                var previous = renewing ? selected : _contractRows.FirstOrDefault(x => x.ContractId == selected?.RenewalOfContractId);
                if (previous is not null)
                    Ensure(begin > (previous.EndDate ?? previous.StartDate), "Hợp đồng gia hạn phải bắt đầu sau hợp đồng trước.");
                await Api.SendAsync<ContractRow>(editing ? HttpMethod.Put : HttpMethod.Post,
                    editing ? $"contracts/{selected!.ContractId}" : $"employees/{id}/contracts",
                    new ContractInput(f.TextValue("number"), f.TextValue("type"), begin, end,
                        f.OptionalText("notes"), renewing ? selected!.ContractId : null), token);
            });
        if (form.ShowDialog(this) == DialogResult.OK) await LoadDetailsAsync();
    }

    private async Task AssignmentAsync()
    {
        var id = EmployeeId;
        var employee = await Api.GetAsync<EmployeeRow>($"employees/{id}", Token);
        var departments = await Api.GetAsync<List<DepartmentRow>>("departments", Token);
        var positions = await Api.GetAsync<List<PositionRow>>("positions", Token);
        Token.ThrowIfCancellationRequested();
        using var form = new HrEditorForm("Phân công / Điều chuyển");
        form.ChoiceField("department", "Phòng ban *", new[] { new Choice(null, "Chọn phòng ban *") }
                .Concat(departments.Where(x => x.Status == "ACTIVE").Select(x => new Choice(x.DepartmentId, x.DisplayName))))
            .ChoiceField("position", "Chức vụ *", HrChoiceOptions.PositionChoices(positions, employee.EmploymentType))
            .DateField("effective", "Ngày hiệu lực *")
            .TextField("notes", "Ghi chú", maxLength: 1000, multiline: true)
            .OnSave(async (f, token) => await Api.SendAsync<AssignmentRow>(HttpMethod.Post, $"employees/{id}/work-history",
                new AssignmentInput(f.GuidValue("department"), f.GuidValue("position"), f.DateValue("effective"), f.OptionalText("notes")), token));
        if (form.ShowDialog(this) == DialogResult.OK) await LoadDetailsAsync();
    }
}

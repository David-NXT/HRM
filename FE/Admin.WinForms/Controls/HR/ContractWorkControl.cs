using Admin.WinForms.Forms.HR;
using Admin.WinForms.Models.HR;

namespace Admin.WinForms.Controls.HR;

public sealed partial class ContractWorkControl : HrPageControl
{
    protected override Label PageStatus => Status;

    private List<ContractRow> _contractRows = [];
    private int _historyCount;
    private bool _contractsLoaded;

    public ContractWorkControl()
    {
        InitializeComponent();
        _contractFilter.SelectedIndex = 0;
        WireEvents();
    }

    private void WireEvents()
    {
        _refreshButton.Click += async (_, _) => await RunAsync(ReloadAsync);
        _addContractButton.Click += async (_, _) => await RunAsync(() => EditContractAsync(false, false));
        _editContractButton.Click += async (_, _) => await RunAsync(() => EditContractAsync(true, false));
        _renewButton.Click += async (_, _) => await RunAsync(() => EditContractAsync(false, true));
        _assignmentButton.Click += async (_, _) => await RunAsync(AssignmentAsync);
        _employees.SelectionChangeCommitted += async (_, _) => await RunAsync(LoadDetailsAsync);
        _contractFilter.SelectionChangeCommitted += (_, _) => ApplyContractFilter();
        _contracts.CellFormatting += FormatDateCell;
        _contracts.CellFormatting += FormatContractPeriod;
        _history.CellFormatting += FormatDateCell;
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
        else
        {
            _contractRows = [];
            _historyCount = 0;
            _contractsLoaded = false;
            _contracts.DataSource = null;
            _history.DataSource = null;
            _contractSummary.Text = "Chưa có nhân viên để xem hợp đồng.";
            _contractSummary.ForeColor = Color.FromArgb(80, 96, 115);
            _contractCountLabel.Text = "";
            Status.Text = "Chưa có nhân viên.";
        }
    }

    private async Task LoadDetailsAsync()
    {
        var id = EmployeeId;
        // Clear the previous employee's data before requesting a different employee.
        _contractRows = [];
        _historyCount = 0;
        _contractsLoaded = false;
        _contracts.DataSource = null;
        _history.DataSource = null;
        _contractSummary.Text = "Chưa có dữ liệu hợp đồng của nhân viên đã chọn.";
        _contractSummary.ForeColor = Color.FromArgb(80, 96, 115);
        _contractCountLabel.Text = "";
        var contracts = Api.GetAsync<List<ContractRow>>($"employees/{id}/contracts", Token);
        var history = Api.GetAsync<List<AssignmentRow>>($"employees/{id}/work-history", Token);
        await Task.WhenAll(contracts, history);
        Token.ThrowIfCancellationRequested();
        _contractRows = await contracts;
        var historyRows = await history;
        _historyCount = historyRows.Count;
        _history.DataSource = historyRows;
        _contractsLoaded = true;
        ApplyContractFilter();
    }

    private void ApplyContractFilter()
    {
        if (!_contractsLoaded || _employees.SelectedItem is not Choice { Value: Guid }) return;
        // Match the reference date used by HR's contract alert job.
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var allRows = _contractRows.Select(contract => new ContractDisplayRow(contract, today)).ToArray();
        var filter = _contractFilter.SelectedIndex switch
        {
            1 => ContractPeriodFilter.InEffect,
            2 => ContractPeriodFilter.ExpiringSoon,
            3 => ContractPeriodFilter.Expired,
            4 => ContractPeriodFilter.NotStarted,
            _ => ContractPeriodFilter.All
        };
        var visible = allRows.Where(contract => contract.Matches(filter)).ToList();
        _contracts.DataSource = visible;

        // The employee overview always uses all contracts, regardless of the table filter.
        var coverage = ContractPresentation.Summarize(allRows);
        _contractSummary.Text = coverage.Text;
        _contractSummary.ForeColor = coverage.HasEffectiveContract
            ? Color.FromArgb(22, 101, 52) : Color.FromArgb(180, 83, 9);
        _contractCountLabel.Text = $"Hiển thị {visible.Count}/{allRows.Length} hợp đồng · {_historyCount} giai đoạn công tác";
        Status.Text = $"Ngày đối chiếu: {today:dd/MM/yyyy} (UTC). "
            + (visible.Count == 0 && allRows.Length > 0
                ? "Không có hợp đồng khớp bộ lọc; chọn Tất cả để xem các hợp đồng khác."
                : "Bộ lọc chỉ áp dụng cho bảng hợp đồng. Bấm Làm mới để cập nhật dữ liệu và ngày đối chiếu.");
    }

    private async Task EditContractAsync(bool editing, bool renewing)
    {
        var id = EmployeeId;
        var selected = editing || renewing ? Selected<ContractDisplayRow>(_contracts).Contract : null;
        if (selected is not null) Ensure(selected.Status == "ACTIVE",
            "Chỉ sửa hoặc gia hạn hợp đồng có trạng thái quản lý ACTIVE (chưa được gia hạn).");
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
            .OnSave(async (f, token) => await Api.SendAsync<AssignmentRow>(HttpMethod.Post,
                $"employees/{id}/work-history",
                new AssignmentInput(f.GuidValue("department"), f.GuidValue("position"), f.DateValue("effective"), f.OptionalText("notes")), token));
        if (form.ShowDialog(this) == DialogResult.OK) await LoadDetailsAsync();
    }

    private void FormatContractPeriod(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex < 0
            || _contracts.Columns[e.ColumnIndex] != _contractDateStatusColumn
            || _contracts.Rows[e.RowIndex].DataBoundItem is not ContractDisplayRow contract) return;
        e.CellStyle.ForeColor = contract.PeriodState switch
        {
            ContractPeriodState.Expired => Color.FromArgb(185, 28, 28),
            ContractPeriodState.ExpiringSoon => Color.FromArgb(180, 83, 9),
            ContractPeriodState.InEffect => Color.FromArgb(22, 101, 52),
            _ => Color.FromArgb(80, 96, 115)
        };
    }

    private static void FormatDateCell(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.Value is DateOnly date)
        {
            e.Value = date.ToString("dd/MM/yyyy");
            e.FormattingApplied = true;
        }
        else if (e.Value is DateTime time)
        {
            e.Value = time.ToLocalTime().ToString("dd/MM/yyyy HH:mm");
            e.FormattingApplied = true;
        }
    }
}

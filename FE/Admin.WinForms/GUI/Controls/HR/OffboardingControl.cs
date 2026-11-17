using Admin.WinForms.GUI.Forms.HR;
using Admin.WinForms.BUS.Models.HR;

namespace Admin.WinForms.GUI.Controls.HR;

public sealed partial class OffboardingControl : HrPageControl
{
    protected override Label PageStatus => Status;

    public OffboardingControl()
    {
        InitializeComponent();
        WireEvents();
    }

    private void WireEvents()
    {
        _refreshButton.Click += async (_, _) => await RunAsync(ReloadAsync);
        _createButton.Click += async (_, _) => await RunAsync(StartAsync);
        _editDateButton.Click += async (_, _) => await RunAsync(EditDateAsync);
        _checklistButton.Click += async (_, _) => await RunAsync(ChecklistAsync);
        _completeButton.Click += async (_, _) => await RunAsync(CompleteAsync);
        _records.SelectionChangeCommitted += (_, _) => ShowRecord();
    }

    private OffboardingRow Current => (_records.SelectedItem as Choice)?.Value as OffboardingRow
        ?? throw new InvalidOperationException("Chưa có hồ sơ nghỉ việc được chọn.");

    protected override async Task ReloadAsync()
    {
        var previousId = ((_records.SelectedItem as Choice)?.Value as OffboardingRow)?.OffboardingRecordId;
        var rows = await Api.GetAsync<List<OffboardingRow>>("offboarding", Token);
        Token.ThrowIfCancellationRequested();
        _records.Items.Clear();
        foreach (var row in rows) _records.Items.Add(new Choice(row, row.DisplayName));
        _records.SelectedItem = _records.Items.Cast<Choice>()
            .FirstOrDefault(x => ((OffboardingRow)x.Value!).OffboardingRecordId == previousId)
            ?? _records.Items.Cast<Choice>().FirstOrDefault();
        ShowRecord();
    }

    private void ShowRecord()
    {
        if (_records.SelectedItem is null)
        {
            _checklist.DataSource = null;
            Status.Text = "Chưa có hồ sơ nghỉ việc. Checklist được cấu hình khi tạo hồ sơ.";
            return;
        }

        var row = Current;
        _checklist.DataSource = row.Checklist;
        Status.Text = $"{row.Status} · Ngày cuối: {row.LastWorkingDate:dd/MM/yyyy} · Bắt buộc: "
            + $"{row.Checklist.Count(x => x.IsRequired && x.IsCompleted)}/{row.Checklist.Count(x => x.IsRequired)}. "
            + "Chỉ xác nhận khi đến ngày nghỉ và đủ bàn giao.";
    }

    private async Task StartAsync()
    {
        var employees = await Api.AllEmployeesAsync(Token);
        Token.ThrowIfCancellationRequested();
        using var form = new HrEditorForm("Tạo hồ sơ nghỉ việc");
        form.ChoiceField("employee", "Nhân viên *", employees.Where(x => x.Status != "RESIGNED")
                .Select(x => new Choice(x.EmployeeId, x.DisplayName)))
            .DateField("last", "Ngày làm việc cuối *")
            .TextField("notes", "Ghi chú", maxLength: 1000, multiline: true)
            .TextField("checklist", "Bàn giao: mỗi dòng 1 mục *",
                "Bàn giao công việc\r\nBàn giao tài sản\r\nHoàn tất thủ tục", true, 10000, true)
            .CheckField("required", "Các mục trên là bắt buộc (có thể sửa tên trước khi lưu)", true)
            .OnSave(async (f, token) =>
            {
                var checklist = f.TextValue("checklist")
                    .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(title => new ChecklistInput(title, f.CheckedValue("required"))).ToList();
                await Api.SendAsync<OffboardingRow>(HttpMethod.Post, "offboarding",
                    new OffboardingInput(f.GuidValue("employee"), f.DateValue("last"), f.OptionalText("notes"), checklist), token);
            });
        if (form.ShowDialog(this) == DialogResult.OK) await ReloadAsync();
    }

    private async Task EditDateAsync()
    {
        var row = Current;
        Ensure(row.Status == "IN_PROGRESS", "Hồ sơ đã hoàn tất, không thể chỉnh sửa.");
        using var form = new HrEditorForm("Cập nhật ngày nghỉ");
        form.DateField("date", "Ngày làm việc cuối *", row.LastWorkingDate)
            .TextField("notes", "Ghi chú", row.Notes, maxLength: 1000, multiline: true)
            .OnSave(async (f, token) => await Api.SendAsync<OffboardingRow>(HttpMethod.Put,
                $"offboarding/{row.OffboardingRecordId}",
                new { LastWorkingDate = f.DateValue("date"), Notes = f.OptionalText("notes"), ExpectedVersion = row.Version }, token));
        if (form.ShowDialog(this) == DialogResult.OK) await ReloadAsync();
    }

    private async Task ChecklistAsync()
    {
        var row = Current;
        var item = Selected<ChecklistRow>(_checklist);
        Ensure(row.Status == "IN_PROGRESS", "Hồ sơ đã hoàn tất, không thể chỉnh sửa.");
        using var form = new HrEditorForm(item.Title);
        form.CheckField("complete", "Đã hoàn thành hạng mục", item.IsCompleted)
            .TextField("notes", "Ghi chú bàn giao", item.CompletionNote, maxLength: 1000, multiline: true)
            .OnSave(async (f, token) => await Api.SendAsync<OffboardingRow>(HttpMethod.Put,
                $"offboarding/{row.OffboardingRecordId}/checklist/{item.OffboardingChecklistItemId}",
                new { IsCompleted = f.CheckedValue("complete"), CompletionNote = f.OptionalText("notes"), ExpectedVersion = row.Version }, token));
        if (form.ShowDialog(this) == DialogResult.OK) await ReloadAsync();
    }

    private async Task CompleteAsync()
    {
        var row = Current;
        Ensure(row.Status == "IN_PROGRESS", "Hồ sơ đã được xử lý.");
        Ensure(row.LastWorkingDate <= DateOnly.FromDateTime(DateTime.UtcNow), "Chưa đến ngày làm việc cuối cùng.");
        Ensure(row.Checklist.All(x => !x.IsRequired || x.IsCompleted), "Chưa hoàn tất các hạng mục bàn giao bắt buộc.");
        if (!Confirm($"Xác nhận {row.EmployeeName} nghỉ việc? Toàn bộ lịch sử công tác vẫn được giữ.")) return;
        await Api.SendAsync<OffboardingRow>(HttpMethod.Post, $"offboarding/{row.OffboardingRecordId}/complete",
            new { ExpectedVersion = row.Version }, Token);
        await ReloadAsync();
    }
}

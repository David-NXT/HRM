using Admin.WinForms.Forms.HR;
using Admin.WinForms.Models.HR;

namespace Admin.WinForms.Controls.HR;

public sealed class ContractAlertsControl : HrPageControl
{
    private readonly ComboBox _filter = Choices(new[] { "", "NEW", "VIEWED", "IN_PROGRESS" }
        .Select(x => new Choice(x, x == "" ? "Tất cả trạng thái" : x)));
    private readonly DataGridView _grid = Table(("Mã NV", "EmployeeCode"), ("Họ tên", "EmployeeName"),
        ("Hợp đồng", "ContractNumber"), ("Hết hạn", "ExpiryDate"), ("Còn ngày", "DaysRemaining"),
        ("Chu kỳ hiện tại", "IsCurrentCycle"), ("Trạng thái", "Status"));
    private readonly Label _summary = new() { AutoSize = true, Margin = new Padding(10) };
    private readonly Button _previous = new() { Text = "Trang trước", AutoSize = true };
    private readonly Button _next = new() { Text = "Trang sau", AutoSize = true };
    private int _page = 1;

    public ContractAlertsControl()
    {
        Toolbar.Controls.Add(_filter);
        ActionButton("Làm mới", ReloadAsync);
        ActionButton("Cập nhật trạng thái", UpdateAsync, true);
        Body.Controls.Add(_grid);
        Footer.Controls.AddRange([_previous, _summary, _next]);
        _previous.Click += async (_, _) => await RunAsync(async () => { _page--; await ReloadAsync(); });
        _next.Click += async (_, _) => await RunAsync(async () => { _page++; await ReloadAsync(); });
        _filter.SelectionChangeCommitted += async (_, _) => await RunAsync(async () => { _page = 1; await ReloadAsync(); });
    }

    protected override async Task ReloadAsync()
    {
        var filter = (_filter.SelectedItem as Choice)?.Value;
        var result = await Api.GetAsync<AlertPage>($"contract-alerts?status={filter}&page={Math.Max(1, _page)}&pageSize=20", Token);
        Token.ThrowIfCancellationRequested();
        if (result.Items.Count == 0 && _page > 1)
        {
            _page = Math.Max(1, (int)Math.Ceiling(result.Total / 20d));
            result = await Api.GetAsync<AlertPage>($"contract-alerts?status={filter}&page={_page}&pageSize=20", Token);
            Token.ThrowIfCancellationRequested();
        }
        _page = result.Page;
        _grid.DataSource = result.Items;
        _previous.Enabled = _page > 1;
        _next.Enabled = (long)_page * 20 < result.Total;
        _summary.Text = $"Trang {_page} · Tổng {result.Total} cảnh báo";
        Status.Text = "Cảnh báo do job HR tạo cho hợp đồng sắp hết hạn trong 30 ngày. Làm mới chỉ tải lại danh sách.";
    }

    private async Task UpdateAsync()
    {
        var row = Selected<AlertRow>(_grid);
        using var form = new HrEditorForm($"Cảnh báo hợp đồng {row.ContractNumber}");
        form.ChoiceField("status", "Trạng thái", new[] { "NEW", "VIEWED", "IN_PROGRESS" }.Select(x => new Choice(x, x)), row.Status)
            .OnSave(async (f, token) => await Api.SendAsync<AlertRow>(HttpMethod.Patch, $"contract-alerts/{row.ContractAlertId}",
                new { Status = (string)f.ChoiceValue("status")!, ExpectedVersion = row.Version }, token));
        if (form.ShowDialog(this) == DialogResult.OK) await ReloadAsync();
    }
}

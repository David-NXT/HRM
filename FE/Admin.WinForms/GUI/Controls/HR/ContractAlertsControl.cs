using Admin.WinForms.GUI.Forms.HR;
using Admin.WinForms.BUS.Models.HR;

namespace Admin.WinForms.GUI.Controls.HR;

public sealed partial class ContractAlertsControl : HrPageControl
{
    protected override Label PageStatus => Status;

    private int _page = 1;

    public ContractAlertsControl()
    {
        InitializeComponent();
        _filter.Items.AddRange(new object[]
        {
            new Choice("", "Tất cả trạng thái"),
            new Choice("NEW", "NEW"),
            new Choice("VIEWED", "VIEWED"),
            new Choice("IN_PROGRESS", "IN_PROGRESS")
        });
        _filter.SelectedIndex = 0;
        WireEvents();
    }

    private void WireEvents()
    {
        _refreshButton.Click += async (_, _) => await RunAsync(async () =>
        {
            _page = 1;
            await ReloadAsync();
        });
        _scanButton.Click += async (_, _) => await RunAsync(ScanAsync);
        _updateButton.Click += async (_, _) => await RunAsync(UpdateAsync);
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
        _filter.SelectionChangeCommitted += async (_, _) => await RunAsync(async () =>
        {
            _page = 1;
            await ReloadAsync();
        });
        _grid.CellFormatting += (_, e) =>
        {
            if (e.Value is DateOnly date)
            {
                e.Value = date.ToString("dd/MM/yyyy");
                e.FormattingApplied = true;
            }
        };
    }

    protected override async Task ReloadAsync()
    {
        var filter = (_filter.SelectedItem as Choice)?.Value;
        var result = await Api.GetAsync<AlertPage>(
            $"contract-alerts?status={filter}&page={Math.Max(1, _page)}&pageSize=20", Token);
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
        Status.Text = "Hợp đồng còn 0–30 ngày được kiểm tra khi lưu và quét hằng ngày. "
            + "Làm mới tải lại danh sách; Quét ngay kiểm tra thêm các hợp đồng đủ điều kiện.";
    }

    private async Task ScanAsync()
    {
        var result = await Api.SendAsync<AlertScanResult>(HttpMethod.Post, "contract-alerts/scan", null, Token);
        _page = 1;
        _filter.SelectedIndex = 0;
        await ReloadAsync();
        Status.Text = $"Quét ngày {result.ScanDate:dd/MM/yyyy}: kiểm tra {result.ScannedCount}, "
            + $"tạo mới {result.CreatedCount}, bỏ qua {result.SkippedCount}, lỗi {result.FailedCount}. "
            + (result.FailedCount > 0
                ? "Một số hợp đồng chưa kiểm tra được; bấm Quét ngay để thử lại."
                : "Đã tải lại tất cả trạng thái từ trang 1.");
    }

    private async Task UpdateAsync()
    {
        var row = Selected<AlertRow>(_grid);
        using var form = new HrEditorForm($"Cảnh báo hợp đồng {row.ContractNumber}");
        form.ChoiceField("status", "Trạng thái", new[] { "NEW", "VIEWED", "IN_PROGRESS" }
                .Select(x => new Choice(x, x)), row.Status)
            .OnSave(async (f, token) => await Api.SendAsync<AlertRow>(HttpMethod.Patch,
                $"contract-alerts/{row.ContractAlertId}",
                new { Status = (string)f.ChoiceValue("status")!, ExpectedVersion = row.Version }, token));
        if (form.ShowDialog(this) == DialogResult.OK) await ReloadAsync();
    }
}

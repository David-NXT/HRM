using Admin.WinForms.BUS.Models.HR;

namespace Admin.WinForms.GUI.Controls.HR;

public sealed partial class HrStatisticsControl : HrPageControl
{
    protected override Label PageStatus => Status;

    public HrStatisticsControl()
    {
        InitializeComponent();
        _refreshButton.Click += async (_, _) => await RunAsync(ReloadAsync);
    }

    protected override async Task ReloadAsync()
    {
        // Unknown counts stay blank if the API fails instead of appearing as zero.
        _totalValue.Text = _activeValue.Text = _lockedValue.Text = _resignedValue.Text =
            _fullTimeValue.Text = _partTimeValue.Text = "—";
        _updatedLabel.Text = "Chưa cập nhật dữ liệu";

        var statistics = await Api.GetAsync<EmployeeStatistics>("employees/statistics", Token);
        Token.ThrowIfCancellationRequested();
        _totalValue.Text = statistics.TotalEmployees.ToString("N0");
        _activeValue.Text = statistics.ActiveEmployees.ToString("N0");
        _lockedValue.Text = statistics.LockedEmployees.ToString("N0");
        _resignedValue.Text = statistics.ResignedEmployees.ToString("N0");
        _fullTimeValue.Text = statistics.FullTimeEmployees.ToString("N0");
        _partTimeValue.Text = statistics.PartTimeEmployees.ToString("N0");
        _updatedLabel.Text = $"Cập nhật lúc: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
        Status.Text = statistics.TotalEmployees == 0
            ? "Chưa có hồ sơ nhân viên trong hệ thống."
            : "Thống kê toàn bộ nhân viên. Bấm Làm mới sau khi thêm, xóa hoặc đổi trạng thái hồ sơ.";
    }
}

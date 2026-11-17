using Admin.WinForms.BUS.Models.HR;
using Admin.WinForms.DAO.HR;
using System.ComponentModel;
using System.Text.Json;

namespace Admin.WinForms.GUI.Controls.HR;

// Shared behavior only. Each page owns its layout in its own Designer file.
public partial class HrPageControl : UserControl
{
    private HrApiClient? _api;
    private readonly CancellationTokenSource _lifetime = new();
    private bool _busy;
    private bool _loaded;
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    protected virtual Label? PageStatus => null;
    private void SetStatus(string text) { if (PageStatus is Label label) label.Text = text; }
    protected HrApiClient Api => _api ??= new HrApiClient();
    protected CancellationToken Token => _lifetime.Token;
    public HrPageControl() => InitializeComponent();

    protected override async void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (_loaded || DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;
        _loaded = true;
        await RunAsync(ReloadAsync);
    }

    protected virtual Task ReloadAsync() => Task.CompletedTask;

    protected async Task RunAsync(Func<Task> action)
    {
        if (_busy || IsDisposed || _lifetime.IsCancellationRequested) return;
        _busy = true;
        Enabled = false;
        SetStatus("Đang xử lý...");
        UseWaitCursor = true;
        try { await action(); }
        catch (OperationCanceledException) when (_lifetime.IsCancellationRequested) { }
        catch (Exception ex) when (ex is HttpRequestException or JsonException or OperationCanceledException
            or InvalidOperationException or IOException or UnauthorizedAccessException)
        {
            if (!IsDisposed && !_lifetime.IsCancellationRequested)
                SetStatus(ex is HttpRequestException { StatusCode: null } or OperationCanceledException
                    ? $"Không kết nối được HR API. Kiểm tra API đang chạy và bấm Làm mới. {ex.Message}"
                    : ex.Message);
        }
        finally
        {
            if (!IsDisposed && !_lifetime.IsCancellationRequested)
            {
                Enabled = true;
                UseWaitCursor = false;
                _busy = false;
            }
        }
    }

    protected static T Selected<T>(DataGridView grid) where T : class =>
        grid.CurrentRow?.DataBoundItem as T ?? throw new InvalidOperationException("Hãy chọn một dòng trước.");

    protected static bool Confirm(string message) => MessageBox.Show(message, "Xác nhận",
        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

    protected static void Ensure(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && !IsDisposed)
        {
            _lifetime.Cancel();
            _api?.Dispose();
        }
        base.Dispose(disposing);
    }
}

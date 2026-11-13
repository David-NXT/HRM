using Admin.WinForms.Models.HR;
using Admin.WinForms.Services.HR;
using System.ComponentModel;
using System.Text.Json;

namespace Admin.WinForms.Controls.HR;

// Shared page chrome. Business rules remain in HR API.
public class HrPageControl : UserControl
{
    private HrApiClient? _api;
    private readonly CancellationTokenSource _lifetime = new();
    private bool _busy;
    private bool _loaded;
    protected HrApiClient Api => _api ??= new HrApiClient();
    protected CancellationToken Token => _lifetime.Token;
    protected FlowLayoutPanel Toolbar { get; } = new() { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true };
    protected Panel Body { get; } = new() { Dock = DockStyle.Fill };
    protected FlowLayoutPanel Footer { get; } = new() { Dock = DockStyle.Fill, AutoSize = true };
    protected Label Status { get; } = new() { AutoSize = true, Dock = DockStyle.Fill, ForeColor = Color.FromArgb(80, 96, 115) };

    public HrPageControl()
    {
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(16), ColumnCount = 1, RowCount = 4 };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        Status.Margin = new Padding(3, 8, 3, 12);
        layout.Controls.Add(Toolbar, 0, 0);
        layout.Controls.Add(Status, 0, 1);
        layout.Controls.Add(Body, 0, 2);
        layout.Controls.Add(Footer, 0, 3);
        Controls.Add(layout);
        BackColor = Color.FromArgb(244, 246, 249);
        Font = new Font("Segoe UI", 9F);
        Size = new Size(1100, 650);
    }

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
        Toolbar.Enabled = Body.Enabled = Footer.Enabled = false;
        Status.Text = "Đang xử lý...";
        UseWaitCursor = true;
        try { await action(); }
        catch (OperationCanceledException) when (_lifetime.IsCancellationRequested) { }
        catch (Exception ex) when (ex is HttpRequestException or JsonException or OperationCanceledException
            or InvalidOperationException or IOException or UnauthorizedAccessException)
        {
            if (!IsDisposed && !_lifetime.IsCancellationRequested)
                Status.Text = ex is HttpRequestException { StatusCode: null } or OperationCanceledException
                    ? $"Không kết nối được HR API. Kiểm tra API đang chạy và bấm Làm mới. {ex.Message}"
                    : ex.Message;
        }
        finally
        {
            if (!IsDisposed && !_lifetime.IsCancellationRequested)
            {
                Toolbar.Enabled = Body.Enabled = Footer.Enabled = true;
                UseWaitCursor = false;
                _busy = false;
            }
        }
    }

    protected Button ActionButton(string text, Func<Task> action, bool primary = false)
    {
        var button = new Button { Text = text, AutoSize = true, MinimumSize = new Size(95, 34), Margin = new Padding(3, 3, 8, 3) };
        if (primary)
        {
            button.BackColor = Color.FromArgb(35, 83, 166);
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
        }
        button.Click += async (_, _) => await RunAsync(action);
        Toolbar.Controls.Add(button);
        return button;
    }

    protected static DataGridView Table(params (string Title, string Property)[] columns)
    {
        var grid = new DataGridView
        {
            Dock = DockStyle.Fill, AutoGenerateColumns = false, ReadOnly = true,
            AllowUserToAddRows = false, AllowUserToDeleteRows = false,
            MultiSelect = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            RowHeadersVisible = false, BackgroundColor = Color.White,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BorderStyle = BorderStyle.FixedSingle, EnableHeadersVisualStyles = false
        };
        grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(230, 236, 246);
        grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(247, 249, 252);
        foreach (var (title, property) in columns)
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = title, DataPropertyName = property, Name = property,
                MinimumWidth = 100, SortMode = DataGridViewColumnSortMode.NotSortable
            });
        grid.CellFormatting += (_, e) =>
        {
            if (e.Value is DateOnly date) { e.Value = date.ToString("dd/MM/yyyy"); e.FormattingApplied = true; }
            else if (e.Value is DateTime time) { e.Value = time.ToLocalTime().ToString("dd/MM/yyyy HH:mm"); e.FormattingApplied = true; }
            else if (e.Value is bool value) { e.Value = value ? "Có" : "Không"; e.FormattingApplied = true; }
        };
        return grid;
    }

    protected static T Selected<T>(DataGridView grid) where T : class =>
        grid.CurrentRow?.DataBoundItem as T ?? throw new InvalidOperationException("Hãy chọn một dòng trước.");

    protected static ComboBox Choices(IEnumerable<Choice> choices)
    {
        var box = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 320 };
        foreach (var choice in choices) box.Items.Add(choice);
        if (box.Items.Count > 0) box.SelectedIndex = 0;
        return box;
    }

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

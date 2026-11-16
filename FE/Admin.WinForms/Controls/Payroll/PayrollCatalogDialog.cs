using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Admin.WinForms.DTOs.Payroll;

namespace Admin.WinForms.Controls.Payroll;

internal sealed class PayrollCatalogDialog : Form
{
    public PayrollCatalogDialog()
    {
        Text = "Quản lý danh mục phụ cấp và khấu trừ";
        ClientSize = new Size(920, 590);
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(920, 590);

        var tabs = new TabControl { Dock = DockStyle.Fill };
        var allowances = new TabPage("Phụ cấp");
        allowances.Controls.Add(new CatalogListControl(true) { Dock = DockStyle.Fill });
        var payCuts = new TabPage("Khấu trừ");
        payCuts.Controls.Add(new CatalogListControl(false) { Dock = DockStyle.Fill });
        tabs.TabPages.Add(allowances);
        tabs.TabPages.Add(payCuts);
        Controls.Add(tabs);
    }
}

internal sealed class CatalogListControl : UserControl
{
    private readonly bool _isAllowance;
    private readonly ListView _items = new();
    private readonly Button _edit = new();
    private readonly Button _deactivate = new();
    private List<PayrollCatalogItemDto> _catalogItems = new();

    public CatalogListControl(bool isAllowance)
    {
        _isAllowance = isAllowance;
        BuildControls();
        Load += async (_, _) => await ReloadAsync();
    }

    private void BuildControls()
    {
        _items.Dock = DockStyle.Fill;
        _items.View = View.Details;
        _items.FullRowSelect = true;
        _items.GridLines = true;
        _items.MultiSelect = false;
        _items.Columns.Add("Tên khoản", 190);
        _items.Columns.Add("Mô tả", 250);
        _items.Columns.Add("Cách tính", 110);
        _items.Columns.Add("Giá trị mặc định", 135);
        _items.Columns.Add("Trạng thái", 100);
        _items.SelectedIndexChanged += (_, _) => UpdateButtons();

        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 58,
            Padding = new Padding(8),
            FlowDirection = FlowDirection.LeftToRight
        };
        var add = new Button { Text = "Thêm mới", Width = 120, Height = 36 };
        _edit.Text = "Sửa";
        _edit.Width = 120;
        _edit.Height = 36;
        _deactivate.Width = 150;
        _deactivate.Height = 36;
        var reload = new Button { Text = "Tải lại", Width = 100, Height = 36 };

        add.Click += async (_, _) => await EditAsync(null);
        _edit.Click += async (_, _) => await EditAsync(SelectedItem());
        _deactivate.Click += async (_, _) => await ToggleStatusAsync(SelectedItem());
        reload.Click += async (_, _) => await ReloadAsync();

        actions.Controls.AddRange(new Control[] { add, _edit, _deactivate, reload });
        Controls.Add(_items);
        Controls.Add(actions);
        UpdateButtons();
    }

    private async Task ReloadAsync()
    {
        try
        {
            _catalogItems = _isAllowance
                ? await Program.PayrollApi.GetAllowanceTypesAsync()
                : await Program.PayrollApi.GetPayCutReasonsAsync();

            _items.BeginUpdate();
            _items.Items.Clear();
            foreach (var item in _catalogItems)
            {
                var row = new ListViewItem(item.Name) { Tag = item };
                row.SubItems.Add(item.Description);
                row.SubItems.Add(FormatCalculationType(item.CalculationType));
                row.SubItems.Add(FormatDefaultRate(item));
                row.SubItems.Add(item.Status == "active" ? "Đang dùng" : "Ngừng dùng");
                if (item.Status != "active")
                {
                    row.ForeColor = Color.Gray;
                }
                _items.Items.Add(row);
            }
            _items.EndUpdate();
            UpdateButtons();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi tải danh mục: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task EditAsync(PayrollCatalogItemDto? selected)
    {
        using var form = new CatalogItemEditDialog(selected);
        if (form.ShowDialog(this) != DialogResult.OK || form.Result == null)
        {
            return;
        }

        try
        {
            if (selected == null)
            {
                if (_isAllowance)
                {
                    await Program.PayrollApi.CreateAllowanceTypeAsync(form.Result);
                }
                else
                {
                    await Program.PayrollApi.CreatePayCutReasonAsync(form.Result);
                }
            }
            else if (_isAllowance)
            {
                await Program.PayrollApi.UpdateAllowanceTypeAsync(selected.ID, form.Result);
            }
            else
            {
                await Program.PayrollApi.UpdatePayCutReasonAsync(selected.ID, form.Result);
            }

            await ReloadAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Không thể lưu danh mục: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task ToggleStatusAsync(PayrollCatalogItemDto? item)
    {
        if (item == null)
        {
            return;
        }

        var activating = item.Status != "active";
        if (!activating && MessageBox.Show(
            $"Ngừng sử dụng “{item.Name}”? Khoản này sẽ không xuất hiện khi thêm phụ cấp/khấu trừ mới; dữ liệu lương cũ vẫn được giữ.",
            "Xác nhận ngừng sử dụng",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning) != DialogResult.Yes)
        {
            return;
        }

        try
        {
            if (activating)
            {
                var request = new SavePayrollCatalogItemDto
                {
                    Name = item.Name,
                    Description = item.Description,
                    CalculationType = item.CalculationType,
                    DefaultRate = item.DefaultRate,
                    IsActive = true
                };
                if (_isAllowance)
                {
                    await Program.PayrollApi.UpdateAllowanceTypeAsync(item.ID, request);
                }
                else
                {
                    await Program.PayrollApi.UpdatePayCutReasonAsync(item.ID, request);
                }
            }
            else if (_isAllowance)
            {
                await Program.PayrollApi.DeactivateAllowanceTypeAsync(item.ID);
            }
            else
            {
                await Program.PayrollApi.DeactivatePayCutReasonAsync(item.ID);
            }

            await ReloadAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Không thể cập nhật trạng thái danh mục: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private PayrollCatalogItemDto? SelectedItem() =>
        _items.SelectedItems.Count == 1
            ? _items.SelectedItems[0].Tag as PayrollCatalogItemDto
            : null;

    private void UpdateButtons()
    {
        var selected = SelectedItem();
        _edit.Enabled = selected != null;
        _deactivate.Enabled = selected != null;
        _deactivate.Text = selected?.Status == "active" ? "Ngừng sử dụng" : "Kích hoạt lại";
    }

    private static string FormatCalculationType(string type) => type switch
    {
        "percentage" => "Phần trăm",
        "formula" => "Công thức",
        _ => "Cố định"
    };

    private static string FormatDefaultRate(PayrollCatalogItemDto item) =>
        item.CalculationType == "percentage"
            ? $"{ToPercent(item.DefaultRate):0.##}%"
            : $"{item.DefaultRate:#,##0.##} đ";

    private static decimal ToPercent(decimal rate) => rate > 0 && rate <= 1m
        ? rate * 100m
        : rate;
}

internal sealed class CatalogItemEditDialog : Form
{
    private readonly TextBox _name = new();
    private readonly TextBox _description = new();
    private readonly ComboBox _calculationType = new();
    private readonly NumericUpDown _defaultRate = new();
    private readonly Label _rateLabel = new();
    private readonly CheckBox _isActive = new();

    public CatalogItemEditDialog(PayrollCatalogItemDto? item)
    {
        Text = item == null ? "Thêm danh mục" : "Sửa danh mục";
        ClientSize = new Size(540, 365);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BuildControls(item);
    }

    public SavePayrollCatalogItemDto? Result { get; private set; }

    private void BuildControls(PayrollCatalogItemDto? item)
    {
        AddLabel("Tên khoản:", 25, 25);
        _name.Location = new Point(175, 20);
        _name.Size = new Size(330, 28);
        _name.MaxLength = 120;
        _name.Text = item?.Name ?? string.Empty;
        Controls.Add(_name);

        AddLabel("Mô tả:", 25, 70);
        _description.Location = new Point(175, 65);
        _description.Size = new Size(330, 65);
        _description.Multiline = true;
        Controls.Add(_description);
        _description.Text = item?.Description ?? string.Empty;

        AddLabel("Kiểu tính:", 25, 150);
        _calculationType.Location = new Point(175, 145);
        _calculationType.Size = new Size(200, 30);
        _calculationType.DropDownStyle = ComboBoxStyle.DropDownList;
        _calculationType.Items.AddRange(new object[] { "Cố định", "Phần trăm", "Công thức" });
        _calculationType.SelectedIndex = item?.CalculationType switch
        {
            "percentage" => 1,
            "formula" => 2,
            _ => 0
        };
        _calculationType.SelectedIndexChanged += (_, _) => UpdateRateLabel();
        Controls.Add(_calculationType);

        _rateLabel.Location = new Point(25, 200);
        _rateLabel.AutoSize = true;
        Controls.Add(_rateLabel);
        _defaultRate.Location = new Point(175, 195);
        _defaultRate.Size = new Size(200, 30);
        _defaultRate.Minimum = 0;
        _defaultRate.Maximum = 100_000_000;
        _defaultRate.DecimalPlaces = 2;
        _defaultRate.ThousandsSeparator = true;
        var initialRate = item?.DefaultRate ?? 0m;
        if (item?.CalculationType == "percentage" && initialRate > 0 && initialRate <= 1m)
        {
            initialRate *= 100m;
        }
        _defaultRate.Value = Math.Clamp(initialRate, _defaultRate.Minimum, _defaultRate.Maximum);
        Controls.Add(_defaultRate);

        _isActive.Text = "Đang hoạt động";
        _isActive.Location = new Point(175, 245);
        _isActive.AutoSize = true;
        _isActive.Checked = item?.Status != "inactive";
        Controls.Add(_isActive);

        var save = new Button
        {
            Text = "Lưu",
            Location = new Point(270, 295),
            Size = new Size(110, 38),
            BackColor = Color.FromArgb(37, 99, 235),
            ForeColor = Color.White
        };
        save.Click += (_, _) => Save();
        Controls.Add(save);

        var cancel = new Button
        {
            Text = "Hủy",
            Location = new Point(395, 295),
            Size = new Size(110, 38),
            DialogResult = DialogResult.Cancel
        };
        Controls.Add(cancel);
        AcceptButton = save;
        CancelButton = cancel;
        UpdateRateLabel();
    }

    private void AddLabel(string text, int x, int y)
    {
        Controls.Add(new Label { Text = text, Location = new Point(x, y), AutoSize = true });
    }

    private void UpdateRateLabel()
    {
        _rateLabel.Text = _calculationType.SelectedIndex == 1
            ? "Tỷ lệ mặc định (%):"
            : _calculationType.SelectedIndex == 2
                ? "Số tiền mặc định (VNĐ):"
                : "Số tiền mặc định (VNĐ):";
        _defaultRate.Maximum = _calculationType.SelectedIndex == 1 ? 100m : 100_000_000m;
    }

    private void Save()
    {
        if (string.IsNullOrWhiteSpace(_name.Text))
        {
            MessageBox.Show("Vui lòng nhập tên danh mục.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        Result = new SavePayrollCatalogItemDto
        {
            Name = _name.Text.Trim(),
            Description = _description.Text.Trim(),
            CalculationType = _calculationType.SelectedIndex switch
            {
                1 => "percentage",
                2 => "formula",
                _ => "fixed"
            },
            DefaultRate = _defaultRate.Value,
            IsActive = _isActive.Checked
        };
        DialogResult = DialogResult.OK;
        Close();
    }
}

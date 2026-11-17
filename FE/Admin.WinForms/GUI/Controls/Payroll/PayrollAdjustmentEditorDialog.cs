using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Admin.WinForms.BUS.DTOs.Payroll;

namespace Admin.WinForms.GUI.Controls.Payroll;

internal sealed class PayrollAdjustmentEditorDialog : Form
{
    private readonly List<PayrollListDto> _draftPayrolls;
    private readonly ListView _employees = new();
    private readonly ListView _catalogItems = new();
    private readonly TextBox _search = new();
    private readonly CheckBox _applyToAll = new();
    private readonly NumericUpDown _value = new();
    private readonly Label _valueLabel = new();
    private readonly Label _calculationHint = new();
    private readonly TextBox _description = new();
    private PayrollCatalogItemDto? _selectedCatalogItem;

    public PayrollAdjustmentEditorDialog(
        string title,
        List<PayrollListDto> draftPayrolls,
        List<PayrollCatalogItemDto> catalogItems)
    {
        _draftPayrolls = draftPayrolls;
        Text = title;
        ClientSize = new Size(1080, 650);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        BuildControls(catalogItems);
        PopulateEmployees(_draftPayrolls);
        AcceptButton = AddButton;
        CancelButton = CancelButtonControl;
    }

    public Button AddButton { get; private set; } = null!;
    public Button CancelButtonControl { get; private set; } = null!;
    public PayrollCatalogItemDto? SelectedCatalogItem => _selectedCatalogItem;
    public string AdjustmentDescription => _description.Text.Trim();
    public IReadOnlyList<PayrollListDto> SelectedPayrolls =>
        _applyToAll.Checked
            ? _draftPayrolls
            : _employees.SelectedItems.Count == 1
                ? new[] { _employees.SelectedItems[0].Tag as PayrollListDto }
                    .OfType<PayrollListDto>()
                    .ToList()
                : Array.Empty<PayrollListDto>();
    public decimal RateApplied => _selectedCatalogItem?.CalculationType == "percentage"
        ? ToRateFraction(_value.Value)
        : 1m;

    public decimal GetAmount(PayrollListDto payroll) =>
        _selectedCatalogItem?.CalculationType == "percentage"
            ? Math.Round(payroll.BaseSalary * RateApplied, 2)
            : _value.Value;

    private void BuildControls(List<PayrollCatalogItemDto> catalogItems)
    {
        Controls.Add(new Label
        {
            Text = "Tìm nhân viên theo mã hoặc tên:",
            Location = new Point(20, 15),
            AutoSize = true
        });

        _search.Location = new Point(20, 42);
        _search.Size = new Size(350, 28);
        _search.PlaceholderText = "Nhập mã hoặc tên nhân viên";
        Controls.Add(_search);

        var searchButton = new Button
        {
            Text = "Tìm nhân viên",
            Location = new Point(380, 39),
            Size = new Size(120, 34)
        };
        searchButton.Click += async (_, _) => await SearchEmployeesAsync();
        Controls.Add(searchButton);

        _applyToAll.Text = "Áp dụng cho tất cả nhân viên có bảng lương nháp trong kỳ";
        _applyToAll.Location = new Point(20, 82);
        _applyToAll.AutoSize = true;
        _applyToAll.CheckedChanged += (_, _) => _employees.Enabled = !_applyToAll.Checked;
        Controls.Add(_applyToAll);

        ConfigureListView(_employees, new Point(20, 112), new Size(500, 455));
        AddColumn(_employees, "Mã NV", 105);
        AddColumn(_employees, "Họ tên", 205);
        AddColumn(_employees, "Trạng thái", 100);
        AddColumn(_employees, "Lương cơ bản", 125, HorizontalAlignment.Right);
        _employees.SelectedIndexChanged += (_, _) => UpdateCalculationHint();
        Controls.Add(_employees);

        Controls.Add(new Label
        {
            Text = "Chọn loại phụ cấp/khấu trừ:",
            Location = new Point(545, 15),
            AutoSize = true
        });

        ConfigureListView(_catalogItems, new Point(545, 42), new Size(515, 275));
        AddColumn(_catalogItems, "Tên khoản", 160);
        AddColumn(_catalogItems, "Kiểu tính", 80);
        AddColumn(_catalogItems, "Giá trị mặc định", 115, HorizontalAlignment.Right);
        AddColumn(_catalogItems, "Mô tả", 155);
        _catalogItems.SelectedIndexChanged += (_, _) => SelectCatalogItem();
        foreach (var item in catalogItems)
        {
            var row = new ListViewItem(item.Name) { Tag = item };
            row.SubItems.Add(FormatCalculationType(item.CalculationType));
            row.SubItems.Add(FormatDefaultRate(item));
            row.SubItems.Add(item.Description);
            _catalogItems.Items.Add(row);
        }
        Controls.Add(_catalogItems);

        _valueLabel.Text = "Số tiền (VNĐ):";
        _valueLabel.Location = new Point(545, 340);
        _valueLabel.AutoSize = true;
        Controls.Add(_valueLabel);

        _value.Location = new Point(700, 335);
        _value.Size = new Size(250, 30);
        _value.Minimum = 0;
        _value.Maximum = 100_000_000;
        _value.DecimalPlaces = 2;
        _value.ThousandsSeparator = true;
        _value.ValueChanged += (_, _) => UpdateCalculationHint();
        Controls.Add(_value);

        _calculationHint.Location = new Point(545, 375);
        _calculationHint.Size = new Size(515, 42);
        _calculationHint.ForeColor = Color.FromArgb(75, 85, 99);
        Controls.Add(_calculationHint);

        Controls.Add(new Label
        {
            Text = "Ghi chú:",
            Location = new Point(545, 425),
            AutoSize = true
        });
        _description.Location = new Point(545, 450);
        _description.Size = new Size(515, 90);
        _description.Multiline = true;
        Controls.Add(_description);

        AddButton = new Button
        {
            Text = "Đồng ý",
            Location = new Point(700, 575),
            Size = new Size(150, 42),
            BackColor = Color.FromArgb(37, 99, 235),
            ForeColor = Color.White
        };
        AddButton.Click += (_, _) =>
        {
            if (ValidateSelection())
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        };
        Controls.Add(AddButton);

        CancelButtonControl = new Button
        {
            Text = "Hủy",
            Location = new Point(870, 575),
            Size = new Size(150, 42),
            DialogResult = DialogResult.Cancel
        };
        Controls.Add(CancelButtonControl);
    }

    private static void ConfigureListView(ListView list, Point location, Size size)
    {
        list.Location = location;
        list.Size = size;
        list.View = View.Details;
        list.FullRowSelect = true;
        list.GridLines = true;
        list.MultiSelect = false;
    }

    private static void AddColumn(
        ListView list,
        string title,
        int width,
        HorizontalAlignment alignment = HorizontalAlignment.Left)
    {
        list.Columns.Add(new ColumnHeader { Text = title, Width = width, TextAlign = alignment });
    }

    private void PopulateEmployees(IEnumerable<PayrollListDto> payrolls)
    {
        _employees.BeginUpdate();
        _employees.Items.Clear();
        foreach (var payroll in payrolls)
        {
            var code = payroll.EmployeeCode
                ?? payroll.EmployeeID.ToString()[..8].ToUpperInvariant();
            var row = new ListViewItem(code) { Tag = payroll };
            row.SubItems.Add(payroll.EmployeeName ?? "Chưa có tên");
            row.SubItems.Add(payroll.Status);
            row.SubItems.Add(payroll.BaseSalary.ToString("#,##0 đ"));
            _employees.Items.Add(row);
        }
        _employees.EndUpdate();
    }

    private async Task SearchEmployeesAsync()
    {
        var keyword = _search.Text.Trim();
        if (string.IsNullOrWhiteSpace(keyword))
        {
            PopulateEmployees(_draftPayrolls);
            return;
        }

        try
        {
            var directoryMatches = await Program.AttendanceApi.SearchEmployeesAsync(keyword);
            var matchedIds = directoryMatches.Select(employee => employee.EmployeeId).ToHashSet();
            foreach (var employee in directoryMatches)
            {
                var payroll = _draftPayrolls.FirstOrDefault(item => item.EmployeeID == employee.EmployeeId);
                if (payroll != null)
                {
                    payroll.EmployeeCode = employee.EmployeeCode;
                    payroll.EmployeeName = employee.FullName;
                }
            }

            var matches = _draftPayrolls.Where(payroll =>
                matchedIds.Contains(payroll.EmployeeID)
                || (payroll.EmployeeCode?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false)
                || (payroll.EmployeeName?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false)
                || payroll.EmployeeID.ToString().Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .ToList();
            PopulateEmployees(matches);

            if (matches.Count == 0)
            {
                MessageBox.Show("Không tìm thấy nhân viên nháp trong kỳ lương này.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi tìm nhân viên: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void SelectCatalogItem()
    {
        _selectedCatalogItem = _catalogItems.SelectedItems.Count == 1
            ? _catalogItems.SelectedItems[0].Tag as PayrollCatalogItemDto
            : null;
        if (_selectedCatalogItem == null)
        {
            return;
        }

        var isPercentage = _selectedCatalogItem.CalculationType == "percentage";
        _valueLabel.Text = isPercentage ? "Tỷ lệ áp dụng (%):" : "Số tiền mặc định (VNĐ):";
        _value.Maximum = isPercentage ? 100m : 100_000_000m;
        var defaultValue = isPercentage
            ? ToPercent(_selectedCatalogItem.DefaultRate)
            : _selectedCatalogItem.DefaultRate;
        _value.Value = Math.Clamp(defaultValue, _value.Minimum, _value.Maximum);
        _description.Text = _selectedCatalogItem.Description;
        UpdateCalculationHint();
    }

    private void UpdateCalculationHint()
    {
        if (_selectedCatalogItem?.CalculationType == "percentage")
        {
            if (!_applyToAll.Checked && _employees.SelectedItems.Count == 1
                && _employees.SelectedItems[0].Tag is PayrollListDto payroll)
            {
                _calculationHint.Text = $"Tạm tính theo lương cơ bản {payroll.BaseSalary:#,##0 đ}: {GetAmount(payroll):#,##0.##} đ.";
            }
            else
            {
                _calculationHint.Text = "Tỷ lệ sẽ được tính riêng trên lương cơ bản của từng nhân viên.";
            }
        }
        else
        {
            _calculationHint.Text = _selectedCatalogItem?.CalculationType == "formula"
                ? "Nhập số tiền đã xác định theo công thức của khoản này."
                : "Số tiền này sẽ được áp dụng cho nhân viên đã chọn.";
        }
    }

    private bool ValidateSelection()
    {
        if (_selectedCatalogItem == null)
        {
            MessageBox.Show("Vui lòng chọn một khoản trong danh mục.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        if (!_applyToAll.Checked && _employees.SelectedItems.Count != 1)
        {
            MessageBox.Show("Vui lòng chọn một nhân viên trong danh sách.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        if (_value.Value <= 0)
        {
            MessageBox.Show("Giá trị áp dụng phải lớn hơn 0.", "Giá trị không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        return true;
    }

    private static decimal ToPercent(decimal rate) => rate > 0 && rate <= 1m
        ? rate * 100m
        : rate;

    private static decimal ToRateFraction(decimal percent) => percent / 100m;

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
}

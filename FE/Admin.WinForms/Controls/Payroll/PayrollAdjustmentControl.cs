using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Admin.WinForms.DTOs.Payroll;

namespace Admin.WinForms.Controls.Payroll
{
    public partial class PayrollAdjustmentControl : UserControl
    {
        private List<PayrollPeriodDto> _periods = new();
        private List<PayrollListDto> _payrolls = new();
        private PayrollListDto? _selectedPayroll;
        private bool _updatingEmployees;

        public PayrollAdjustmentControl()
        {
            InitializeComponent();

            comboBox_Periods.SelectedIndexChanged += async (s, e) => await LoadEmployeesInPeriodAsync();
            comboBox_Employees.SelectedIndexChanged += async (s, e) =>
            {
                if (_updatingEmployees) return;
                _selectedPayroll = comboBox_Employees.SelectedItem as PayrollListDto;
                UpdateAdjustmentButtons();
                await LoadAdjustmentDetailsAsync();
            };

            button_SearchEmployee.Click += async (s, e) => await FilterEmployeesAsync();
            button_AddAllowance.Click += async (s, e) => await OnAddAllowanceAsync();
            button_AddPayCut.Click += async (s, e) => await OnAddPayCutAsync();
            button_ManageCatalog.Click += (s, e) =>
            {
                using var dialog = new PayrollCatalogDialog();
                dialog.ShowDialog(this);
            };
            button_Reload.Click += async (s, e) => await LoadAdjustmentDetailsAsync();

            Load += async (s, e) => await LoadPeriodsAsync();
        }

        private async Task LoadPeriodsAsync()
        {
            try
            {
                _periods = await Program.PayrollPeriods.GetPeriodsAsync();
                comboBox_Periods.Items.Clear();

                foreach (var p in _periods)
                {
                    comboBox_Periods.Items.Add(p);
                }

                comboBox_Periods.DisplayMember = "DisplayName";
                comboBox_Periods.ValueMember = "PayrollPeriodID";

                if (comboBox_Periods.Items.Count > 0)
                {
                    comboBox_Periods.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải kỳ lương: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadEmployeesInPeriodAsync()
        {
            if (comboBox_Periods.SelectedItem is not PayrollPeriodDto period)
            {
                comboBox_Employees.Items.Clear();
                _selectedPayroll = null;
                UpdateAdjustmentButtons();
                return;
            }

            try
            {
                _payrolls = await Program.PayrollManagement.GetPayrollsByPeriodAsync(period.PayrollPeriodID);
                comboBox_Employees.Items.Clear();

                foreach (var p in _payrolls)
                {
                    p.EmployeeCode ??= p.EmployeeID.ToString()[..8].ToUpper();
                    p.EmployeeName ??= $"Nhân viên {p.EmployeeCode}";
                    comboBox_Employees.Items.Add(p);
                }

                comboBox_Employees.DisplayMember = "EmployeeDisplayName";
                comboBox_Employees.ValueMember = "PayrollID";

                if (comboBox_Employees.Items.Count > 0)
                {
                    comboBox_Employees.SelectedIndex = 0;
                }
                else
                {
                    listView_Allowances.Items.Clear();
                    listView_PayCuts.Items.Clear();
                    label_Summary.Text = "Chưa có dữ liệu bảng lương cho kỳ này.";
                }

                UpdateAdjustmentButtons();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách nhân viên: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task FilterEmployeesAsync()
        {
            var keyword = textBox_EmployeeSearch.Text.Trim();
            var selectedPayrollId = _selectedPayroll?.PayrollID;
            List<PayrollListDto> matches;
            if (string.IsNullOrWhiteSpace(keyword))
            {
                matches = _payrolls;
            }
            else
            {
                try
                {
                    var directoryMatches = await Program.AttendanceApi.SearchEmployeesAsync(keyword);
                    foreach (var employee in directoryMatches)
                    {
                        var payroll = _payrolls.FirstOrDefault(item => item.EmployeeID == employee.EmployeeId);
                        if (payroll != null)
                        {
                            payroll.EmployeeCode = employee.EmployeeCode;
                            payroll.EmployeeName = employee.FullName;
                        }
                    }

                    var matchingIds = directoryMatches.Select(employee => employee.EmployeeId).ToHashSet();
                    matches = _payrolls.Where(payroll =>
                        matchingIds.Contains(payroll.EmployeeID)
                        || (payroll.EmployeeCode?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false)
                        || (payroll.EmployeeName?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false)
                        || payroll.EmployeeID.ToString().Contains(keyword, StringComparison.OrdinalIgnoreCase)
                        || payroll.EmployeeID.ToString()[..8].Contains(keyword, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi tìm nhân viên: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            _updatingEmployees = true;
            try
            {
                comboBox_Employees.Items.Clear();
                foreach (var payroll in matches)
                {
                    comboBox_Employees.Items.Add(payroll);
                }

                comboBox_Employees.DisplayMember = "EmployeeDisplayName";
                comboBox_Employees.ValueMember = "PayrollID";
                var selectedIndex = matches.FindIndex(payroll => payroll.PayrollID == selectedPayrollId);
                if (selectedIndex < 0 && (string.IsNullOrWhiteSpace(keyword) || matches.Count == 1) && matches.Count > 0)
                {
                    selectedIndex = 0;
                }
                comboBox_Employees.SelectedIndex = selectedIndex;
            }
            finally
            {
                _updatingEmployees = false;
            }

            _selectedPayroll = comboBox_Employees.SelectedItem as PayrollListDto;
            UpdateAdjustmentButtons();
            await LoadAdjustmentDetailsAsync();

            if (matches.Count == 0)
            {
                MessageBox.Show("Không tìm thấy nhân viên trong kỳ lương này.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (matches.Count > 1 && _selectedPayroll == null)
            {
                MessageBox.Show($"Tìm thấy {matches.Count} nhân viên. Vui lòng chọn đúng nhân viên trong danh sách.", "Chọn nhân viên", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void UpdateAdjustmentButtons()
        {
            var hasDraftPayroll = _payrolls.Any(payroll =>
                string.Equals(payroll.Status, "draft", StringComparison.OrdinalIgnoreCase));
            button_AddAllowance.Enabled = hasDraftPayroll;
            button_AddPayCut.Enabled = hasDraftPayroll;
        }

        private async Task LoadAdjustmentDetailsAsync()
        {
            if (_selectedPayroll == null)
            {
                listView_Allowances.Items.Clear();
                listView_PayCuts.Items.Clear();
                label_Summary.Text = "Vui lòng chọn nhân viên.";
                return;
            }

            try
            {
                var detail = await Program.PayrollManagement.GetPayrollDetailAsync(_selectedPayroll.PayrollID);

                // 1. Render Allowances
                listView_Allowances.BeginUpdate();
                listView_Allowances.Items.Clear();
                decimal totalAllowance = 0;

                if (detail?.Allowances != null)
                {
                    foreach (var a in detail.Allowances)
                    {
                        totalAllowance += a.Amount;
                        var item = new ListViewItem(a.AllowanceTypeName);
                        item.SubItems.Add(a.Description);
                        item.SubItems.Add($"{a.RateApplied}");
                        item.SubItems.Add(a.Amount.ToString("#,##0 đ"));
                        listView_Allowances.Items.Add(item);
                    }
                }
                listView_Allowances.EndUpdate();

                // 2. Render PayCuts
                listView_PayCuts.BeginUpdate();
                listView_PayCuts.Items.Clear();
                decimal totalPayCut = 0;

                if (detail?.PayCuts != null)
                {
                    foreach (var c in detail.PayCuts)
                    {
                        totalPayCut += c.Amount;
                        var item = new ListViewItem(c.PayCutReasonName);
                        item.SubItems.Add(c.Description);
                        item.SubItems.Add($"{c.RateApplied}");
                        item.SubItems.Add(c.Amount.ToString("#,##0 đ"));
                        listView_PayCuts.Items.Add(item);
                    }
                }
                listView_PayCuts.EndUpdate();

                // 3. Update summary
                label_Summary.Text = $"Nhân sự: {_selectedPayroll.EmployeeDisplayName} | Phụ cấp dự án: {detail?.TotalProjectAllowance ?? 0:#,##0 đ} | Phụ cấp khác: {totalAllowance:#,##0 đ} | Khấu trừ: {totalPayCut:#,##0 đ} | Chênh lệch: {(totalAllowance - totalPayCut):+#,##0 đ;-#,##0 đ;0 đ}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải chi tiết phụ cấp/khấu trừ: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task OnAddAllowanceAsync()
        {
            await AddPayrollAdjustmentAsync(isAllowance: true);
        }

        private async Task OnAddPayCutAsync()
        {
            await AddPayrollAdjustmentAsync(isAllowance: false);
        }

        private async Task AddPayrollAdjustmentAsync(bool isAllowance)
        {
            var targets = _payrolls
                .Where(payroll => string.Equals(payroll.Status, "draft", StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (targets.Count == 0)
            {
                MessageBox.Show("Kỳ lương không có bảng lương ở trạng thái nháp để điều chỉnh.", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<PayrollCatalogItemDto> catalogItems;
            try
            {
                catalogItems = isAllowance
                    ? await Program.PayrollCatalog.GetAllowanceTypesAsync(activeOnly: true)
                    : await Program.PayrollCatalog.GetPayCutReasonsAsync(activeOnly: true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh mục: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (catalogItems.Count == 0)
            {
                MessageBox.Show("Danh mục đang trống. Hãy mở Quản lý danh mục để thêm khoản trước.", "Chưa có danh mục", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var dialog = new PayrollAdjustmentEditorDialog(
                isAllowance ? "Chọn phụ cấp và nhân viên" : "Chọn khoản khấu trừ và nhân viên",
                targets,
                catalogItems);
            if (dialog.ShowDialog(this) != DialogResult.OK || dialog.SelectedCatalogItem == null)
            {
                return;
            }

            var selectedPayrolls = dialog.SelectedPayrolls.ToList();
            if (selectedPayrolls.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn nhân viên để tiếp tục.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (selectedPayrolls.Count > 1
                && MessageBox.Show(
                    $"Xác nhận thêm “{dialog.SelectedCatalogItem.Name}” cho {selectedPayrolls.Count} nhân viên?",
                    "Xác nhận áp dụng hàng loạt",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            var failures = new List<string>();
            var succeeded = 0;
            foreach (var payroll in selectedPayrolls)
            {
                try
                {
                    if (isAllowance)
                    {
                        await Program.PayrollManagement.AddAllowanceAsync(payroll.PayrollID, new CreateAllowanceDto
                        {
                            AllowanceTypeID = dialog.SelectedCatalogItem.ID,
                            Amount = dialog.GetAmount(payroll),
                            RateApplied = dialog.RateApplied,
                            Description = string.IsNullOrWhiteSpace(dialog.AdjustmentDescription)
                                ? dialog.SelectedCatalogItem.Name
                                : dialog.AdjustmentDescription
                        });
                    }
                    else
                    {
                        await Program.PayrollManagement.AddPayCutAsync(payroll.PayrollID, new CreatePayCutDto
                        {
                            PayCutReasonID = dialog.SelectedCatalogItem.ID,
                            Amount = dialog.GetAmount(payroll),
                            RateApplied = dialog.RateApplied,
                            Description = string.IsNullOrWhiteSpace(dialog.AdjustmentDescription)
                                ? dialog.SelectedCatalogItem.Name
                                : dialog.AdjustmentDescription
                        });
                    }

                    succeeded++;
                }
                catch (Exception ex)
                {
                    failures.Add($"{payroll.EmployeeCode ?? payroll.EmployeeID.ToString()}: {ex.Message}");
                }
            }

            var result = $"Đã thêm khoản thành công cho {succeeded}/{selectedPayrolls.Count} nhân viên.";
            if (failures.Count > 0)
            {
                result += $"{Environment.NewLine}{Environment.NewLine}Các trường hợp lỗi:{Environment.NewLine}{string.Join(Environment.NewLine, failures)}";
            }
            MessageBox.Show(
                result,
                failures.Count == 0 ? "Thành công" : "Kết quả một phần",
                MessageBoxButtons.OK,
                failures.Count == 0 ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            await LoadAdjustmentDetailsAsync();
        }
    }
}

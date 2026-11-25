using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Admin.WinForms.DTOs.Payroll;

namespace Admin.WinForms.Controls.Payroll
{
    public partial class PayrollApprovalPaymentControl : UserControl
    {
        private List<PayrollPeriodDto> _periods = new();
        private List<PayrollListDto> _payrolls = new();
        private PayrollPeriodDto? _selectedPeriod;

        public PayrollApprovalPaymentControl()
        {
            InitializeComponent();

            comboBox_Periods.SelectedIndexChanged += async (s, e) =>
            {
                _selectedPeriod = comboBox_Periods.SelectedItem as PayrollPeriodDto;
                await LoadPayrollsAsync();
            };

            button_ApproveSelected.Click += async (s, e) => await OnApproveSelectedAsync();
            button_PaySelected.Click += async (s, e) => await OnPaySelectedAsync();
            button_ApproveAll.Click += async (s, e) => await OnApproveAllAsync();
            button_PayAll.Click += async (s, e) => await OnPayAllAsync();
            button_LockPeriod.Click += async (s, e) => await OnLockPeriodAsync();
            button_Reload.Click += async (s, e) => await LoadPayrollsAsync();

            UpdateSelectedActionButtons();
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

        private async Task LoadPayrollsAsync()
        {
            if (_selectedPeriod == null)
            {
                listView_Approvals.Items.Clear();
                ResetCards();
                return;
            }

            try
            {
                _payrolls = await Program.PayrollManagement.GetPayrollsByPeriodAsync(_selectedPeriod.PayrollPeriodID);

                listView_Approvals.BeginUpdate();
                listView_Approvals.Items.Clear();

                int calculatedCount = 0;
                int approvedCount = 0;
                int paidCount = 0;

                foreach (var p in _payrolls)
                {
                    var status = p.Status?.ToLowerInvariant() ?? "";
                    if (status == "paid") paidCount++;
                    else if (status == "approved") approvedCount++;
                    else calculatedCount++;

                    var empCode = !string.IsNullOrWhiteSpace(p.EmployeeCode) ? p.EmployeeCode : p.EmployeeID.ToString()[..8].ToUpper();
                    var item = new ListViewItem(empCode);

                    item.SubItems.Add(!string.IsNullOrWhiteSpace(p.EmployeeName) ? p.EmployeeName : $"Nhân viên {empCode}");
                    item.SubItems.Add(p.GrossSalary.ToString("#,##0 đ"));
                    item.SubItems.Add(p.TotalAllowance.ToString("#,##0 đ"));
                    item.SubItems.Add(p.TotalPayCut.ToString("#,##0 đ"));
                    item.SubItems.Add(p.NetSalary.ToString("#,##0 đ"));
                    item.SubItems.Add(FormatStatus(p.Status));
                    item.SubItems.Add(p.ApprovedAt.HasValue ? p.ApprovedAt.Value.ToString("dd/MM/yyyy HH:mm") : "--");
                    item.SubItems.Add(p.PaidAt.HasValue ? p.PaidAt.Value.ToString("dd/MM/yyyy HH:mm") : "--");

                    item.Tag = p.PayrollID;

                    if (status == "paid")
                    {
                        item.ForeColor = Color.FromArgb(16, 149, 74);
                    }
                    else if (status == "approved")
                    {
                        item.ForeColor = Color.FromArgb(37, 99, 235);
                    }
                    else
                    {
                        item.ForeColor = Color.FromArgb(180, 83, 9);
                    }

                    listView_Approvals.Items.Add(item);
                }

                listView_Approvals.EndUpdate();

                label_CountCalculated.Text = calculatedCount.ToString();
                label_CountApproved.Text = approvedCount.ToString();
                label_CountPaid.Text = paidCount.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách phê duyệt: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ResetCards()
        {
            label_CountCalculated.Text = "0";
            label_CountApproved.Text = "0";
            label_CountPaid.Text = "0";
        }

        private void listView_Approvals_SelectedIndexChanged(object? sender, EventArgs e)
        {
            UpdateSelectedActionButtons();
        }

        private void UpdateSelectedActionButtons()
        {
            var selectedPayroll = listView_Approvals.SelectedItems.Count == 1
                && listView_Approvals.SelectedItems[0].Tag is Guid payrollId
                    ? _payrolls.FirstOrDefault(p => p.PayrollID == payrollId)
                    : null;

            button_ApproveSelected.Enabled = selectedPayroll is not null
                && string.Equals(selectedPayroll.Status, "calculated", StringComparison.OrdinalIgnoreCase);
            button_PaySelected.Enabled = selectedPayroll is not null
                && string.Equals(selectedPayroll.Status, "approved", StringComparison.OrdinalIgnoreCase);
        }

        private static string FormatStatus(string? status)
        {
            return status?.ToLowerInvariant() switch
            {
                "draft" => "Bản nháp",
                "calculated" => "Chờ duyệt",
                "approved" => "Đã duyệt",
                "paid" => "Đã chi trả",
                _ => status ?? "--"
            };
        }

        private async Task OnApproveSelectedAsync()
        {
            if (listView_Approvals.SelectedItems.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn dòng bảng lương cần phê duyệt.", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (listView_Approvals.SelectedItems[0].Tag is not Guid payrollId) return;
            try
            {
                await Program.PayrollManagement.ApprovePayrollAsync(payrollId);
                MessageBox.Show("Đã phê duyệt bảng lương nhân viên thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadPayrollsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task OnPaySelectedAsync()
        {
            if (listView_Approvals.SelectedItems.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn dòng bảng lương cần xác nhận chi trả.", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (listView_Approvals.SelectedItems[0].Tag is not Guid payrollId) return;
            try
            {
                await Program.PayrollManagement.MarkAsPaidAsync(payrollId);
                MessageBox.Show("Đã xác nhận chi trả lương thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadPayrollsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task OnApproveAllAsync()
        {
            if (_selectedPeriod == null) return;
            var toApprove = _payrolls.Where(p => !p.Status.Equals("approved", StringComparison.OrdinalIgnoreCase) && !p.Status.Equals("paid", StringComparison.OrdinalIgnoreCase)).ToList();

            if (toApprove.Count == 0)
            {
                MessageBox.Show("Không có bảng lương nào đang ở trạng thái chờ duyệt.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirm = MessageBox.Show($"Bạn có chắc chắn muốn phê duyệt toàn bộ {toApprove.Count} bảng lương trong kỳ?", "Xác nhận phê duyệt", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            int count = 0;
            var failures = new List<string>();
            foreach (var p in toApprove)
            {
                try
                {
                    await Program.PayrollManagement.ApprovePayrollAsync(p.PayrollID);
                    count++;
                }
                catch (Exception ex)
                {
                    failures.Add($"{p.EmployeeCode ?? p.EmployeeID.ToString()}: {ex.Message}");
                }
            }

            var resultMessage = $"Đã phê duyệt thành công {count}/{toApprove.Count} bảng lương.";
            if (failures.Count > 0)
            {
                resultMessage += $"{Environment.NewLine}{Environment.NewLine}Lỗi:{Environment.NewLine}{string.Join(Environment.NewLine, failures)}";
            }

            MessageBox.Show(
                resultMessage,
                "Kết quả phê duyệt",
                MessageBoxButtons.OK,
                failures.Count == 0 ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            await LoadPayrollsAsync();
        }

        private async Task OnPayAllAsync()
        {
            if (_selectedPeriod == null) return;
            var toPay = _payrolls.Where(p => p.Status.Equals("approved", StringComparison.OrdinalIgnoreCase)).ToList();

            if (toPay.Count == 0)
            {
                MessageBox.Show("Chưa có bảng lương nào đã được phê duyệt sẵn sàng để chi trả.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirm = MessageBox.Show($"Xác nhận quyết toán và chi trả lương cho {toPay.Count} nhân sự đã được phê duyệt?", "Xác nhận chi trả", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            int count = 0;
            var failures = new List<string>();
            foreach (var p in toPay)
            {
                try
                {
                    await Program.PayrollManagement.MarkAsPaidAsync(p.PayrollID);
                    count++;
                }
                catch (Exception ex)
                {
                    failures.Add($"{p.EmployeeCode ?? p.EmployeeID.ToString()}: {ex.Message}");
                }
            }

            var resultMessage = $"Đã xác nhận chi trả lương thành công cho {count}/{toPay.Count} nhân sự.";
            if (failures.Count > 0)
            {
                resultMessage += $"{Environment.NewLine}{Environment.NewLine}Lỗi:{Environment.NewLine}{string.Join(Environment.NewLine, failures)}";
            }

            MessageBox.Show(
                resultMessage,
                "Kết quả chi trả",
                MessageBoxButtons.OK,
                failures.Count == 0 ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            await LoadPayrollsAsync();
        }

        private async Task OnLockPeriodAsync()
        {
            if (_selectedPeriod == null) return;

            var confirm = MessageBox.Show(
                $"Bạn có chắc chắn muốn cập nhật trạng thái / chốt đóng Kỳ lương Tháng {_selectedPeriod.Month}/{_selectedPeriod.Year}?\n\nSau khi khóa, kỳ lương sẽ được hoàn tất.",
                "Khóa kỳ lương",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            try
            {
                var result =                 await Program.PayrollPeriods.UpdatePeriodStatusAsync(_selectedPeriod.PayrollPeriodID);
                MessageBox.Show($"Đã cập nhật trạng thái kỳ lương thành: {result?.Status ?? "Completed"}!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadPeriodsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

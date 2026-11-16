using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Admin.WinForms.DTOs.Payroll;

namespace Admin.WinForms.Controls.Payroll
{
    public partial class PayslipViewerControl : UserControl
    {
        private List<PayrollPeriodDto> _periods = new();
        private List<PayrollListDto> _payrolls = new();
        private PayrollListDto? _selectedPayroll;
        private PayslipDto? _currentPayslip;

        public PayslipViewerControl()
        {
            InitializeComponent();

            panel_CenterWrapper.Resize += (s, e) =>
            {
                panel_SlipCard.Left = Math.Max(20, (panel_CenterWrapper.ClientSize.Width - panel_SlipCard.Width) / 2);
            };

            comboBox_Periods.SelectedIndexChanged += async (s, e) => await LoadEmployeesInPeriodAsync();
            comboBox_Employees.SelectedIndexChanged += async (s, e) =>
            {
                _selectedPayroll = comboBox_Employees.SelectedItem as PayrollListDto;
                await LoadPayslipAsync();
            };

            button_ExportPayslip.Click += (s, e) => ExportPayslip();
            button_Print.Click += (s, e) => MessageBox.Show("Đã gửi lệnh in phiếu lương tới máy in mặc định.", "In Phiếu Lương", MessageBoxButtons.OK, MessageBoxIcon.Information);
            button_Reload.Click += async (s, e) => await LoadPayslipAsync();

            Load += async (s, e) => await LoadPeriodsAsync();
        }

        private async Task LoadPeriodsAsync()
        {
            try
            {
                _periods = await Program.PayrollApi.GetPeriodsAsync();
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
                return;
            }

            try
            {
                _payrolls = await Program.PayrollApi.GetPayrollsByPeriodAsync(period.PayrollPeriodID);
                comboBox_Employees.Items.Clear();

                foreach (var p in _payrolls)
                {
                    p.EmployeeCode ??= p.EmployeeID.ToString()[..8].ToUpper();
                    p.EmployeeName ??= $"Nhân viên {p.EmployeeCode}";
                    comboBox_Employees.Items.Add(p);
                }

                comboBox_Employees.DisplayMember = "EmployeeName";
                comboBox_Employees.ValueMember = "PayrollID";

                if (comboBox_Employees.Items.Count > 0)
                {
                    comboBox_Employees.SelectedIndex = 0;
                }
                else
                {
                    ClearSlip();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách nhân viên: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadPayslipAsync()
        {
            if (_selectedPayroll == null)
            {
                ClearSlip();
                return;
            }

            try
            {
                _currentPayslip = await Program.PayrollApi.GetPayslipAsync(_selectedPayroll.PayrollID);

                if (_currentPayslip == null)
                {
                    ClearSlip();
                    label_SlipTitle.Text = "PHIẾU LƯƠNG CHƯA ĐƯỢC PHÁT HÀNH";
                    label_EmpCodeVal.Text = $"Mã NV: {_selectedPayroll.EmployeeCode}";
                    label_EmpNameVal.Text = $"Họ và Tên: {_selectedPayroll.EmployeeName}";
                    label_StatusVal.Text = "Trạng thái: Chưa phát hành";
                    return;
                }

                // Render dữ liệu lên phiếu
                label_SlipTitle.Text = $"PHIẾU LƯƠNG NHÂN VIÊN - THÁNG {_currentPayslip.Month:D2}/{_currentPayslip.Year}";
                label_EmpCodeVal.Text = $"Mã NV: {_selectedPayroll.EmployeeCode}";
                label_EmpNameVal.Text = $"Họ và Tên: {_selectedPayroll.EmployeeName}";
                label_StatusVal.Text = $"Trạng thái: {FormatStatus(_currentPayslip.Status)}";

                label_BaseSalary.Text = $"Lương cơ bản: {_currentPayslip.BaseSalary:#,##0 đ}";
                label_StandardDays.Text = $"Công chuẩn: {_currentPayslip.StandardWorkingDay:0.##} ngày";
                label_ActualDays.Text = $"Công thực tế: {_currentPayslip.WorkingDays:0.##} ngày";
                label_GrossSalary.Text = $"Lương Gross: {_currentPayslip.GrossSalary:#,##0 đ}";

                // Allowances
                listView_Allowances.BeginUpdate();
                listView_Allowances.Items.Clear();
                if (_currentPayslip.TotalProjectAllowance > 0)
                {
                    var projectAllowance = new ListViewItem("Phụ cấp dự án");
                    projectAllowance.SubItems.Add(
                        _currentPayslip.TotalProjectAllowance.ToString("#,##0 đ"));
                    listView_Allowances.Items.Add(projectAllowance);
                }

                if (_currentPayslip.Allowances != null)
                {
                    foreach (var a in _currentPayslip.Allowances)
                    {
                        var item = new ListViewItem(a.AllowanceTypeName);
                        item.SubItems.Add(a.Amount.ToString("#,##0 đ"));
                        listView_Allowances.Items.Add(item);
                    }
                }
                listView_Allowances.EndUpdate();

                // PayCuts
                listView_PayCuts.BeginUpdate();
                listView_PayCuts.Items.Clear();
                if (_currentPayslip.PayCuts != null)
                {
                    foreach (var c in _currentPayslip.PayCuts)
                    {
                        var item = new ListViewItem(c.PayCutReasonName);
                        item.SubItems.Add(c.Amount.ToString("#,##0 đ"));
                        listView_PayCuts.Items.Add(item);
                    }
                }
                listView_PayCuts.EndUpdate();

                // Net Salary
                label_NetValue.Text = _currentPayslip.NetSalary.ToString("#,##0 đ");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải phiếu lương: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearSlip()
        {
            label_SlipTitle.Text = "PHIẾU LƯƠNG NHÂN VIÊN";
            label_EmpCodeVal.Text = "Mã NV: --";
            label_EmpNameVal.Text = "Họ và Tên: --";
            label_StatusVal.Text = "Trạng thái: --";
            label_BaseSalary.Text = "Lương cơ bản: 0 đ";
            label_StandardDays.Text = "Công chuẩn: 0";
            label_ActualDays.Text = "Công thực tế: 0";
            label_GrossSalary.Text = "Lương Gross: 0 đ";
            label_NetValue.Text = "0 đ";
            listView_Allowances.Items.Clear();
            listView_PayCuts.Items.Clear();
        }

        private static string FormatStatus(string status)
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

        private void ExportPayslip()
        {
            if (_currentPayslip == null || _selectedPayroll == null)
            {
                MessageBox.Show("Chưa có phiếu lương để xuất.", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using var sfd = new SaveFileDialog
                {
                    Filter = "Text File (*.txt)|*.txt",
                    FileName = $"PhieuLuong_{_selectedPayroll.EmployeeCode}_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
                };

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("=============================================================");
                    sb.AppendLine("            TẬP ĐOÀN CÔNG NGHỆ & NHÂN SỰ HRM");
                    sb.AppendLine($"        PHIẾU LƯƠNG NHÂN VIÊN - THÁNG {_currentPayslip.Month:D2}/{_currentPayslip.Year}");
                    sb.AppendLine("=============================================================");
                    sb.AppendLine($"Mã nhân viên: {_selectedPayroll.EmployeeCode}");
                    sb.AppendLine($"Họ và tên:    {_selectedPayroll.EmployeeName}");
                    sb.AppendLine($"Trạng thái:   {FormatStatus(_currentPayslip.Status)}");
                    sb.AppendLine("-------------------------------------------------------------");
                    sb.AppendLine("1. LƯƠNG & CÔNG LÀM VIỆC:");
                    sb.AppendLine($" - Lương cơ bản:    {_currentPayslip.BaseSalary:#,##0 đ}");
                    sb.AppendLine($" - Ngày công chuẩn: {_currentPayslip.StandardWorkingDay:0.##} ngày");
                    sb.AppendLine($" - Ngày công làm:   {_currentPayslip.WorkingDays:0.##} ngày");
                    sb.AppendLine($" - Lương Gross:     {_currentPayslip.GrossSalary:#,##0 đ}");
                    sb.AppendLine("-------------------------------------------------------------");
                    sb.AppendLine("2. CÁC KHOẢN PHỤ CẤP (+):");
                    if (_currentPayslip.Allowances != null && _currentPayslip.Allowances.Count > 0)
                    {
                        foreach (var a in _currentPayslip.Allowances)
                        {
                            sb.AppendLine($" + {a.AllowanceTypeName,-35} : {a.Amount,15:#,##0 đ}");
                        }
                    }
                    else
                    {
                        sb.AppendLine(" (Không có phụ cấp)");
                    }
                    sb.AppendLine("-------------------------------------------------------------");
                    sb.AppendLine("3. CÁC KHOẢN TRÍCH ĐÓNG & KHẤU TRỪ (-):");
                    if (_currentPayslip.PayCuts != null && _currentPayslip.PayCuts.Count > 0)
                    {
                        foreach (var c in _currentPayslip.PayCuts)
                        {
                            sb.AppendLine($" - {c.PayCutReasonName,-35} : {c.Amount,15:#,##0 đ}");
                        }
                    }
                    else
                    {
                        sb.AppendLine(" (Không có khoản giảm trừ)");
                    }
                    sb.AppendLine("=============================================================");
                    sb.AppendLine($"  THỰC LĨNH (NET SALARY): {_currentPayslip.NetSalary:#,##0 đ}");
                    sb.AppendLine("=============================================================");
                    sb.AppendLine($"Ngày xuất phiếu: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");

                    File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                    MessageBox.Show("Xuất file phiếu lương thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xuất phiếu lương: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

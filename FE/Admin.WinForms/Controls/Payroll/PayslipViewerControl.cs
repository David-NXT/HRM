using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Admin.WinForms.DTOs.Payroll;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

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
                _currentPayslip = await Program.Payslips.GetPayslipAsync(_selectedPayroll.PayrollID);

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
                label_ProjectAllowance.Text = $"Tổng phụ cấp dự án: {_currentPayslip.TotalProjectAllowance:#,##0 đ}";
                label_TotalAllowance.Text = $"Tổng phụ cấp: {_currentPayslip.TotalAllowance:#,##0 đ}";

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
            label_ProjectAllowance.Text = "Tổng phụ cấp dự án: 0 đ";
            label_TotalAllowance.Text = "Tổng phụ cấp: 0 đ";
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
                    Filter = "PDF Document (*.pdf)|*.pdf",
                    DefaultExt = "pdf",
                    AddExtension = true,
                    FileName = $"PhieuLuong_{SafeFileName(_selectedPayroll.EmployeeCode ?? _selectedPayroll.EmployeeID.ToString())}_{_currentPayslip.Month:D2}_{_currentPayslip.Year}.pdf"
                };

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    QuestPDF.Settings.License = LicenseType.Community;
                    Document.Create(document =>
                    {
                        document.Page(page =>
                        {
                            page.Size(PageSizes.A4);
                            page.Margin(36);
                            page.DefaultTextStyle(style => style.FontSize(10));

                            page.Header().Column(column =>
                            {
                                column.Item().AlignCenter().Text("TẬP ĐOÀN CÔNG NGHỆ & NHÂN SỰ HRM")
                                    .FontSize(12).SemiBold();
                                column.Item().PaddingTop(8).AlignCenter()
                                    .Text($"PHIẾU LƯƠNG NHÂN VIÊN - THÁNG {_currentPayslip.Month:D2}/{_currentPayslip.Year}")
                                    .FontSize(16).Bold();
                            });

                            page.Content().PaddingTop(20).Column(column =>
                            {
                                column.Spacing(8);
                                column.Item().Text($"Mã nhân viên: {_selectedPayroll.EmployeeCode ?? _currentPayslip.EmployeeCode ?? "--"}");
                                column.Item().Text($"Họ và tên: {_selectedPayroll.EmployeeName ?? _currentPayslip.EmployeeName ?? "--"}");
                                column.Item().Text($"Trạng thái: {FormatStatus(_currentPayslip.Status)}");

                                AddPdfSection(column, "1. LƯƠNG & NGÀY CÔNG");
                                AddPdfRow(column, "Lương cơ bản", $"{_currentPayslip.BaseSalary:#,##0 đ}");
                                AddPdfRow(column, "Ngày công chuẩn", $"{_currentPayslip.StandardWorkingDay:0.##} ngày");
                                AddPdfRow(column, "Ngày công thực tế", $"{_currentPayslip.WorkingDays:0.##} ngày");
                                AddPdfRow(column, "Lương Gross", $"{_currentPayslip.GrossSalary:#,##0 đ}");

                                AddPdfSection(column, "2. CÁC KHOẢN PHỤ CẤP (+)");
                                if (_currentPayslip.TotalProjectAllowance > 0)
                                {
                                    AddPdfDetailRow(column, "Phụ cấp dự án", "--", _currentPayslip.TotalProjectAllowance);
                                }
                                foreach (var allowance in _currentPayslip.Allowances)
                                {
                                    AddPdfDetailRow(
                                        column,
                                        allowance.AllowanceTypeName,
                                        allowance.Description,
                                        allowance.Amount);
                                }
                                if (_currentPayslip.TotalProjectAllowance <= 0 && _currentPayslip.Allowances.Count == 0)
                                {
                                    column.Item().Text("(Không có phụ cấp)").Italic();
                                }
                                AddPdfRow(column, "TỔNG PHỤ CẤP", $"{_currentPayslip.TotalAllowance:#,##0 đ}", true);

                                AddPdfSection(column, "3. CÁC KHOẢN KHẤU TRỪ (-)");
                                if (_currentPayslip.PayCuts.Count == 0)
                                {
                                    column.Item().Text("(Không có khoản giảm trừ)").Italic();
                                }
                                foreach (var payCut in _currentPayslip.PayCuts)
                                {
                                    AddPdfDetailRow(
                                        column,
                                        payCut.PayCutReasonName,
                                        payCut.Description,
                                        payCut.Amount);
                                }
                                AddPdfRow(column, "TỔNG KHẤU TRỪ", $"{_currentPayslip.TotalPayCut:#,##0 đ}", true);

                                column.Item().PaddingTop(8).BorderTop(1).BorderColor(Colors.Grey.Lighten1)
                                    .PaddingTop(10).Row(row =>
                                    {
                                        row.RelativeItem().Text("THỰC LĨNH (NET SALARY)").Bold().FontSize(14);
                                        row.ConstantItem(180).AlignRight()
                                            .Text($"{_currentPayslip.NetSalary:#,##0 đ}").Bold().FontSize(14)
                                            .FontColor(Colors.Green.Darken2);
                                    });
                                column.Item().PaddingTop(12).AlignRight()
                                    .Text($"Ngày xuất phiếu: {DateTime.Now:dd/MM/yyyy HH:mm:ss}")
                                    .FontSize(8).FontColor(Colors.Grey.Darken1);
                            });

                            page.Footer().AlignCenter().Text(text =>
                            {
                                text.Span("Trang ");
                                text.CurrentPageNumber();
                                text.Span(" / ");
                                text.TotalPages();
                            });
                        });
                    }).GeneratePdf(sfd.FileName);
                    MessageBox.Show("Xuất file phiếu lương thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xuất phiếu lương: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void AddPdfSection(ColumnDescriptor column, string title)
        {
            column.Item().PaddingTop(8).BorderBottom(1).BorderColor(Colors.Grey.Lighten1)
                .PaddingBottom(4).Text(title).Bold().FontSize(11);
        }

        private static void AddPdfRow(ColumnDescriptor column, string label, string value, bool bold = false)
        {
            column.Item().Row(row =>
            {
                var labelText = row.RelativeItem().Text(label);
                var valueText = row.ConstantItem(180).AlignRight().Text(value);
                if (bold)
                {
                    labelText.Bold();
                    valueText.Bold();
                }
            });
        }

        private static void AddPdfDetailRow(ColumnDescriptor column, string name, string? description, decimal amount)
        {
            column.Item().Row(row =>
            {
                row.RelativeItem(3).Text(name);
                row.RelativeItem(4).Text(string.IsNullOrWhiteSpace(description) ? "--" : description)
                    .FontColor(Colors.Grey.Darken1);
                row.RelativeItem(2).AlignRight().Text($"{amount:#,##0 đ}");
            });
        }

        private static string SafeFileName(string value)
        {
            foreach (var invalidCharacter in Path.GetInvalidFileNameChars())
            {
                value = value.Replace(invalidCharacter, '_');
            }

            return value;
        }
    }
}

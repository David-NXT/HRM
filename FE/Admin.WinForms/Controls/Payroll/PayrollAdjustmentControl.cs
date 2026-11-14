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

        // Dữ liệu loại phụ cấp & lý do khấu trừ mẫu khớp với Database Seeder
        private static readonly List<AllowanceTypeItemDto> DefaultAllowanceTypes = new()
        {
            new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111101"), Name = "Phụ cấp ăn trưa", DefaultAmount = 730000 },
            new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111102"), Name = "Phụ cấp xăng xe & đi lại", DefaultAmount = 500000 },
            new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111103"), Name = "Phụ cấp trách nhiệm", DefaultAmount = 1000000 },
            new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111104"), Name = "Làm thêm giờ (OT)", DefaultAmount = 500000 }
        };

        private static readonly List<PayCutReasonItemDto> DefaultPayCutReasons = new()
        {
            new() { Id = Guid.Parse("22222222-2222-2222-2222-222222222201"), Name = "Bảo hiểm xã hội (BHXH 8%)", DefaultAmount = 800000 },
            new() { Id = Guid.Parse("22222222-2222-2222-2222-222222222202"), Name = "Bảo hiểm y tế (BHYT 1.5%)", DefaultAmount = 150000 },
            new() { Id = Guid.Parse("22222222-2222-2222-2222-222222222203"), Name = "Bảo hiểm thất nghiệp (BHTN 1%)", DefaultAmount = 100000 },
            new() { Id = Guid.Parse("22222222-2222-2222-2222-222222222204"), Name = "Thuế thu nhập cá nhân (TNCN)", DefaultAmount = 300000 },
            new() { Id = Guid.Parse("22222222-2222-2222-2222-222222222205"), Name = "Phạt đi muộn / về sớm", DefaultAmount = 100000 }
        };

        public PayrollAdjustmentControl()
        {
            InitializeComponent();

            comboBox_Periods.SelectedIndexChanged += async (s, e) => await LoadEmployeesInPeriodAsync();
            comboBox_Employees.SelectedIndexChanged += async (s, e) =>
            {
                _selectedPayroll = comboBox_Employees.SelectedItem as PayrollListDto;
                await LoadAdjustmentDetailsAsync();
            };

            button_AddAllowance.Click += async (s, e) => await OnAddAllowanceAsync();
            button_AddPayCut.Click += async (s, e) => await OnAddPayCutAsync();
            button_Reload.Click += async (s, e) => await LoadAdjustmentDetailsAsync();

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
                    listView_Allowances.Items.Clear();
                    listView_PayCuts.Items.Clear();
                    label_Summary.Text = "Chưa có dữ liệu bảng lương cho kỳ này.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách nhân viên: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                var detail = await Program.PayrollApi.GetPayrollDetailAsync(_selectedPayroll.PayrollID);

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
                label_Summary.Text = $"Nhân sự: {_selectedPayroll.EmployeeName} | Tổng phụ cấp: {totalAllowance:#,##0 đ} | Tổng giảm trừ/phạt: {totalPayCut:#,##0 đ} | Chênh lệch: {(totalAllowance - totalPayCut):+#,##0 đ;-#,##0 đ;0 đ}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải chi tiết phụ cấp/khấu trừ: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task OnAddAllowanceAsync()
        {
            if (_selectedPayroll == null)
            {
                MessageBox.Show("Vui lòng chọn nhân viên.", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var form = new Form
            {
                Text = "Thêm Phụ Cấp Cho Nhân Viên",
                Size = new Size(500, 360),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var labelType = new Label { Text = "Loại phụ cấp:", Location = new Point(30, 30), AutoSize = true };
            var cmbType = new ComboBox { Location = new Point(150, 26), Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (var t in DefaultAllowanceTypes) cmbType.Items.Add(t);
            cmbType.DisplayMember = "Name";
            cmbType.ValueMember = "Id";
            if (cmbType.Items.Count > 0) cmbType.SelectedIndex = 0;

            var labelAmount = new Label { Text = "Số tiền (VNĐ):", Location = new Point(30, 80), AutoSize = true };
            var numAmount = new NumericUpDown { Location = new Point(150, 76), Minimum = 1000, Maximum = 100000000, Increment = 50000, Width = 200 };
            numAmount.Value = DefaultAllowanceTypes[0].DefaultAmount;

            cmbType.SelectedIndexChanged += (s, e) =>
            {
                if (cmbType.SelectedItem is AllowanceTypeItemDto sel && sel.DefaultAmount > 0)
                {
                    numAmount.Value = sel.DefaultAmount;
                }
            };

            var labelDesc = new Label { Text = "Ghi chú / Lý do:", Location = new Point(30, 130), AutoSize = true };
            var txtDesc = new TextBox { Location = new Point(150, 126), Width = 300, Height = 60, Multiline = true };

            var btnOk = new Button { Text = "Lưu Phụ Cấp", DialogResult = DialogResult.OK, Location = new Point(150, 220), Width = 140, BackColor = Color.FromArgb(16, 185, 129), ForeColor = Color.White };
            var btnCancel = new Button { Text = "Hủy", DialogResult = DialogResult.Cancel, Location = new Point(310, 220), Width = 100 };

            form.Controls.AddRange(new Control[] { labelType, cmbType, labelAmount, numAmount, labelDesc, txtDesc, btnOk, btnCancel });
            form.AcceptButton = btnOk;
            form.CancelButton = btnCancel;

            if (form.ShowDialog() == DialogResult.OK)
            {
                if (cmbType.SelectedItem is not AllowanceTypeItemDto selectedType) return;

                try
                {
                    var dto = new CreateAllowanceDto
                    {
                        AllowanceTypeID = selectedType.Id,
                        Amount = numAmount.Value,
                        RateApplied = 1,
                        Description = string.IsNullOrWhiteSpace(txtDesc.Text) ? selectedType.Name : txtDesc.Text.Trim()
                    };

                    await Program.PayrollApi.AddAllowanceAsync(_selectedPayroll.PayrollID, dto);
                    MessageBox.Show("Thêm phụ cấp thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadAdjustmentDetailsAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async Task OnAddPayCutAsync()
        {
            if (_selectedPayroll == null)
            {
                MessageBox.Show("Vui lòng chọn nhân viên.", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var form = new Form
            {
                Text = "Thêm Khoản Khấu Trừ / Phạt",
                Size = new Size(500, 360),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var labelReason = new Label { Text = "Lý do giảm trừ:", Location = new Point(30, 30), AutoSize = true };
            var cmbReason = new ComboBox { Location = new Point(150, 26), Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (var r in DefaultPayCutReasons) cmbReason.Items.Add(r);
            cmbReason.DisplayMember = "Name";
            cmbReason.ValueMember = "Id";
            if (cmbReason.Items.Count > 0) cmbReason.SelectedIndex = 0;

            var labelAmount = new Label { Text = "Số tiền (VNĐ):", Location = new Point(30, 80), AutoSize = true };
            var numAmount = new NumericUpDown { Location = new Point(150, 76), Minimum = 1000, Maximum = 100000000, Increment = 50000, Width = 200 };
            numAmount.Value = DefaultPayCutReasons[0].DefaultAmount;

            cmbReason.SelectedIndexChanged += (s, e) =>
            {
                if (cmbReason.SelectedItem is PayCutReasonItemDto sel && sel.DefaultAmount > 0)
                {
                    numAmount.Value = sel.DefaultAmount;
                }
            };

            var labelDesc = new Label { Text = "Ghi chú / Lý do:", Location = new Point(30, 130), AutoSize = true };
            var txtDesc = new TextBox { Location = new Point(150, 126), Width = 300, Height = 60, Multiline = true };

            var btnOk = new Button { Text = "Lưu Khấu Trừ", DialogResult = DialogResult.OK, Location = new Point(150, 220), Width = 140, BackColor = Color.FromArgb(239, 68, 68), ForeColor = Color.White };
            var btnCancel = new Button { Text = "Hủy", DialogResult = DialogResult.Cancel, Location = new Point(310, 220), Width = 100 };

            form.Controls.AddRange(new Control[] { labelReason, cmbReason, labelAmount, numAmount, labelDesc, txtDesc, btnOk, btnCancel });
            form.AcceptButton = btnOk;
            form.CancelButton = btnCancel;

            if (form.ShowDialog() == DialogResult.OK)
            {
                if (cmbReason.SelectedItem is not PayCutReasonItemDto selectedReason) return;

                try
                {
                    var dto = new CreatePayCutDto
                    {
                        PayCutReasonID = selectedReason.Id,
                        Amount = numAmount.Value,
                        RateApplied = 1,
                        Description = string.IsNullOrWhiteSpace(txtDesc.Text) ? selectedReason.Name : txtDesc.Text.Trim()
                    };

                    await Program.PayrollApi.AddPayCutAsync(_selectedPayroll.PayrollID, dto);
                    MessageBox.Show("Thêm khoản khấu trừ thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadAdjustmentDetailsAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}

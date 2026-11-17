using Admin.WinForms.BUS.DTOs.Attendance;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Admin.WinForms.GUI.Controls.Attendance
{
    public partial class AttendanceConfigControl : UserControl
    {
        // ───── Lưu ID hàng đang chọn ─────
        private Guid _selectedShiftId = Guid.Empty;
        private Guid _selectedRuleId = Guid.Empty;
        private Guid _selectedOTRuleId = Guid.Empty;

        public AttendanceConfigControl()
        {
            InitializeComponent();

            // ── Reload ──
            button_ReloadShifts.Click += async (s, e) => await LoadShiftsAsync();
            button_ReloadRules.Click  += async (s, e) => await LoadRulesAsync();
            button_ReloadOT.Click     += async (s, e) => await LoadOTRulesAsync();

            // ── Shifts CRUD ──
            button_AddShift.Click    += async (s, e) => await OnAddShiftAsync();
            button_EditShift.Click   += async (s, e) => await OnEditShiftAsync();
            button_DeleteShift.Click += async (s, e) => await OnDeleteShiftAsync();
            listView_Shifts.SelectedIndexChanged += (s, e) =>
            {
                _selectedShiftId = listView_Shifts.SelectedItems.Count > 0
                    ? (Guid)(listView_Shifts.SelectedItems[0].Tag ?? Guid.Empty)
                    : Guid.Empty;
            };

            // ── Rules CRUD ──
            button_AddRule.Click    += async (s, e) => await OnAddRuleAsync();
            button_EditRule.Click   += async (s, e) => await OnEditRuleAsync();
            button_DeleteRule.Click += async (s, e) => await OnDeleteRuleAsync();
            listView_Rules.SelectedIndexChanged += (s, e) =>
            {
                _selectedRuleId = listView_Rules.SelectedItems.Count > 0
                    ? (Guid)(listView_Rules.SelectedItems[0].Tag ?? Guid.Empty)
                    : Guid.Empty;
            };

            // ── OT Rules CRUD ──
            button_AddOT.Click    += async (s, e) => await OnAddOTRuleAsync();
            button_EditOT.Click   += async (s, e) => await OnEditOTRuleAsync();
            button_DeleteOT.Click += async (s, e) => await OnDeleteOTRuleAsync();
            listView_OT.SelectedIndexChanged += (s, e) =>
            {
                _selectedOTRuleId = listView_OT.SelectedItems.Count > 0
                    ? (Guid)(listView_OT.SelectedItems[0].Tag ?? Guid.Empty)
                    : Guid.Empty;
            };

            Load += AttendanceConfigControl_Load;
        }

        private async void AttendanceConfigControl_Load(object? sender, EventArgs e)
        {
            await LoadShiftsAsync();
            await LoadRulesAsync();
            await LoadOTRulesAsync();
        }

        // ══════════════════════════════════════
        //  LOAD
        // ══════════════════════════════════════
        private async Task LoadShiftsAsync()
        {
            try
            {
                var shifts = await Program.AttendanceApi.GetWorkShiftsAsync();
                listView_Shifts.BeginUpdate();
                listView_Shifts.Items.Clear();

                foreach (var shift in shifts)
                {
                    var item = new ListViewItem(shift.Name);
                    item.SubItems.Add(shift.StartTime.ToString("HH:mm"));
                    item.SubItems.Add(shift.EndTime.ToString("HH:mm"));
                    item.SubItems.Add($"{shift.BreakMins} phút");
                    item.SubItems.Add(shift.Description ?? "--");
                    item.SubItems.Add(shift.IsActive ? "Đang hoạt động" : "Tạm dừng");
                    item.Tag = shift.Id;

                    if (!shift.IsActive) item.ForeColor = Color.Gray;
                    listView_Shifts.Items.Add(item);
                }
                listView_Shifts.EndUpdate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể tải danh sách ca: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadRulesAsync()
        {
            try
            {
                var rules = await Program.AttendanceApi.GetAttendanceRulesAsync();
                listView_Rules.BeginUpdate();
                listView_Rules.Items.Clear();

                foreach (var r in rules)
                {
                    var item = new ListViewItem(r.RuleName);
                    item.SubItems.Add($"{r.AllowedLateMinutes} phút");
                    item.SubItems.Add($"{r.AllowedEarlyLeaveMinutes} phút");
                    item.SubItems.Add($"{r.MaxViolationsPerMonth} lần");
                    item.SubItems.Add(r.EffectiveDate.ToString("dd/MM/yyyy"));
                    item.SubItems.Add(r.IsActive ? "Đang áp dụng" : "Hết hiệu lực");
                    item.SubItems.Add(r.Description ?? "--");
                    item.Tag = r.Id;

                    if (!r.IsActive) item.ForeColor = Color.Gray;
                    listView_Rules.Items.Add(item);
                }
                listView_Rules.EndUpdate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể tải quy định chấm công: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadOTRulesAsync()
        {
            try
            {
                var otRules = await Program.AttendanceApi.GetOvertimeRulesAsync();
                listView_OT.BeginUpdate();
                listView_OT.Items.Clear();

                foreach (var r in otRules)
                {
                    var item = new ListViewItem(r.DayType);
                    item.SubItems.Add($"{r.Multiplier}x ({r.Multiplier * 100}%)");
                    item.SubItems.Add($"{r.MaxHoursPerDay}h");
                    item.SubItems.Add($"{r.MaxHoursPerMonth}h");
                    item.SubItems.Add(r.EffectiveDate.ToString("dd/MM/yyyy"));
                    item.SubItems.Add(r.IsActive ? "Đang áp dụng" : "Hết hiệu lực");
                    item.SubItems.Add(r.Description ?? "--");
                    item.Tag = r.Id;

                    if (!r.IsActive) item.ForeColor = Color.Gray;
                    listView_OT.Items.Add(item);
                }
                listView_OT.EndUpdate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể tải quy tắc OT: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ══════════════════════════════════════
        //  WORK SHIFTS
        // ══════════════════════════════════════
        private async Task OnAddShiftAsync()
        {
            using var dlg = new ShiftFormDialog();
            if (dlg.ShowDialog() != DialogResult.OK) return;
            try
            {
                await Program.AttendanceApi.CreateWorkShiftAsync(dlg.CreateDto!);
                await LoadShiftsAsync();
                MessageBox.Show("Thêm ca làm việc thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task OnEditShiftAsync()
        {
            if (_selectedShiftId == Guid.Empty)
            {
                MessageBox.Show("Vui lòng chọn một ca để sửa.", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedItem = listView_Shifts.SelectedItems[0];
            var current = new WorkShiftDto
            {
                Id        = _selectedShiftId,
                Name      = selectedItem.Text,
                StartTime = TimeOnly.Parse(selectedItem.SubItems[1].Text),
                EndTime   = TimeOnly.Parse(selectedItem.SubItems[2].Text),
                BreakMins = int.TryParse(selectedItem.SubItems[3].Text.Replace(" phút", ""), out var bm) ? bm : 60,
                Description = selectedItem.SubItems[4].Text == "--" ? "" : selectedItem.SubItems[4].Text,
                IsActive    = selectedItem.SubItems[5].Text == "Đang hoạt động"
            };

            using var dlg = new ShiftFormDialog(current);
            if (dlg.ShowDialog() != DialogResult.OK) return;
            try
            {
                await Program.AttendanceApi.UpdateWorkShiftAsync(_selectedShiftId, dlg.UpdateDto!);
                await LoadShiftsAsync();
                MessageBox.Show("Cập nhật ca làm việc thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task OnDeleteShiftAsync()
        {
            if (_selectedShiftId == Guid.Empty)
            {
                MessageBox.Show("Vui lòng chọn một ca để xóa.", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var name = listView_Shifts.SelectedItems[0].Text;
            var confirm = MessageBox.Show($"Bạn có chắc muốn xóa ca \"{name}\"?", "Xác nhận xóa",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;
            try
            {
                await Program.AttendanceApi.DeleteWorkShiftAsync(_selectedShiftId);
                await LoadShiftsAsync();
                MessageBox.Show("Đã xóa ca làm việc.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ══════════════════════════════════════
        //  ATTENDANCE RULES
        // ══════════════════════════════════════
        private async Task OnAddRuleAsync()
        {
            using var dlg = new AttendanceRuleFormDialog();
            if (dlg.ShowDialog() != DialogResult.OK) return;
            try
            {
                await Program.AttendanceApi.CreateAttendanceRuleAsync(dlg.CreateDto!);
                await LoadRulesAsync();
                MessageBox.Show("Thêm quy tắc chấm công thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task OnEditRuleAsync()
        {
            if (_selectedRuleId == Guid.Empty)
            {
                MessageBox.Show("Vui lòng chọn một quy tắc để sửa.", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var selectedItem = listView_Rules.SelectedItems[0];
            var current = new AttendanceRuleDto
            {
                Id                      = _selectedRuleId,
                RuleName                = selectedItem.Text,
                AllowedLateMinutes      = int.TryParse(selectedItem.SubItems[1].Text.Replace(" phút", ""), out var lm) ? lm : 15,
                AllowedEarlyLeaveMinutes = int.TryParse(selectedItem.SubItems[2].Text.Replace(" phút", ""), out var em) ? em : 0,
                MaxViolationsPerMonth   = int.TryParse(selectedItem.SubItems[3].Text.Replace(" lần", ""), out var mv) ? mv : 3,
                EffectiveDate           = DateOnly.TryParse(selectedItem.SubItems[4].Text, out var ed) ? ed : DateOnly.FromDateTime(DateTime.Today),
                IsActive                = selectedItem.SubItems[5].Text == "Đang áp dụng",
                Description             = selectedItem.SubItems[6].Text == "--" ? null : selectedItem.SubItems[6].Text
            };

            using var dlg = new AttendanceRuleFormDialog(current);
            if (dlg.ShowDialog() != DialogResult.OK) return;
            try
            {
                await Program.AttendanceApi.UpdateAttendanceRuleAsync(_selectedRuleId, dlg.UpdateDto!);
                await LoadRulesAsync();
                MessageBox.Show("Cập nhật quy tắc thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task OnDeleteRuleAsync()
        {
            if (_selectedRuleId == Guid.Empty)
            {
                MessageBox.Show("Vui lòng chọn một quy tắc để xóa.", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var name = listView_Rules.SelectedItems[0].Text;
            var confirm = MessageBox.Show($"Bạn có chắc muốn xóa quy tắc \"{name}\"?", "Xác nhận xóa",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;
            try
            {
                await Program.AttendanceApi.DeleteAttendanceRuleAsync(_selectedRuleId);
                await LoadRulesAsync();
                MessageBox.Show("Đã xóa quy tắc chấm công.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ══════════════════════════════════════
        //  OVERTIME RULES
        // ══════════════════════════════════════
        private async Task OnAddOTRuleAsync()
        {
            using var dlg = new OvertimeRuleFormDialog();
            if (dlg.ShowDialog() != DialogResult.OK) return;
            try
            {
                await Program.AttendanceApi.CreateOvertimeRuleAsync(dlg.CreateDto!);
                await LoadOTRulesAsync();
                MessageBox.Show("Thêm quy tắc OT thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task OnEditOTRuleAsync()
        {
            if (_selectedOTRuleId == Guid.Empty)
            {
                MessageBox.Show("Vui lòng chọn một quy tắc OT để sửa.", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var selectedItem = listView_OT.SelectedItems[0];
            var current = new OvertimeRuleDto
            {
                Id              = _selectedOTRuleId,
                DayType         = selectedItem.Text,
                Multiplier      = decimal.TryParse(selectedItem.SubItems[1].Text.Split('x')[0], out var mul) ? mul : 1.5m,
                MaxHoursPerDay  = decimal.TryParse(selectedItem.SubItems[2].Text.Replace("h", ""), out var mhd) ? mhd : 4m,
                MaxHoursPerMonth= decimal.TryParse(selectedItem.SubItems[3].Text.Replace("h", ""), out var mhm) ? mhm : 40m,
                EffectiveDate   = DateOnly.TryParse(selectedItem.SubItems[4].Text, out var ed) ? ed : DateOnly.FromDateTime(DateTime.Today),
                IsActive        = selectedItem.SubItems[5].Text == "Đang áp dụng",
                Description     = selectedItem.SubItems[6].Text == "--" ? null : selectedItem.SubItems[6].Text
            };

            using var dlg = new OvertimeRuleFormDialog(current);
            if (dlg.ShowDialog() != DialogResult.OK) return;
            try
            {
                await Program.AttendanceApi.UpdateOvertimeRuleAsync(_selectedOTRuleId, dlg.UpdateDto!);
                await LoadOTRulesAsync();
                MessageBox.Show("Cập nhật quy tắc OT thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task OnDeleteOTRuleAsync()
        {
            if (_selectedOTRuleId == Guid.Empty)
            {
                MessageBox.Show("Vui lòng chọn một quy tắc OT để xóa.", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var name = listView_OT.SelectedItems[0].Text;
            var confirm = MessageBox.Show($"Bạn có chắc muốn xóa quy tắc OT \"{name}\"?", "Xác nhận xóa",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;
            try
            {
                await Program.AttendanceApi.DeleteOvertimeRuleAsync(_selectedOTRuleId);
                await LoadOTRulesAsync();
                MessageBox.Show("Đã xóa quy tắc OT.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    // ═══════════════════════════════════════════════════════
    //  DIALOG: Work Shift Form
    // ═══════════════════════════════════════════════════════
    internal class ShiftFormDialog : Form
    {
        public CreateWorkShiftDto? CreateDto { get; private set; }
        public UpdateWorkShiftDto? UpdateDto { get; private set; }

        private readonly TextBox txt_Name = new() { Width = 300 };
        private readonly DateTimePicker dtp_Start = new() { Format = DateTimePickerFormat.Time, ShowUpDown = true, Width = 150 };
        private readonly DateTimePicker dtp_End   = new() { Format = DateTimePickerFormat.Time, ShowUpDown = true, Width = 150 };
        private readonly NumericUpDown  num_Break  = new() { Minimum = 0, Maximum = 240, Value = 60, Width = 100 };
        private readonly TextBox txt_Desc = new() { Width = 400, Height = 60, Multiline = true };
        private readonly CheckBox chk_Active = new() { Text = "Đang hoạt động", Checked = true };
        private readonly DateTimePicker dtp_Effective = new() { Format = DateTimePickerFormat.Short, Width = 160 };

        private readonly bool _isEdit;

        public ShiftFormDialog(WorkShiftDto? existing = null)
        {
            _isEdit = existing != null;
            Text = _isEdit ? "Sửa Ca làm việc" : "Thêm Ca làm việc";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(560, 420);
            MaximizeBox = MinimizeBox = false;

            if (existing != null)
            {
                txt_Name.Text = existing.Name;
                dtp_Start.Value = DateTime.Today.Add(existing.StartTime.ToTimeSpan());
                dtp_End.Value   = DateTime.Today.Add(existing.EndTime.ToTimeSpan());
                num_Break.Value = existing.BreakMins;
                txt_Desc.Text   = existing.Description ?? "";
                chk_Active.Checked = existing.IsActive;
                if (existing.EffectiveDate.HasValue)
                    dtp_Effective.Value = existing.EffectiveDate.Value.ToDateTime(TimeOnly.MinValue);
            }

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(16),
                RowCount = 8,
                ColumnCount = 2
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            void AddRow(string label, Control ctrl, int row)
            {
                layout.Controls.Add(new Label { Text = label, TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, row);
                layout.Controls.Add(ctrl, 1, row);
            }
            AddRow("Tên ca:", txt_Name, 0);
            AddRow("Giờ bắt đầu:", dtp_Start, 1);
            AddRow("Giờ kết thúc:", dtp_End, 2);
            AddRow("Nghỉ giữa ca (phút):", num_Break, 3);
            AddRow("Mô tả:", txt_Desc, 4);
            AddRow("Ngày hiệu lực:", dtp_Effective, 5);
            layout.Controls.Add(chk_Active, 1, 6);

            var btnOk = new Button { Text = "Lưu", DialogResult = DialogResult.OK, Width = 100, BackColor = Color.FromArgb(37, 99, 235), ForeColor = Color.White };
            var btnCancel = new Button { Text = "Hủy", DialogResult = DialogResult.Cancel, Width = 100 };
            btnOk.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txt_Name.Text)) { MessageBox.Show("Vui lòng nhập tên ca."); return; }
                var effective = DateOnly.FromDateTime(dtp_Effective.Value);
                if (_isEdit)
                {
                    UpdateDto = new UpdateWorkShiftDto
                    {
                        Name = txt_Name.Text.Trim(),
                        StartTime = TimeOnly.FromDateTime(dtp_Start.Value),
                        EndTime = TimeOnly.FromDateTime(dtp_End.Value),
                        BreakMins = (int)num_Break.Value,
                        Description = txt_Desc.Text.Trim(),
                        IsActive = chk_Active.Checked,
                        EffectiveDate = effective
                    };
                }
                else
                {
                    CreateDto = new CreateWorkShiftDto
                    {
                        Name = txt_Name.Text.Trim(),
                        StartTime = TimeOnly.FromDateTime(dtp_Start.Value),
                        EndTime = TimeOnly.FromDateTime(dtp_End.Value),
                        BreakMins = (int)num_Break.Value,
                        Description = txt_Desc.Text.Trim(),
                        IsActive = chk_Active.Checked,
                        EffectiveDate = effective
                    };
                }
                DialogResult = DialogResult.OK;
            };

            var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Bottom, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(8), Height = 50 };
            btnPanel.Controls.Add(btnCancel);
            btnPanel.Controls.Add(btnOk);

            Controls.Add(layout);
            Controls.Add(btnPanel);
            AcceptButton = btnOk;
            CancelButton = btnCancel;
        }
    }

    // ═══════════════════════════════════════════════════════
    //  DIALOG: Attendance Rule Form
    // ═══════════════════════════════════════════════════════
    internal class AttendanceRuleFormDialog : Form
    {
        public CreateAttendanceRuleDto? CreateDto { get; private set; }
        public UpdateAttendanceRuleDto? UpdateDto { get; private set; }

        private readonly TextBox txt_Name    = new() { Width = 300 };
        private readonly NumericUpDown num_Late  = new() { Minimum = 0, Maximum = 120, Value = 15, Width = 100 };
        private readonly NumericUpDown num_Early = new() { Minimum = 0, Maximum = 120, Value = 0,  Width = 100 };
        private readonly NumericUpDown num_Max   = new() { Minimum = 0, Maximum = 30,  Value = 3,  Width = 100 };
        private readonly TextBox txt_Desc    = new() { Width = 400, Height = 60, Multiline = true };
        private readonly CheckBox chk_Active = new() { Text = "Đang áp dụng", Checked = true };
        private readonly DateTimePicker dtp_Effective = new() { Format = DateTimePickerFormat.Short, Width = 160 };

        private readonly bool _isEdit;

        public AttendanceRuleFormDialog(AttendanceRuleDto? existing = null)
        {
            _isEdit = existing != null;
            Text = _isEdit ? "Sửa Quy tắc Chấm công" : "Thêm Quy tắc Chấm công";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(560, 420);
            MaximizeBox = MinimizeBox = false;

            if (existing != null)
            {
                txt_Name.Text  = existing.RuleName;
                num_Late.Value  = existing.AllowedLateMinutes;
                num_Early.Value = existing.AllowedEarlyLeaveMinutes;
                num_Max.Value   = existing.MaxViolationsPerMonth;
                txt_Desc.Text   = existing.Description ?? "";
                chk_Active.Checked = existing.IsActive;
                dtp_Effective.Value = existing.EffectiveDate.ToDateTime(TimeOnly.MinValue);
            }

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(16),
                RowCount = 8,
                ColumnCount = 2
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            void AddRow(string label, Control ctrl, int row)
            {
                layout.Controls.Add(new Label { Text = label, TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, row);
                layout.Controls.Add(ctrl, 1, row);
            }
            AddRow("Tên quy tắc:", txt_Name, 0);
            AddRow("Trễ cho phép (phút):", num_Late, 1);
            AddRow("Về sớm cho phép (phút):", num_Early, 2);
            AddRow("Vi phạm tối đa/tháng:", num_Max, 3);
            AddRow("Mô tả:", txt_Desc, 4);
            AddRow("Ngày hiệu lực:", dtp_Effective, 5);
            layout.Controls.Add(chk_Active, 1, 6);

            var btnOk = new Button { Text = "Lưu", DialogResult = DialogResult.OK, Width = 100, BackColor = Color.FromArgb(37, 99, 235), ForeColor = Color.White };
            var btnCancel = new Button { Text = "Hủy", DialogResult = DialogResult.Cancel, Width = 100 };
            btnOk.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txt_Name.Text)) { MessageBox.Show("Vui lòng nhập tên quy tắc."); return; }
                var effective = DateOnly.FromDateTime(dtp_Effective.Value);
                if (_isEdit)
                {
                    UpdateDto = new UpdateAttendanceRuleDto
                    {
                        RuleName = txt_Name.Text.Trim(),
                        AllowedLateMinutes = (int)num_Late.Value,
                        AllowedEarlyLeaveMinutes = (int)num_Early.Value,
                        MaxViolationsPerMonth = (int)num_Max.Value,
                        Description = txt_Desc.Text.Trim(),
                        IsActive = chk_Active.Checked,
                        EffectiveDate = effective,
                        UpdatedBy = "Admin"
                    };
                }
                else
                {
                    CreateDto = new CreateAttendanceRuleDto
                    {
                        RuleName = txt_Name.Text.Trim(),
                        AllowedLateMinutes = (int)num_Late.Value,
                        AllowedEarlyLeaveMinutes = (int)num_Early.Value,
                        MaxViolationsPerMonth = (int)num_Max.Value,
                        Description = txt_Desc.Text.Trim(),
                        IsActive = chk_Active.Checked,
                        EffectiveDate = effective,
                        CreatedBy = "Admin"
                    };
                }
                DialogResult = DialogResult.OK;
            };

            var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Bottom, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(8), Height = 50 };
            btnPanel.Controls.Add(btnCancel);
            btnPanel.Controls.Add(btnOk);

            Controls.Add(layout);
            Controls.Add(btnPanel);
            AcceptButton = btnOk;
            CancelButton = btnCancel;
        }
    }

    // ═══════════════════════════════════════════════════════
    //  DIALOG: Overtime Rule Form
    // ═══════════════════════════════════════════════════════
    internal class OvertimeRuleFormDialog : Form
    {
        public CreateOvertimeRuleDto? CreateDto { get; private set; }
        public UpdateOvertimeRuleDto? UpdateDto { get; private set; }

        private readonly ComboBox cmb_DayType = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 200 };
        private readonly NumericUpDown num_Multiplier = new() { Minimum = 1, Maximum = 5, DecimalPlaces = 1, Increment = 0.5m, Value = 1.5m, Width = 100 };
        private readonly NumericUpDown num_MaxDay     = new() { Minimum = 1, Maximum = 24, DecimalPlaces = 1, Increment = 0.5m, Value = 4, Width = 100 };
        private readonly NumericUpDown num_MaxMonth   = new() { Minimum = 1, Maximum = 200, Value = 40, Width = 100 };
        private readonly NumericUpDown num_MaxYear    = new() { Minimum = 1, Maximum = 500, Value = 200, Width = 100 };
        private readonly TextBox txt_Desc    = new() { Width = 400, Height = 60, Multiline = true };
        private readonly CheckBox chk_Active = new() { Text = "Đang áp dụng", Checked = true };
        private readonly DateTimePicker dtp_Effective = new() { Format = DateTimePickerFormat.Short, Width = 160 };

        private readonly bool _isEdit;

        public OvertimeRuleFormDialog(OvertimeRuleDto? existing = null)
        {
            _isEdit = existing != null;
            Text = _isEdit ? "Sửa Quy tắc OT" : "Thêm Quy tắc OT";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(560, 460);
            MaximizeBox = MinimizeBox = false;

            cmb_DayType.Items.AddRange(new[] { "WEEKDAY", "WEEKEND", "HOLIDAY" });

            if (existing != null)
            {
                cmb_DayType.SelectedItem = existing.DayType;
                num_Multiplier.Value = existing.Multiplier;
                num_MaxDay.Value    = existing.MaxHoursPerDay;
                num_MaxMonth.Value  = existing.MaxHoursPerMonth;
                num_MaxYear.Value   = existing.MaxHoursPerYear;
                txt_Desc.Text       = existing.Description ?? "";
                chk_Active.Checked  = existing.IsActive;
                dtp_Effective.Value = existing.EffectiveDate.ToDateTime(TimeOnly.MinValue);
            }
            else
            {
                cmb_DayType.SelectedIndex = 0;
            }

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(16),
                RowCount = 9,
                ColumnCount = 2
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            void AddRow(string label, Control ctrl, int row)
            {
                layout.Controls.Add(new Label { Text = label, TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, row);
                layout.Controls.Add(ctrl, 1, row);
            }
            AddRow("Loại ngày (DayType):", cmb_DayType, 0);
            AddRow("Hệ số lương OT:", num_Multiplier, 1);
            AddRow("Tối đa giờ/ngày:", num_MaxDay, 2);
            AddRow("Tối đa giờ/tháng:", num_MaxMonth, 3);
            AddRow("Tối đa giờ/năm:", num_MaxYear, 4);
            AddRow("Mô tả:", txt_Desc, 5);
            AddRow("Ngày hiệu lực:", dtp_Effective, 6);
            layout.Controls.Add(chk_Active, 1, 7);

            var btnOk = new Button { Text = "Lưu", DialogResult = DialogResult.OK, Width = 100, BackColor = Color.FromArgb(37, 99, 235), ForeColor = Color.White };
            var btnCancel = new Button { Text = "Hủy", DialogResult = DialogResult.Cancel, Width = 100 };
            btnOk.Click += (s, e) =>
            {
                if (cmb_DayType.SelectedItem == null) { MessageBox.Show("Vui lòng chọn loại ngày."); return; }
                var dayType = cmb_DayType.SelectedItem.ToString()!;
                var effective = DateOnly.FromDateTime(dtp_Effective.Value);
                if (_isEdit)
                {
                    UpdateDto = new UpdateOvertimeRuleDto
                    {
                        DayType         = dayType,
                        Multiplier      = num_Multiplier.Value,
                        MaxHoursPerDay  = num_MaxDay.Value,
                        MaxHoursPerMonth= num_MaxMonth.Value,
                        MaxHoursPerYear = num_MaxYear.Value,
                        Description     = txt_Desc.Text.Trim(),
                        IsActive        = chk_Active.Checked,
                        EffectiveDate   = effective,
                        UpdatedBy       = "Admin"
                    };
                }
                else
                {
                    CreateDto = new CreateOvertimeRuleDto
                    {
                        DayType         = dayType,
                        Multiplier      = num_Multiplier.Value,
                        MaxHoursPerDay  = num_MaxDay.Value,
                        MaxHoursPerMonth= num_MaxMonth.Value,
                        MaxHoursPerYear = num_MaxYear.Value,
                        Description     = txt_Desc.Text.Trim(),
                        IsActive        = chk_Active.Checked,
                        EffectiveDate   = effective,
                        CreatedBy       = "Admin"
                    };
                }
                DialogResult = DialogResult.OK;
            };

            var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Bottom, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(8), Height = 50 };
            btnPanel.Controls.Add(btnCancel);
            btnPanel.Controls.Add(btnOk);

            Controls.Add(layout);
            Controls.Add(btnPanel);
            AcceptButton = btnOk;
            CancelButton = btnCancel;
        }
    }
}

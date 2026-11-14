namespace Admin.WinForms.Controls.Attendance
{
    partial class AttendanceConfigControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panel_Container = new Panel();
            tabControl_Config = new TabControl();
            tabPage_Shifts = new TabPage();
            label_ShiftTitle = new Label();
            button_ReloadShifts = new Button();
            button_AddShift = new Button();
            button_EditShift = new Button();
            button_DeleteShift = new Button();
            listView_Shifts = new ListView();
            col_Shift_Name = new ColumnHeader();
            col_Shift_StartTime = new ColumnHeader();
            col_Shift_EndTime = new ColumnHeader();
            col_Shift_Break = new ColumnHeader();
            col_Shift_Desc = new ColumnHeader();
            col_Shift_Active = new ColumnHeader();
            tabPage_Rules = new TabPage();
            label_RulesTitle = new Label();
            button_ReloadRules = new Button();
            button_AddRule = new Button();
            button_EditRule = new Button();
            button_DeleteRule = new Button();
            listView_Rules = new ListView();
            col_Rule_Name = new ColumnHeader();
            col_Rule_LateMins = new ColumnHeader();
            col_Rule_EarlyMins = new ColumnHeader();
            col_Rule_MaxViolations = new ColumnHeader();
            col_Rule_Effective = new ColumnHeader();
            col_Rule_Active = new ColumnHeader();
            col_Rule_Desc = new ColumnHeader();
            tabPage_OT = new TabPage();
            label_OTTitle = new Label();
            button_ReloadOT = new Button();
            button_AddOT = new Button();
            button_EditOT = new Button();
            button_DeleteOT = new Button();
            listView_OT = new ListView();
            col_OT_DayType = new ColumnHeader();
            col_OT_Multiplier = new ColumnHeader();
            col_OT_MaxDay = new ColumnHeader();
            col_OT_MaxMonth = new ColumnHeader();
            col_OT_Effective = new ColumnHeader();
            col_OT_Active = new ColumnHeader();
            col_OT_Desc = new ColumnHeader();
            panel_Container.SuspendLayout();
            tabControl_Config.SuspendLayout();
            tabPage_Shifts.SuspendLayout();
            tabPage_Rules.SuspendLayout();
            tabPage_OT.SuspendLayout();
            SuspendLayout();
            // 
            // panel_Container
            // 
            panel_Container.Controls.Add(tabControl_Config);
            panel_Container.Dock = DockStyle.Fill;
            panel_Container.Location = new Point(0, 0);
            panel_Container.Name = "panel_Container";
            panel_Container.Size = new Size(1815, 884);
            panel_Container.TabIndex = 0;
            // 
            // tabControl_Config
            // 
            tabControl_Config.Controls.Add(tabPage_Shifts);
            tabControl_Config.Controls.Add(tabPage_Rules);
            tabControl_Config.Controls.Add(tabPage_OT);
            tabControl_Config.Dock = DockStyle.Fill;
            tabControl_Config.Font = new Font("Segoe UI", 10F);
            tabControl_Config.Location = new Point(0, 0);
            tabControl_Config.Name = "tabControl_Config";
            tabControl_Config.SelectedIndex = 0;
            tabControl_Config.Size = new Size(1815, 884);
            tabControl_Config.TabIndex = 0;
            // 
            // tabPage_Shifts
            // 
            tabPage_Shifts.Controls.Add(label_ShiftTitle);
            tabPage_Shifts.Controls.Add(button_ReloadShifts);
            tabPage_Shifts.Controls.Add(button_AddShift);
            tabPage_Shifts.Controls.Add(button_EditShift);
            tabPage_Shifts.Controls.Add(button_DeleteShift);
            tabPage_Shifts.Controls.Add(listView_Shifts);
            tabPage_Shifts.Location = new Point(8, 45);
            tabPage_Shifts.Name = "tabPage_Shifts";
            tabPage_Shifts.Padding = new Padding(3);
            tabPage_Shifts.Size = new Size(1799, 831);
            tabPage_Shifts.TabIndex = 0;
            tabPage_Shifts.Text = "Ca làm việc (Work Shifts)";
            tabPage_Shifts.UseVisualStyleBackColor = true;
            // 
            // label_ShiftTitle
            // 
            label_ShiftTitle.AutoSize = true;
            label_ShiftTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            label_ShiftTitle.Location = new Point(10, 15);
            label_ShiftTitle.Name = "label_ShiftTitle";
            label_ShiftTitle.Size = new Size(320, 30);
            label_ShiftTitle.TabIndex = 2;
            label_ShiftTitle.Text = "Danh sách Ca làm việc chuẩn";
            // 
            // button_ReloadShifts
            // 
            button_ReloadShifts.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_ReloadShifts.Font = new Font("Segoe UI", 9F);
            button_ReloadShifts.Location = new Point(1680, 8);
            button_ReloadShifts.Name = "button_ReloadShifts";
            button_ReloadShifts.Size = new Size(110, 42);
            button_ReloadShifts.TabIndex = 1;
            button_ReloadShifts.Text = "Reload";
            button_ReloadShifts.UseVisualStyleBackColor = true;
            // 
            // button_AddShift
            // 
            button_AddShift.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_AddShift.BackColor = Color.FromArgb(37, 99, 235);
            button_AddShift.ForeColor = Color.White;
            button_AddShift.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button_AddShift.Location = new Point(1200, 8);
            button_AddShift.Name = "button_AddShift";
            button_AddShift.Size = new Size(145, 42);
            button_AddShift.TabIndex = 3;
            button_AddShift.Text = "+ Thêm Ca";
            button_AddShift.UseVisualStyleBackColor = false;
            // 
            // button_EditShift
            // 
            button_EditShift.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_EditShift.BackColor = Color.FromArgb(245, 158, 11);
            button_EditShift.ForeColor = Color.White;
            button_EditShift.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button_EditShift.Location = new Point(1355, 8);
            button_EditShift.Name = "button_EditShift";
            button_EditShift.Size = new Size(145, 42);
            button_EditShift.TabIndex = 4;
            button_EditShift.Text = "✏ Sửa";
            button_EditShift.UseVisualStyleBackColor = false;
            // 
            // button_DeleteShift
            // 
            button_DeleteShift.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_DeleteShift.BackColor = Color.FromArgb(239, 68, 68);
            button_DeleteShift.ForeColor = Color.White;
            button_DeleteShift.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button_DeleteShift.Location = new Point(1510, 8);
            button_DeleteShift.Name = "button_DeleteShift";
            button_DeleteShift.Size = new Size(145, 42);
            button_DeleteShift.TabIndex = 5;
            button_DeleteShift.Text = "🗑 Xóa";
            button_DeleteShift.UseVisualStyleBackColor = false;
            // 
            // listView_Shifts
            // 
            listView_Shifts.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listView_Shifts.BackColor = Color.White;
            listView_Shifts.BorderStyle = BorderStyle.FixedSingle;
            listView_Shifts.Columns.AddRange(new ColumnHeader[] { col_Shift_Name, col_Shift_StartTime, col_Shift_EndTime, col_Shift_Break, col_Shift_Desc, col_Shift_Active });
            listView_Shifts.Font = new Font("Segoe UI", 9F);
            listView_Shifts.ForeColor = Color.FromArgb(37, 48, 74);
            listView_Shifts.FullRowSelect = true;
            listView_Shifts.GridLines = true;
            listView_Shifts.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            listView_Shifts.Location = new Point(6, 56);
            listView_Shifts.Name = "listView_Shifts";
            listView_Shifts.Size = new Size(1787, 765);
            listView_Shifts.TabIndex = 0;
            listView_Shifts.UseCompatibleStateImageBehavior = false;
            listView_Shifts.View = View.Details;
            // 
            // col_Shift_Name
            // 
            col_Shift_Name.Text = "Tên Ca";
            col_Shift_Name.Width = 250;
            // 
            // col_Shift_StartTime
            // 
            col_Shift_StartTime.Text = "Giờ bắt đầu";
            col_Shift_StartTime.Width = 140;
            // 
            // col_Shift_EndTime
            // 
            col_Shift_EndTime.Text = "Giờ kết thúc";
            col_Shift_EndTime.Width = 140;
            // 
            // col_Shift_Break
            // 
            col_Shift_Break.Text = "Nghỉ giữa ca (phút)";
            col_Shift_Break.Width = 180;
            // 
            // col_Shift_Desc
            // 
            col_Shift_Desc.Text = "Mô tả";
            col_Shift_Desc.Width = 350;
            // 
            // col_Shift_Active
            // 
            col_Shift_Active.Text = "Đang áp dụng";
            col_Shift_Active.Width = 150;
            // 
            // tabPage_Rules
            // 
            tabPage_Rules.Controls.Add(label_RulesTitle);
            tabPage_Rules.Controls.Add(button_ReloadRules);
            tabPage_Rules.Controls.Add(button_AddRule);
            tabPage_Rules.Controls.Add(button_EditRule);
            tabPage_Rules.Controls.Add(button_DeleteRule);
            tabPage_Rules.Controls.Add(listView_Rules);
            tabPage_Rules.Location = new Point(8, 45);
            tabPage_Rules.Name = "tabPage_Rules";
            tabPage_Rules.Padding = new Padding(3);
            tabPage_Rules.Size = new Size(1799, 831);
            tabPage_Rules.TabIndex = 1;
            tabPage_Rules.Text = "Quy định Đi trễ / Về sớm (Attendance Rules)";
            tabPage_Rules.UseVisualStyleBackColor = true;
            // 
            // label_RulesTitle
            // 
            label_RulesTitle.AutoSize = true;
            label_RulesTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            label_RulesTitle.Location = new Point(10, 15);
            label_RulesTitle.Name = "label_RulesTitle";
            label_RulesTitle.Size = new Size(390, 30);
            label_RulesTitle.TabIndex = 2;
            label_RulesTitle.Text = "Quy tắc Chấm công & Hạn mức vi phạm";
            // 
            // button_ReloadRules
            // 
            button_ReloadRules.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_ReloadRules.Font = new Font("Segoe UI", 9F);
            button_ReloadRules.Location = new Point(1680, 8);
            button_ReloadRules.Name = "button_ReloadRules";
            button_ReloadRules.Size = new Size(110, 42);
            button_ReloadRules.TabIndex = 1;
            button_ReloadRules.Text = "Reload";
            button_ReloadRules.UseVisualStyleBackColor = true;
            // 
            // button_AddRule
            // 
            button_AddRule.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_AddRule.BackColor = Color.FromArgb(37, 99, 235);
            button_AddRule.ForeColor = Color.White;
            button_AddRule.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button_AddRule.Location = new Point(1200, 8);
            button_AddRule.Name = "button_AddRule";
            button_AddRule.Size = new Size(145, 42);
            button_AddRule.TabIndex = 3;
            button_AddRule.Text = "+ Thêm Quy tắc";
            button_AddRule.UseVisualStyleBackColor = false;
            // 
            // button_EditRule
            // 
            button_EditRule.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_EditRule.BackColor = Color.FromArgb(245, 158, 11);
            button_EditRule.ForeColor = Color.White;
            button_EditRule.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button_EditRule.Location = new Point(1355, 8);
            button_EditRule.Name = "button_EditRule";
            button_EditRule.Size = new Size(145, 42);
            button_EditRule.TabIndex = 4;
            button_EditRule.Text = "✏ Sửa";
            button_EditRule.UseVisualStyleBackColor = false;
            // 
            // button_DeleteRule
            // 
            button_DeleteRule.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_DeleteRule.BackColor = Color.FromArgb(239, 68, 68);
            button_DeleteRule.ForeColor = Color.White;
            button_DeleteRule.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button_DeleteRule.Location = new Point(1510, 8);
            button_DeleteRule.Name = "button_DeleteRule";
            button_DeleteRule.Size = new Size(145, 42);
            button_DeleteRule.TabIndex = 5;
            button_DeleteRule.Text = "🗑 Xóa";
            button_DeleteRule.UseVisualStyleBackColor = false;
            // 
            // listView_Rules
            // 
            listView_Rules.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listView_Rules.BackColor = Color.White;
            listView_Rules.BorderStyle = BorderStyle.FixedSingle;
            listView_Rules.Columns.AddRange(new ColumnHeader[] { col_Rule_Name, col_Rule_LateMins, col_Rule_EarlyMins, col_Rule_MaxViolations, col_Rule_Effective, col_Rule_Active, col_Rule_Desc });
            listView_Rules.Font = new Font("Segoe UI", 9F);
            listView_Rules.ForeColor = Color.FromArgb(37, 48, 74);
            listView_Rules.FullRowSelect = true;
            listView_Rules.GridLines = true;
            listView_Rules.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            listView_Rules.Location = new Point(6, 56);
            listView_Rules.Name = "listView_Rules";
            listView_Rules.Size = new Size(1787, 765);
            listView_Rules.TabIndex = 0;
            listView_Rules.UseCompatibleStateImageBehavior = false;
            listView_Rules.View = View.Details;
            // 
            // col_Rule_Name
            // 
            col_Rule_Name.Text = "Tên Quy định";
            col_Rule_Name.Width = 260;
            // 
            // col_Rule_LateMins
            // 
            col_Rule_LateMins.Text = "Trễ cho phép (phút)";
            col_Rule_LateMins.Width = 180;
            // 
            // col_Rule_EarlyMins
            // 
            col_Rule_EarlyMins.Text = "Về sớm cho phép (phút)";
            col_Rule_EarlyMins.Width = 200;
            // 
            // col_Rule_MaxViolations
            // 
            col_Rule_MaxViolations.Text = "Vi phạm tối đa/tháng";
            col_Rule_MaxViolations.Width = 180;
            // 
            // col_Rule_Effective
            // 
            col_Rule_Effective.Text = "Ngày hiệu lực";
            col_Rule_Effective.Width = 150;
            // 
            // col_Rule_Active
            // 
            col_Rule_Active.Text = "Đang áp dụng";
            col_Rule_Active.Width = 150;
            // 
            // col_Rule_Desc
            // 
            col_Rule_Desc.Text = "Mô tả";
            col_Rule_Desc.Width = 350;
            // 
            // tabPage_OT
            // 
            tabPage_OT.Controls.Add(label_OTTitle);
            tabPage_OT.Controls.Add(button_ReloadOT);
            tabPage_OT.Controls.Add(button_AddOT);
            tabPage_OT.Controls.Add(button_EditOT);
            tabPage_OT.Controls.Add(button_DeleteOT);
            tabPage_OT.Controls.Add(listView_OT);
            tabPage_OT.Location = new Point(8, 45);
            tabPage_OT.Name = "tabPage_OT";
            tabPage_OT.Padding = new Padding(3);
            tabPage_OT.Size = new Size(1799, 831);
            tabPage_OT.TabIndex = 2;
            tabPage_OT.Text = "Quy tắc Tăng ca & Hệ số (Overtime Rules)";
            tabPage_OT.UseVisualStyleBackColor = true;
            // 
            // label_OTTitle
            // 
            label_OTTitle.AutoSize = true;
            label_OTTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            label_OTTitle.Location = new Point(10, 15);
            label_OTTitle.Name = "label_OTTitle";
            label_OTTitle.Size = new Size(380, 30);
            label_OTTitle.TabIndex = 2;
            label_OTTitle.Text = "Hệ số Lương Tăng ca (OT Multipliers)";
            // 
            // button_ReloadOT
            // 
            button_ReloadOT.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_ReloadOT.Font = new Font("Segoe UI", 9F);
            button_ReloadOT.Location = new Point(1680, 8);
            button_ReloadOT.Name = "button_ReloadOT";
            button_ReloadOT.Size = new Size(110, 42);
            button_ReloadOT.TabIndex = 1;
            button_ReloadOT.Text = "Reload";
            button_ReloadOT.UseVisualStyleBackColor = true;
            // 
            // button_AddOT
            // 
            button_AddOT.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_AddOT.BackColor = Color.FromArgb(37, 99, 235);
            button_AddOT.ForeColor = Color.White;
            button_AddOT.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button_AddOT.Location = new Point(1200, 8);
            button_AddOT.Name = "button_AddOT";
            button_AddOT.Size = new Size(145, 42);
            button_AddOT.TabIndex = 3;
            button_AddOT.Text = "+ Thêm Quy tắc OT";
            button_AddOT.UseVisualStyleBackColor = false;
            // 
            // button_EditOT
            // 
            button_EditOT.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_EditOT.BackColor = Color.FromArgb(245, 158, 11);
            button_EditOT.ForeColor = Color.White;
            button_EditOT.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button_EditOT.Location = new Point(1355, 8);
            button_EditOT.Name = "button_EditOT";
            button_EditOT.Size = new Size(145, 42);
            button_EditOT.TabIndex = 4;
            button_EditOT.Text = "✏ Sửa";
            button_EditOT.UseVisualStyleBackColor = false;
            // 
            // button_DeleteOT
            // 
            button_DeleteOT.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_DeleteOT.BackColor = Color.FromArgb(239, 68, 68);
            button_DeleteOT.ForeColor = Color.White;
            button_DeleteOT.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button_DeleteOT.Location = new Point(1510, 8);
            button_DeleteOT.Name = "button_DeleteOT";
            button_DeleteOT.Size = new Size(145, 42);
            button_DeleteOT.TabIndex = 5;
            button_DeleteOT.Text = "🗑 Xóa";
            button_DeleteOT.UseVisualStyleBackColor = false;
            // 
            // listView_OT
            // 
            listView_OT.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listView_OT.BackColor = Color.White;
            listView_OT.BorderStyle = BorderStyle.FixedSingle;
            listView_OT.Columns.AddRange(new ColumnHeader[] { col_OT_DayType, col_OT_Multiplier, col_OT_MaxDay, col_OT_MaxMonth, col_OT_Effective, col_OT_Active, col_OT_Desc });
            listView_OT.Font = new Font("Segoe UI", 9F);
            listView_OT.ForeColor = Color.FromArgb(37, 48, 74);
            listView_OT.FullRowSelect = true;
            listView_OT.GridLines = true;
            listView_OT.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            listView_OT.Location = new Point(6, 56);
            listView_OT.Name = "listView_OT";
            listView_OT.Size = new Size(1787, 765);
            listView_OT.TabIndex = 0;
            listView_OT.UseCompatibleStateImageBehavior = false;
            listView_OT.View = View.Details;
            // 
            // col_OT_DayType
            // 
            col_OT_DayType.Text = "Loại ngày (DayType)";
            col_OT_DayType.Width = 200;
            // 
            // col_OT_Multiplier
            // 
            col_OT_Multiplier.Text = "Hệ số lương OT";
            col_OT_Multiplier.Width = 160;
            // 
            // col_OT_MaxDay
            // 
            col_OT_MaxDay.Text = "Tối đa giờ/ngày";
            col_OT_MaxDay.Width = 150;
            // 
            // col_OT_MaxMonth
            // 
            col_OT_MaxMonth.Text = "Tối đa giờ/tháng";
            col_OT_MaxMonth.Width = 150;
            // 
            // col_OT_Effective
            // 
            col_OT_Effective.Text = "Ngày hiệu lực";
            col_OT_Effective.Width = 150;
            // 
            // col_OT_Active
            // 
            col_OT_Active.Text = "Đang áp dụng";
            col_OT_Active.Width = 150;
            // 
            // col_OT_Desc
            // 
            col_OT_Desc.Text = "Mô tả";
            col_OT_Desc.Width = 350;
            // 
            // AttendanceConfigControl
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(panel_Container);
            Name = "AttendanceConfigControl";
            Size = new Size(1815, 884);
            panel_Container.ResumeLayout(false);
            tabControl_Config.ResumeLayout(false);
            tabPage_Shifts.ResumeLayout(false);
            tabPage_Shifts.PerformLayout();
            tabPage_Rules.ResumeLayout(false);
            tabPage_Rules.PerformLayout();
            tabPage_OT.ResumeLayout(false);
            tabPage_OT.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel_Container;
        private TabControl tabControl_Config;
        private TabPage tabPage_Shifts;
        private Label label_ShiftTitle;
        private Button button_ReloadShifts;
        private Button button_AddShift;
        private Button button_EditShift;
        private Button button_DeleteShift;
        private ListView listView_Shifts;
        private ColumnHeader col_Shift_Name;
        private ColumnHeader col_Shift_StartTime;
        private ColumnHeader col_Shift_EndTime;
        private ColumnHeader col_Shift_Break;
        private ColumnHeader col_Shift_Desc;
        private ColumnHeader col_Shift_Active;

        private TabPage tabPage_Rules;
        private Label label_RulesTitle;
        private Button button_ReloadRules;
        private Button button_AddRule;
        private Button button_EditRule;
        private Button button_DeleteRule;
        private ListView listView_Rules;
        private ColumnHeader col_Rule_Name;
        private ColumnHeader col_Rule_LateMins;
        private ColumnHeader col_Rule_EarlyMins;
        private ColumnHeader col_Rule_MaxViolations;
        private ColumnHeader col_Rule_Effective;
        private ColumnHeader col_Rule_Active;
        private ColumnHeader col_Rule_Desc;

        private TabPage tabPage_OT;
        private Label label_OTTitle;
        private Button button_ReloadOT;
        private Button button_AddOT;
        private Button button_EditOT;
        private Button button_DeleteOT;
        private ListView listView_OT;
        private ColumnHeader col_OT_DayType;
        private ColumnHeader col_OT_Multiplier;
        private ColumnHeader col_OT_MaxDay;
        private ColumnHeader col_OT_MaxMonth;
        private ColumnHeader col_OT_Effective;
        private ColumnHeader col_OT_Active;
        private ColumnHeader col_OT_Desc;
    }
}

namespace Admin.WinForms.Controls.Attendance
{
    partial class AttendanceCheckInControl
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            panel_Header = new Panel();
            label_Title = new Label();
            label_SubTitle = new Label();
            label_CurrentClock = new Label();

            splitContainer_Main = new SplitContainer();
            groupBox_EmpLookup = new GroupBox();
            label_InputPrompt = new Label();
            textBox_EmpCode = new TextBox();
            button_Lookup = new Button();
            label_OrSelect = new Label();
            comboBox_Employees = new ComboBox();

            label_EmpCode = new Label();
            textBox_EmpCodeDisplay = new TextBox();
            label_FullName = new Label();
            textBox_FullName = new TextBox();
            label_Dept = new Label();
            textBox_Department = new TextBox();
            label_Position = new Label();
            textBox_Position = new TextBox();
            label_Shift = new Label();
            textBox_Shift = new TextBox();

            panel_ActionBox = new Panel();
            label_StatusToday = new Label();
            button_CheckIn = new Button();
            button_CheckOut = new Button();

            groupBox_RecentLogs = new GroupBox();
            listView_RecentLogs = new ListView();
            col_Log_Code = new ColumnHeader();
            col_Log_Name = new ColumnHeader();
            col_Log_Date = new ColumnHeader();
            col_Log_TimeIn = new ColumnHeader();
            col_Log_TimeOut = new ColumnHeader();
            col_Log_Hours = new ColumnHeader();
            col_Log_Status = new ColumnHeader();

            panel_Header.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer_Main).BeginInit();
            splitContainer_Main.Panel1.SuspendLayout();
            splitContainer_Main.Panel2.SuspendLayout();
            splitContainer_Main.SuspendLayout();
            groupBox_EmpLookup.SuspendLayout();
            panel_ActionBox.SuspendLayout();
            groupBox_RecentLogs.SuspendLayout();
            SuspendLayout();

            // 
            // panel_Header
            // 
            panel_Header.BackColor = Color.FromArgb(30, 58, 138);
            panel_Header.Controls.Add(label_CurrentClock);
            panel_Header.Controls.Add(label_SubTitle);
            panel_Header.Controls.Add(label_Title);
            panel_Header.Dock = DockStyle.Top;
            panel_Header.Location = new Point(0, 0);
            panel_Header.Name = "panel_Header";
            panel_Header.Size = new Size(1800, 90);
            panel_Header.TabIndex = 0;

            label_Title.AutoSize = true;
            label_Title.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            label_Title.ForeColor = Color.White;
            label_Title.Location = new Point(25, 12);
            label_Title.Name = "label_Title";
            label_Title.Size = new Size(540, 45);
            label_Title.Text = "TRẠM CHẤM CÔNG (KIOSK TERMINAL)";

            label_SubTitle.AutoSize = true;
            label_SubTitle.Font = new Font("Segoe UI", 10F);
            label_SubTitle.ForeColor = Color.FromArgb(191, 219, 254);
            label_SubTitle.Location = new Point(28, 55);
            label_SubTitle.Name = "label_SubTitle";
            label_SubTitle.Size = new Size(580, 28);
            label_SubTitle.Text = "Nhập mã nhân viên hoặc quẹt mã để Check-in / Check-out trong ngày";

            label_CurrentClock.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label_CurrentClock.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            label_CurrentClock.ForeColor = Color.FromArgb(254, 240, 138);
            label_CurrentClock.Location = new Point(1400, 15);
            label_CurrentClock.Name = "label_CurrentClock";
            label_CurrentClock.Size = new Size(380, 55);
            label_CurrentClock.Text = "00:00:00";
            label_CurrentClock.TextAlign = ContentAlignment.MiddleRight;

            // 
            // splitContainer_Main
            // 
            splitContainer_Main.Dock = DockStyle.Fill;
            splitContainer_Main.Location = new Point(0, 90);
            splitContainer_Main.Name = "splitContainer_Main";
            splitContainer_Main.Size = new Size(1800, 790);
            splitContainer_Main.SplitterDistance = 750;
            splitContainer_Main.TabIndex = 1;
            splitContainer_Main.Panel1.Controls.Add(groupBox_EmpLookup);
            splitContainer_Main.Panel2.Controls.Add(groupBox_RecentLogs);

            // 
            // groupBox_EmpLookup
            // 
            groupBox_EmpLookup.BackColor = Color.White;
            groupBox_EmpLookup.Controls.Add(panel_ActionBox);
            groupBox_EmpLookup.Controls.Add(label_Shift);
            groupBox_EmpLookup.Controls.Add(textBox_Shift);
            groupBox_EmpLookup.Controls.Add(label_Position);
            groupBox_EmpLookup.Controls.Add(textBox_Position);
            groupBox_EmpLookup.Controls.Add(label_Dept);
            groupBox_EmpLookup.Controls.Add(textBox_Department);
            groupBox_EmpLookup.Controls.Add(label_FullName);
            groupBox_EmpLookup.Controls.Add(textBox_FullName);
            groupBox_EmpLookup.Controls.Add(label_EmpCode);
            groupBox_EmpLookup.Controls.Add(textBox_EmpCodeDisplay);
            groupBox_EmpLookup.Controls.Add(comboBox_Employees);
            groupBox_EmpLookup.Controls.Add(label_OrSelect);
            groupBox_EmpLookup.Controls.Add(button_Lookup);
            groupBox_EmpLookup.Controls.Add(textBox_EmpCode);
            groupBox_EmpLookup.Controls.Add(label_InputPrompt);
            groupBox_EmpLookup.Dock = DockStyle.Fill;
            groupBox_EmpLookup.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            groupBox_EmpLookup.ForeColor = Color.FromArgb(31, 41, 55);
            groupBox_EmpLookup.Location = new Point(0, 0);
            groupBox_EmpLookup.Name = "groupBox_EmpLookup";
            groupBox_EmpLookup.Padding = new Padding(20);
            groupBox_EmpLookup.Size = new Size(750, 790);
            groupBox_EmpLookup.TabIndex = 0;
            groupBox_EmpLookup.TabStop = false;
            groupBox_EmpLookup.Text = "1. Nhập Mã & Xác Nhận Nhân Viên";

            label_InputPrompt.AutoSize = true;
            label_InputPrompt.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label_InputPrompt.Location = new Point(25, 45);
            label_InputPrompt.Name = "label_InputPrompt";
            label_InputPrompt.Size = new Size(198, 28);
            label_InputPrompt.Text = "Nhập Mã Nhân Viên:";

            textBox_EmpCode.CharacterCasing = CharacterCasing.Upper;
            textBox_EmpCode.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            textBox_EmpCode.Location = new Point(25, 78);
            textBox_EmpCode.Name = "textBox_EmpCode";
            textBox_EmpCode.Size = new Size(320, 42);
            textBox_EmpCode.TabIndex = 0;

            button_Lookup.BackColor = Color.FromArgb(37, 99, 235);
            button_Lookup.ForeColor = Color.White;
            button_Lookup.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            button_Lookup.Location = new Point(360, 76);
            button_Lookup.Name = "button_Lookup";
            button_Lookup.Size = new Size(160, 46);
            button_Lookup.TabIndex = 1;
            button_Lookup.Text = "🔍 Tra cứu";
            button_Lookup.UseVisualStyleBackColor = false;

            label_OrSelect.AutoSize = true;
            label_OrSelect.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            label_OrSelect.ForeColor = Color.FromArgb(107, 114, 128);
            label_OrSelect.Location = new Point(25, 130);
            label_OrSelect.Name = "label_OrSelect";
            label_OrSelect.Size = new Size(245, 25);
            label_OrSelect.Text = "Hoặc chọn nhanh từ danh mục:";

            comboBox_Employees.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_Employees.Font = new Font("Segoe UI", 10F);
            comboBox_Employees.FormattingEnabled = true;
            comboBox_Employees.Location = new Point(25, 160);
            comboBox_Employees.Name = "comboBox_Employees";
            comboBox_Employees.Size = new Size(495, 36);
            comboBox_Employees.TabIndex = 2;

            // Thông tin nhân viên tự động hiển thị ra các ô text
            label_EmpCode.AutoSize = true;
            label_EmpCode.Font = new Font("Segoe UI", 9.5F);
            label_EmpCode.Location = new Point(25, 220);
            label_EmpCode.Name = "label_EmpCode";
            label_EmpCode.Size = new Size(133, 25);
            label_EmpCode.Text = "Mã Nhân Viên:";

            textBox_EmpCodeDisplay.BackColor = Color.FromArgb(243, 244, 246);
            textBox_EmpCodeDisplay.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            textBox_EmpCodeDisplay.Location = new Point(200, 216);
            textBox_EmpCodeDisplay.Name = "textBox_EmpCodeDisplay";
            textBox_EmpCodeDisplay.ReadOnly = true;
            textBox_EmpCodeDisplay.Size = new Size(320, 34);
            textBox_EmpCodeDisplay.TabIndex = 3;

            label_FullName.AutoSize = true;
            label_FullName.Font = new Font("Segoe UI", 9.5F);
            label_FullName.Location = new Point(25, 270);
            label_FullName.Name = "label_FullName";
            label_FullName.Size = new Size(100, 25);
            label_FullName.Text = "Họ và Tên:";

            textBox_FullName.BackColor = Color.FromArgb(243, 244, 246);
            textBox_FullName.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            textBox_FullName.ForeColor = Color.FromArgb(30, 58, 138);
            textBox_FullName.Location = new Point(200, 266);
            textBox_FullName.Name = "textBox_FullName";
            textBox_FullName.ReadOnly = true;
            textBox_FullName.Size = new Size(320, 34);
            textBox_FullName.TabIndex = 4;

            label_Dept.AutoSize = true;
            label_Dept.Font = new Font("Segoe UI", 9.5F);
            label_Dept.Location = new Point(25, 320);
            label_Dept.Name = "label_Dept";
            label_Dept.Size = new Size(106, 25);
            label_Dept.Text = "Phòng Ban:";

            textBox_Department.BackColor = Color.FromArgb(243, 244, 246);
            textBox_Department.Font = new Font("Segoe UI", 10F);
            textBox_Department.Location = new Point(200, 316);
            textBox_Department.Name = "textBox_Department";
            textBox_Department.ReadOnly = true;
            textBox_Department.Size = new Size(320, 34);
            textBox_Department.TabIndex = 5;

            label_Position.AutoSize = true;
            label_Position.Font = new Font("Segoe UI", 9.5F);
            label_Position.Location = new Point(25, 370);
            label_Position.Name = "label_Position";
            label_Position.Size = new Size(84, 25);
            label_Position.Text = "Chức Vụ:";

            textBox_Position.BackColor = Color.FromArgb(243, 244, 246);
            textBox_Position.Font = new Font("Segoe UI", 10F);
            textBox_Position.Location = new Point(200, 366);
            textBox_Position.Name = "textBox_Position";
            textBox_Position.ReadOnly = true;
            textBox_Position.Size = new Size(320, 34);
            textBox_Position.TabIndex = 6;

            label_Shift.AutoSize = true;
            label_Shift.Font = new Font("Segoe UI", 9.5F);
            label_Shift.Location = new Point(25, 420);
            label_Shift.Name = "label_Shift";
            label_Shift.Size = new Size(116, 25);
            label_Shift.Text = "Ca Làm Việc:";

            textBox_Shift.BackColor = Color.FromArgb(243, 244, 246);
            textBox_Shift.Font = new Font("Segoe UI", 10F);
            textBox_Shift.Location = new Point(200, 416);
            textBox_Shift.Name = "textBox_Shift";
            textBox_Shift.ReadOnly = true;
            textBox_Shift.Size = new Size(320, 34);
            textBox_Shift.TabIndex = 7;

            // 
            // panel_ActionBox
            // 
            panel_ActionBox.BackColor = Color.FromArgb(249, 250, 251);
            panel_ActionBox.BorderStyle = BorderStyle.FixedSingle;
            panel_ActionBox.Controls.Add(button_CheckOut);
            panel_ActionBox.Controls.Add(button_CheckIn);
            panel_ActionBox.Controls.Add(label_StatusToday);
            panel_ActionBox.Location = new Point(25, 480);
            panel_ActionBox.Name = "panel_ActionBox";
            panel_ActionBox.Size = new Size(495, 260);
            panel_ActionBox.TabIndex = 8;

            label_StatusToday.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label_StatusToday.ForeColor = Color.FromArgb(55, 65, 81);
            label_StatusToday.Location = new Point(15, 15);
            label_StatusToday.Name = "label_StatusToday";
            label_StatusToday.Size = new Size(465, 60);
            label_StatusToday.Text = "Trạng thái hôm nay: Vui lòng nhập mã NV để chấm công.";
            label_StatusToday.TextAlign = ContentAlignment.MiddleCenter;

            button_CheckIn.BackColor = Color.FromArgb(16, 185, 129);
            button_CheckIn.ForeColor = Color.White;
            button_CheckIn.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            button_CheckIn.Location = new Point(30, 95);
            button_CheckIn.Name = "button_CheckIn";
            button_CheckIn.Size = new Size(435, 65);
            button_CheckIn.TabIndex = 0;
            button_CheckIn.Text = "👉 CHECK - IN (VÀO CA)";
            button_CheckIn.UseVisualStyleBackColor = false;

            button_CheckOut.BackColor = Color.FromArgb(245, 158, 11);
            button_CheckOut.ForeColor = Color.White;
            button_CheckOut.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            button_CheckOut.Location = new Point(30, 175);
            button_CheckOut.Name = "button_CheckOut";
            button_CheckOut.Size = new Size(435, 65);
            button_CheckOut.TabIndex = 1;
            button_CheckOut.Text = "👈 CHECK - OUT (HẾT CA)";
            button_CheckOut.UseVisualStyleBackColor = false;

            // 
            // groupBox_RecentLogs
            // 
            groupBox_RecentLogs.BackColor = Color.White;
            groupBox_RecentLogs.Controls.Add(listView_RecentLogs);
            groupBox_RecentLogs.Dock = DockStyle.Fill;
            groupBox_RecentLogs.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            groupBox_RecentLogs.ForeColor = Color.FromArgb(31, 41, 55);
            groupBox_RecentLogs.Location = new Point(0, 0);
            groupBox_RecentLogs.Name = "groupBox_RecentLogs";
            groupBox_RecentLogs.Padding = new Padding(15);
            groupBox_RecentLogs.Size = new Size(1046, 790);
            groupBox_RecentLogs.TabIndex = 0;
            groupBox_RecentLogs.TabStop = false;
            groupBox_RecentLogs.Text = "2. Nhật Ký Quẹt Thẻ Hôm Nay (Recent Attendance Logs)";

            listView_RecentLogs.BorderStyle = BorderStyle.FixedSingle;
            listView_RecentLogs.Columns.AddRange(new ColumnHeader[] {
                col_Log_Code,
                col_Log_Name,
                col_Log_Date,
                col_Log_TimeIn,
                col_Log_TimeOut,
                col_Log_Hours,
                col_Log_Status
            });
            listView_RecentLogs.Dock = DockStyle.Fill;
            listView_RecentLogs.Font = new Font("Segoe UI", 9.5F);
            listView_RecentLogs.FullRowSelect = true;
            listView_RecentLogs.GridLines = true;
            listView_RecentLogs.Location = new Point(15, 43);
            listView_RecentLogs.Name = "listView_RecentLogs";
            listView_RecentLogs.Size = new Size(1016, 732);
            listView_RecentLogs.TabIndex = 0;
            listView_RecentLogs.UseCompatibleStateImageBehavior = false;
            listView_RecentLogs.View = View.Details;

            col_Log_Code.Text = "Mã NV";
            col_Log_Code.Width = 110;

            col_Log_Name.Text = "Họ và Tên";
            col_Log_Name.Width = 200;

            col_Log_Date.Text = "Ngày";
            col_Log_Date.TextAlign = HorizontalAlignment.Center;
            col_Log_Date.Width = 120;

            col_Log_TimeIn.Text = "Check In";
            col_Log_TimeIn.TextAlign = HorizontalAlignment.Center;
            col_Log_TimeIn.Width = 110;

            col_Log_TimeOut.Text = "Check Out";
            col_Log_TimeOut.TextAlign = HorizontalAlignment.Center;
            col_Log_TimeOut.Width = 110;

            col_Log_Hours.Text = "Giờ Làm";
            col_Log_Hours.TextAlign = HorizontalAlignment.Right;
            col_Log_Hours.Width = 110;

            col_Log_Status.Text = "Trạng Thái";
            col_Log_Status.TextAlign = HorizontalAlignment.Center;
            col_Log_Status.Width = 140;

            // 
            // AttendanceCheckInControl
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(splitContainer_Main);
            Controls.Add(panel_Header);
            Name = "AttendanceCheckInControl";
            Size = new Size(1800, 880);

            panel_Header.ResumeLayout(false);
            panel_Header.PerformLayout();
            splitContainer_Main.Panel1.ResumeLayout(false);
            splitContainer_Main.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer_Main).EndInit();
            splitContainer_Main.ResumeLayout(false);
            groupBox_EmpLookup.ResumeLayout(false);
            groupBox_EmpLookup.PerformLayout();
            panel_ActionBox.ResumeLayout(false);
            groupBox_RecentLogs.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panel_Header;
        private Label label_Title;
        private Label label_SubTitle;
        private Label label_CurrentClock;

        private SplitContainer splitContainer_Main;
        private GroupBox groupBox_EmpLookup;
        private Label label_InputPrompt;
        private TextBox textBox_EmpCode;
        private Button button_Lookup;
        private Label label_OrSelect;
        private ComboBox comboBox_Employees;

        private Label label_EmpCode;
        private TextBox textBox_EmpCodeDisplay;
        private Label label_FullName;
        private TextBox textBox_FullName;
        private Label label_Dept;
        private TextBox textBox_Department;
        private Label label_Position;
        private TextBox textBox_Position;
        private Label label_Shift;
        private TextBox textBox_Shift;

        private Panel panel_ActionBox;
        private Label label_StatusToday;
        private Button button_CheckIn;
        private Button button_CheckOut;

        private GroupBox groupBox_RecentLogs;
        private ListView listView_RecentLogs;
        private ColumnHeader col_Log_Code;
        private ColumnHeader col_Log_Name;
        private ColumnHeader col_Log_Date;
        private ColumnHeader col_Log_TimeIn;
        private ColumnHeader col_Log_TimeOut;
        private ColumnHeader col_Log_Hours;
        private ColumnHeader col_Log_Status;
    }
}

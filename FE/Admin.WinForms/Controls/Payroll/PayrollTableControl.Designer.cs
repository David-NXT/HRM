namespace Admin.WinForms.Controls.Payroll
{
    partial class PayrollTableControl
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
            panel_Stats = new Panel();
            tableLayout_Stats = new TableLayoutPanel();
            panel_NetSalary = new Panel();
            label_TotalNet = new Label();
            label_TitleNet = new Label();
            panel_Employees = new Panel();
            label_TotalEmployees = new Label();
            label_TitleEmployees = new Label();
            panel_Allowance = new Panel();
            label_TotalAllowance = new Label();
            label_TitleAllowance = new Label();
            panel_PayCut = new Panel();
            label_TotalPayCut = new Label();
            label_TitlePayCut = new Label();

            panel_Toolbar = new Panel();
            label_Period = new Label();
            comboBox_Periods = new ComboBox();
            button_NewPeriod = new Button();
            button_SyncAttendance = new Button();
            button_SyncSalary = new Button();
            button_CreatePayrolls = new Button();
            button_CalculateAll = new Button();
            button_ExportExcel = new Button();

            listView_Payrolls = new ListView();
            col_EmpCode = new ColumnHeader();
            col_EmpName = new ColumnHeader();
            col_BaseSalary = new ColumnHeader();
            col_StandardDays = new ColumnHeader();
            col_ActualDays = new ColumnHeader();
            col_Allowance = new ColumnHeader();
            col_PayCut = new ColumnHeader();
            col_Gross = new ColumnHeader();
            col_Net = new ColumnHeader();
            col_Status = new ColumnHeader();
            col_CalculatedAt = new ColumnHeader();

            panel_Stats.SuspendLayout();
            panel_NetSalary.SuspendLayout();
            panel_Employees.SuspendLayout();
            panel_Allowance.SuspendLayout();
            panel_PayCut.SuspendLayout();
            panel_Toolbar.SuspendLayout();
            SuspendLayout();

            // 
            // panel_Stats
            // 
            panel_Stats.Controls.Add(tableLayout_Stats);
            panel_Stats.Dock = DockStyle.Top;
            panel_Stats.Location = new Point(0, 0);
            panel_Stats.Name = "panel_Stats";
            panel_Stats.Size = new Size(1800, 130);
            panel_Stats.TabIndex = 0;
            // 
            // tableLayout_Stats
            // 
            tableLayout_Stats.ColumnCount = 4;
            tableLayout_Stats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayout_Stats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayout_Stats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayout_Stats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayout_Stats.Controls.Add(panel_NetSalary, 0, 0);
            tableLayout_Stats.Controls.Add(panel_Employees, 1, 0);
            tableLayout_Stats.Controls.Add(panel_Allowance, 2, 0);
            tableLayout_Stats.Controls.Add(panel_PayCut, 3, 0);
            tableLayout_Stats.Dock = DockStyle.Fill;
            tableLayout_Stats.Location = new Point(0, 0);
            tableLayout_Stats.Name = "tableLayout_Stats";
            tableLayout_Stats.RowCount = 1;
            tableLayout_Stats.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayout_Stats.Size = new Size(1800, 130);
            tableLayout_Stats.TabIndex = 0;
            // 
            // panel_NetSalary
            // 
            panel_NetSalary.BackColor = Color.FromArgb(236, 249, 240);
            panel_NetSalary.Controls.Add(label_TotalNet);
            panel_NetSalary.Controls.Add(label_TitleNet);
            panel_NetSalary.Dock = DockStyle.Fill;
            panel_NetSalary.Location = new Point(8, 8);
            panel_NetSalary.Margin = new Padding(8);
            panel_NetSalary.Name = "panel_NetSalary";
            panel_NetSalary.Size = new Size(434, 114);
            panel_NetSalary.TabIndex = 0;

            label_TotalNet.AutoSize = true;
            label_TotalNet.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            label_TotalNet.ForeColor = Color.FromArgb(16, 149, 74);
            label_TotalNet.Location = new Point(15, 10);
            label_TotalNet.Name = "label_TotalNet";
            label_TotalNet.Size = new Size(130, 45);
            label_TotalNet.Text = "0 đ";

            label_TitleNet.AutoSize = true;
            label_TitleNet.Font = new Font("Segoe UI", 10F);
            label_TitleNet.ForeColor = Color.FromArgb(75, 85, 99);
            label_TitleNet.Location = new Point(15, 60);
            label_TitleNet.Name = "label_TitleNet";
            label_TitleNet.Size = new Size(240, 28);
            label_TitleNet.Text = "Tổng Quỹ Lương Thực Lĩnh";

            // 
            // panel_Employees
            // 
            panel_Employees.BackColor = Color.FromArgb(229, 238, 253);
            panel_Employees.Controls.Add(label_TotalEmployees);
            panel_Employees.Controls.Add(label_TitleEmployees);
            panel_Employees.Dock = DockStyle.Fill;
            panel_Employees.Location = new Point(458, 8);
            panel_Employees.Margin = new Padding(8);
            panel_Employees.Name = "panel_Employees";
            panel_Employees.Size = new Size(434, 114);
            panel_Employees.TabIndex = 1;

            label_TotalEmployees.AutoSize = true;
            label_TotalEmployees.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            label_TotalEmployees.ForeColor = Color.FromArgb(37, 99, 235);
            label_TotalEmployees.Location = new Point(15, 10);
            label_TotalEmployees.Name = "label_TotalEmployees";
            label_TotalEmployees.Size = new Size(40, 45);
            label_TotalEmployees.Text = "0";

            label_TitleEmployees.AutoSize = true;
            label_TitleEmployees.Font = new Font("Segoe UI", 10F);
            label_TitleEmployees.ForeColor = Color.FromArgb(75, 85, 99);
            label_TitleEmployees.Location = new Point(15, 60);
            label_TitleEmployees.Name = "label_TitleEmployees";
            label_TitleEmployees.Size = new Size(200, 28);
            label_TitleEmployees.Text = "Số Nhân Sự Trong Kỳ";

            // 
            // panel_Allowance
            // 
            panel_Allowance.BackColor = Color.FromArgb(254, 244, 234);
            panel_Allowance.Controls.Add(label_TotalAllowance);
            panel_Allowance.Controls.Add(label_TitleAllowance);
            panel_Allowance.Dock = DockStyle.Fill;
            panel_Allowance.Location = new Point(908, 8);
            panel_Allowance.Margin = new Padding(8);
            panel_Allowance.Name = "panel_Allowance";
            panel_Allowance.Size = new Size(434, 114);
            panel_Allowance.TabIndex = 2;

            label_TotalAllowance.AutoSize = true;
            label_TotalAllowance.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            label_TotalAllowance.ForeColor = Color.FromArgb(217, 119, 6);
            label_TotalAllowance.Location = new Point(15, 10);
            label_TotalAllowance.Name = "label_TotalAllowance";
            label_TotalAllowance.Size = new Size(130, 45);
            label_TotalAllowance.Text = "0 đ";

            label_TitleAllowance.AutoSize = true;
            label_TitleAllowance.Font = new Font("Segoe UI", 10F);
            label_TitleAllowance.ForeColor = Color.FromArgb(75, 85, 99);
            label_TitleAllowance.Location = new Point(15, 60);
            label_TitleAllowance.Name = "label_TitleAllowance";
            label_TitleAllowance.Size = new Size(180, 28);
            label_TitleAllowance.Text = "Tổng Tiền Phụ Cấp";

            // 
            // panel_PayCut
            // 
            panel_PayCut.BackColor = Color.FromArgb(253, 237, 240);
            panel_PayCut.Controls.Add(label_TotalPayCut);
            panel_PayCut.Controls.Add(label_TitlePayCut);
            panel_PayCut.Dock = DockStyle.Fill;
            panel_PayCut.Location = new Point(1358, 8);
            panel_PayCut.Margin = new Padding(8);
            panel_PayCut.Name = "panel_PayCut";
            panel_PayCut.Size = new Size(434, 114);
            panel_PayCut.TabIndex = 3;

            label_TotalPayCut.AutoSize = true;
            label_TotalPayCut.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            label_TotalPayCut.ForeColor = Color.FromArgb(220, 38, 38);
            label_TotalPayCut.Location = new Point(15, 10);
            label_TotalPayCut.Name = "label_TotalPayCut";
            label_TotalPayCut.Size = new Size(130, 45);
            label_TotalPayCut.Text = "0 đ";

            label_TitlePayCut.AutoSize = true;
            label_TitlePayCut.Font = new Font("Segoe UI", 10F);
            label_TitlePayCut.ForeColor = Color.FromArgb(75, 85, 99);
            label_TitlePayCut.Location = new Point(15, 60);
            label_TitlePayCut.Name = "label_TitlePayCut";
            label_TitlePayCut.Size = new Size(220, 28);
            label_TitlePayCut.Text = "Tổng Khoản Khấu Trừ/Phạt";

            // 
            // panel_Toolbar
            // 
            panel_Toolbar.Controls.Add(label_Period);
            panel_Toolbar.Controls.Add(comboBox_Periods);
            panel_Toolbar.Controls.Add(button_NewPeriod);
            panel_Toolbar.Controls.Add(button_SyncAttendance);
            panel_Toolbar.Controls.Add(button_SyncSalary);
            panel_Toolbar.Controls.Add(button_CreatePayrolls);
            panel_Toolbar.Controls.Add(button_CalculateAll);
            panel_Toolbar.Controls.Add(button_ExportExcel);
            panel_Toolbar.Dock = DockStyle.Top;
            panel_Toolbar.Location = new Point(0, 130);
            panel_Toolbar.Name = "panel_Toolbar";
            panel_Toolbar.Size = new Size(1800, 70);
            panel_Toolbar.TabIndex = 1;

            label_Period.AutoSize = true;
            label_Period.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label_Period.Location = new Point(15, 20);
            label_Period.Name = "label_Period";
            label_Period.Size = new Size(106, 28);
            label_Period.Text = "Kỳ lương:";

            comboBox_Periods.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_Periods.Font = new Font("Segoe UI", 10F);
            comboBox_Periods.FormattingEnabled = true;
            comboBox_Periods.Location = new Point(125, 16);
            comboBox_Periods.Name = "comboBox_Periods";
            comboBox_Periods.Size = new Size(320, 36);
            comboBox_Periods.TabIndex = 0;

            button_NewPeriod.Font = new Font("Segoe UI", 9F);
            button_NewPeriod.Location = new Point(455, 14);
            button_NewPeriod.Name = "button_NewPeriod";
            button_NewPeriod.Size = new Size(140, 42);
            button_NewPeriod.TabIndex = 1;
            button_NewPeriod.Text = "+ Mở Kỳ Mới";
            button_NewPeriod.UseVisualStyleBackColor = true;

            button_SyncAttendance.BackColor = Color.FromArgb(243, 244, 246);
            button_SyncAttendance.Font = new Font("Segoe UI", 9F);
            button_SyncAttendance.Location = new Point(605, 14);
            button_SyncAttendance.Name = "button_SyncAttendance";
            button_SyncAttendance.Size = new Size(190, 42);
            button_SyncAttendance.TabIndex = 2;
            button_SyncAttendance.Text = "🔄 Chốt & Đồng Bộ Công";
            button_SyncAttendance.UseVisualStyleBackColor = true;

            button_SyncSalary.BackColor = Color.FromArgb(243, 244, 246);
            button_SyncSalary.Font = new Font("Segoe UI", 9F);
            button_SyncSalary.Location = new Point(805, 14);
            button_SyncSalary.Name = "button_SyncSalary";
            button_SyncSalary.Size = new Size(180, 42);
            button_SyncSalary.TabIndex = 3;
            button_SyncSalary.Text = "🔄 Đồng Bộ Mức Lương";
            button_SyncSalary.UseVisualStyleBackColor = true;

            button_CreatePayrolls.BackColor = Color.FromArgb(37, 99, 235);
            button_CreatePayrolls.ForeColor = Color.White;
            button_CreatePayrolls.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button_CreatePayrolls.Location = new Point(995, 14);
            button_CreatePayrolls.Name = "button_CreatePayrolls";
            button_CreatePayrolls.Size = new Size(180, 42);
            button_CreatePayrolls.TabIndex = 4;
            button_CreatePayrolls.Text = "➕ Tạo Bảng Lương";
            button_CreatePayrolls.UseVisualStyleBackColor = false;

            button_CalculateAll.BackColor = Color.FromArgb(16, 185, 129);
            button_CalculateAll.ForeColor = Color.White;
            button_CalculateAll.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button_CalculateAll.Location = new Point(1185, 14);
            button_CalculateAll.Name = "button_CalculateAll";
            button_CalculateAll.Size = new Size(170, 42);
            button_CalculateAll.TabIndex = 5;
            button_CalculateAll.Text = "⚡ Tính Lương";
            button_CalculateAll.UseVisualStyleBackColor = false;

            button_ExportExcel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_ExportExcel.Font = new Font("Segoe UI", 9F);
            button_ExportExcel.Location = new Point(1620, 14);
            button_ExportExcel.Name = "button_ExportExcel";
            button_ExportExcel.Size = new Size(160, 42);
            button_ExportExcel.TabIndex = 6;
            button_ExportExcel.Text = "Xuất Excel (CSV)";
            button_ExportExcel.UseVisualStyleBackColor = true;

            // 
            // listView_Payrolls
            // 
            listView_Payrolls.BorderStyle = BorderStyle.FixedSingle;
            listView_Payrolls.Columns.AddRange(new ColumnHeader[] {
                col_EmpCode,
                col_EmpName,
                col_BaseSalary,
                col_StandardDays,
                col_ActualDays,
                col_Allowance,
                col_PayCut,
                col_Gross,
                col_Net,
                col_Status,
                col_CalculatedAt
            });
            listView_Payrolls.Dock = DockStyle.Fill;
            listView_Payrolls.Font = new Font("Segoe UI", 9.5F);
            listView_Payrolls.FullRowSelect = true;
            listView_Payrolls.GridLines = true;
            listView_Payrolls.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            listView_Payrolls.Location = new Point(0, 200);
            listView_Payrolls.Name = "listView_Payrolls";
            listView_Payrolls.Size = new Size(1800, 680);
            listView_Payrolls.TabIndex = 2;
            listView_Payrolls.UseCompatibleStateImageBehavior = false;
            listView_Payrolls.View = View.Details;

            col_EmpCode.Text = "Mã NV";
            col_EmpCode.Width = 120;

            col_EmpName.Text = "Họ và Tên";
            col_EmpName.Width = 220;

            col_BaseSalary.Text = "Lương cơ bản";
            col_BaseSalary.TextAlign = HorizontalAlignment.Right;
            col_BaseSalary.Width = 150;

            col_StandardDays.Text = "Công chuẩn";
            col_StandardDays.TextAlign = HorizontalAlignment.Center;
            col_StandardDays.Width = 110;

            col_ActualDays.Text = "Công thực tế";
            col_ActualDays.TextAlign = HorizontalAlignment.Center;
            col_ActualDays.Width = 110;

            col_Allowance.Text = "Tổng phụ cấp";
            col_Allowance.TextAlign = HorizontalAlignment.Right;
            col_Allowance.Width = 150;

            col_PayCut.Text = "Khấu trừ/Phạt";
            col_PayCut.TextAlign = HorizontalAlignment.Right;
            col_PayCut.Width = 150;

            col_Gross.Text = "Lương Gross";
            col_Gross.TextAlign = HorizontalAlignment.Right;
            col_Gross.Width = 160;

            col_Net.Text = "Thực lĩnh (NET)";
            col_Net.TextAlign = HorizontalAlignment.Right;
            col_Net.Width = 180;

            col_Status.Text = "Trạng thái";
            col_Status.TextAlign = HorizontalAlignment.Center;
            col_Status.Width = 130;

            col_CalculatedAt.Text = "Ngày tính toán";
            col_CalculatedAt.Width = 160;

            // 
            // PayrollTableControl
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(listView_Payrolls);
            Controls.Add(panel_Toolbar);
            Controls.Add(panel_Stats);
            Name = "PayrollTableControl";
            Size = new Size(1800, 880);

            panel_Stats.ResumeLayout(false);
            panel_NetSalary.ResumeLayout(false);
            panel_NetSalary.PerformLayout();
            panel_Employees.ResumeLayout(false);
            panel_Employees.PerformLayout();
            panel_Allowance.ResumeLayout(false);
            panel_Allowance.PerformLayout();
            panel_PayCut.ResumeLayout(false);
            panel_PayCut.PerformLayout();
            panel_Toolbar.ResumeLayout(false);
            panel_Toolbar.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel_Stats;
        private TableLayoutPanel tableLayout_Stats;
        private Panel panel_NetSalary;
        private Label label_TotalNet;
        private Label label_TitleNet;
        private Panel panel_Employees;
        private Label label_TotalEmployees;
        private Label label_TitleEmployees;
        private Panel panel_Allowance;
        private Label label_TotalAllowance;
        private Label label_TitleAllowance;
        private Panel panel_PayCut;
        private Label label_TotalPayCut;
        private Label label_TitlePayCut;

        private Panel panel_Toolbar;
        private Label label_Period;
        private ComboBox comboBox_Periods;
        private Button button_NewPeriod;
        private Button button_SyncAttendance;
        private Button button_SyncSalary;
        private Button button_CreatePayrolls;
        private Button button_CalculateAll;
        private Button button_ExportExcel;

        private ListView listView_Payrolls;
        private ColumnHeader col_EmpCode;
        private ColumnHeader col_EmpName;
        private ColumnHeader col_BaseSalary;
        private ColumnHeader col_StandardDays;
        private ColumnHeader col_ActualDays;
        private ColumnHeader col_Allowance;
        private ColumnHeader col_PayCut;
        private ColumnHeader col_Gross;
        private ColumnHeader col_Net;
        private ColumnHeader col_Status;
        private ColumnHeader col_CalculatedAt;
    }
}

namespace Admin.WinForms.Controls.Payroll
{
    partial class PayrollAdjustmentControl
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
            panel_Top = new Panel();
            label_Period = new Label();
            comboBox_Periods = new ComboBox();
            label_Employee = new Label();
            textBox_EmployeeSearch = new TextBox();
            button_SearchEmployee = new Button();
            comboBox_Employees = new ComboBox();
            button_AddAllowance = new Button();
            button_AddPayCut = new Button();
            button_ManageCatalog = new Button();
            button_Reload = new Button();

            splitContainer_Main = new SplitContainer();
            groupBox_Allowances = new GroupBox();
            listView_Allowances = new ListView();
            col_Allow_Type = new ColumnHeader();
            col_Allow_Desc = new ColumnHeader();
            col_Allow_Rate = new ColumnHeader();
            col_Allow_Amount = new ColumnHeader();

            groupBox_PayCuts = new GroupBox();
            listView_PayCuts = new ListView();
            col_Cut_Reason = new ColumnHeader();
            col_Cut_Desc = new ColumnHeader();
            col_Cut_Rate = new ColumnHeader();
            col_Cut_Amount = new ColumnHeader();

            panel_Bottom = new Panel();
            label_Summary = new Label();

            panel_Top.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer_Main).BeginInit();
            splitContainer_Main.Panel1.SuspendLayout();
            splitContainer_Main.Panel2.SuspendLayout();
            splitContainer_Main.SuspendLayout();
            groupBox_Allowances.SuspendLayout();
            groupBox_PayCuts.SuspendLayout();
            panel_Bottom.SuspendLayout();
            SuspendLayout();

            // 
            // panel_Top
            // 
            panel_Top.Controls.Add(label_Period);
            panel_Top.Controls.Add(comboBox_Periods);
            panel_Top.Controls.Add(label_Employee);
            panel_Top.Controls.Add(textBox_EmployeeSearch);
            panel_Top.Controls.Add(button_SearchEmployee);
            panel_Top.Controls.Add(comboBox_Employees);
            panel_Top.Controls.Add(button_AddAllowance);
            panel_Top.Controls.Add(button_AddPayCut);
            panel_Top.Controls.Add(button_ManageCatalog);
            panel_Top.Controls.Add(button_Reload);
            panel_Top.Dock = DockStyle.Top;
            panel_Top.Location = new Point(0, 0);
            panel_Top.Name = "panel_Top";
            panel_Top.Size = new Size(1800, 75);
            panel_Top.TabIndex = 0;

            label_Period.AutoSize = true;
            label_Period.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label_Period.Location = new Point(15, 23);
            label_Period.Name = "label_Period";
            label_Period.Size = new Size(106, 28);
            label_Period.Text = "Kỳ lương:";

            comboBox_Periods.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_Periods.Font = new Font("Segoe UI", 10F);
            comboBox_Periods.Location = new Point(125, 19);
            comboBox_Periods.Name = "comboBox_Periods";
            comboBox_Periods.Size = new Size(300, 36);
            comboBox_Periods.TabIndex = 0;

            label_Employee.AutoSize = true;
            label_Employee.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label_Employee.Location = new Point(445, 23);
            label_Employee.Name = "label_Employee";
            label_Employee.Size = new Size(116, 28);
            label_Employee.Text = "Nhân viên:";

            textBox_EmployeeSearch.Font = new Font("Segoe UI", 9F);
            textBox_EmployeeSearch.Location = new Point(565, 20);
            textBox_EmployeeSearch.Name = "textBox_EmployeeSearch";
            textBox_EmployeeSearch.PlaceholderText = "Tìm mã / tên / ID";
            textBox_EmployeeSearch.Size = new Size(170, 32);
            textBox_EmployeeSearch.TabIndex = 1;

            button_SearchEmployee.Font = new Font("Segoe UI", 9F);
            button_SearchEmployee.Location = new Point(740, 16);
            button_SearchEmployee.Name = "button_SearchEmployee";
            button_SearchEmployee.Size = new Size(75, 42);
            button_SearchEmployee.TabIndex = 2;
            button_SearchEmployee.Text = "Tìm";
            button_SearchEmployee.UseVisualStyleBackColor = true;

            comboBox_Employees.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_Employees.Font = new Font("Segoe UI", 10F);
            comboBox_Employees.Location = new Point(825, 19);
            comboBox_Employees.Name = "comboBox_Employees";
            comboBox_Employees.Size = new Size(220, 36);
            comboBox_Employees.TabIndex = 3;

            button_AddAllowance.BackColor = Color.FromArgb(16, 185, 129);
            button_AddAllowance.ForeColor = Color.White;
            button_AddAllowance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button_AddAllowance.Location = new Point(1055, 16);
            button_AddAllowance.Name = "button_AddAllowance";
            button_AddAllowance.Size = new Size(180, 42);
            button_AddAllowance.TabIndex = 4;
            button_AddAllowance.Text = "+ Thêm Phụ Cấp";
            button_AddAllowance.UseVisualStyleBackColor = false;

            button_AddPayCut.BackColor = Color.FromArgb(239, 68, 68);
            button_AddPayCut.ForeColor = Color.White;
            button_AddPayCut.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button_AddPayCut.Location = new Point(1245, 16);
            button_AddPayCut.Name = "button_AddPayCut";
            button_AddPayCut.Size = new Size(200, 42);
            button_AddPayCut.TabIndex = 5;
            button_AddPayCut.Text = "- Thêm Khấu Trừ/Phạt";
            button_AddPayCut.UseVisualStyleBackColor = false;

            button_ManageCatalog.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button_ManageCatalog.Location = new Point(1455, 16);
            button_ManageCatalog.Name = "button_ManageCatalog";
            button_ManageCatalog.Size = new Size(200, 42);
            button_ManageCatalog.TabIndex = 6;
            button_ManageCatalog.Text = "Quản lý danh mục";
            button_ManageCatalog.UseVisualStyleBackColor = true;

            button_Reload.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_Reload.Font = new Font("Segoe UI", 9F);
            button_Reload.Location = new Point(1670, 16);
            button_Reload.Name = "button_Reload";
            button_Reload.Size = new Size(110, 42);
            button_Reload.TabIndex = 7;
            button_Reload.Text = "Reload";
            button_Reload.UseVisualStyleBackColor = true;

            // 
            // splitContainer_Main
            // 
            splitContainer_Main.Dock = DockStyle.Fill;
            splitContainer_Main.Location = new Point(0, 75);
            splitContainer_Main.Name = "splitContainer_Main";
            splitContainer_Main.Size = new Size(1800, 745);
            splitContainer_Main.SplitterDistance = 900;
            splitContainer_Main.TabIndex = 1;

            // 
            // groupBox_Allowances
            // 
            groupBox_Allowances.Controls.Add(listView_Allowances);
            groupBox_Allowances.Dock = DockStyle.Fill;
            groupBox_Allowances.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            groupBox_Allowances.ForeColor = Color.FromArgb(16, 149, 74);
            groupBox_Allowances.Location = new Point(0, 0);
            groupBox_Allowances.Name = "groupBox_Allowances";
            groupBox_Allowances.Padding = new Padding(10);
            groupBox_Allowances.Size = new Size(900, 745);
            groupBox_Allowances.TabIndex = 0;
            groupBox_Allowances.TabStop = false;
            groupBox_Allowances.Text = "Khoản Phụ Cấp & Thưởng (Allowances)";

            listView_Allowances.BorderStyle = BorderStyle.FixedSingle;
            listView_Allowances.Columns.AddRange(new ColumnHeader[] { col_Allow_Type, col_Allow_Desc, col_Allow_Rate, col_Allow_Amount });
            listView_Allowances.Dock = DockStyle.Fill;
            listView_Allowances.Font = new Font("Segoe UI", 9.5F);
            listView_Allowances.ForeColor = Color.FromArgb(37, 48, 74);
            listView_Allowances.FullRowSelect = true;
            listView_Allowances.GridLines = true;
            listView_Allowances.Location = new Point(10, 37);
            listView_Allowances.Name = "listView_Allowances";
            listView_Allowances.Size = new Size(880, 698);
            listView_Allowances.TabIndex = 0;
            listView_Allowances.UseCompatibleStateImageBehavior = false;
            listView_Allowances.View = View.Details;

            col_Allow_Type.Text = "Loại phụ cấp";
            col_Allow_Type.Width = 240;

            col_Allow_Desc.Text = "Mô tả / Ghi chú";
            col_Allow_Desc.Width = 280;

            col_Allow_Rate.Text = "Tỷ lệ áp dụng";
            col_Allow_Rate.TextAlign = HorizontalAlignment.Center;
            col_Allow_Rate.Width = 130;

            col_Allow_Amount.Text = "Số tiền";
            col_Allow_Amount.TextAlign = HorizontalAlignment.Right;
            col_Allow_Amount.Width = 180;

            // 
            // groupBox_PayCuts
            // 
            groupBox_PayCuts.Controls.Add(listView_PayCuts);
            groupBox_PayCuts.Dock = DockStyle.Fill;
            groupBox_PayCuts.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            groupBox_PayCuts.ForeColor = Color.FromArgb(220, 38, 38);
            groupBox_PayCuts.Location = new Point(0, 0);
            groupBox_PayCuts.Name = "groupBox_PayCuts";
            groupBox_PayCuts.Padding = new Padding(10);
            groupBox_PayCuts.Size = new Size(896, 745);
            groupBox_PayCuts.TabIndex = 0;
            groupBox_PayCuts.TabStop = false;
            groupBox_PayCuts.Text = "Khoản Khấu Trừ, Phạt & Bảo Hiểm (Deductions)";

            listView_PayCuts.BorderStyle = BorderStyle.FixedSingle;
            listView_PayCuts.Columns.AddRange(new ColumnHeader[] { col_Cut_Reason, col_Cut_Desc, col_Cut_Rate, col_Cut_Amount });
            listView_PayCuts.Dock = DockStyle.Fill;
            listView_PayCuts.Font = new Font("Segoe UI", 9.5F);
            listView_PayCuts.ForeColor = Color.FromArgb(37, 48, 74);
            listView_PayCuts.FullRowSelect = true;
            listView_PayCuts.GridLines = true;
            listView_PayCuts.Location = new Point(10, 37);
            listView_PayCuts.Name = "listView_PayCuts";
            listView_PayCuts.Size = new Size(876, 698);
            listView_PayCuts.TabIndex = 0;
            listView_PayCuts.UseCompatibleStateImageBehavior = false;
            listView_PayCuts.View = View.Details;

            col_Cut_Reason.Text = "Lý do khấu trừ";
            col_Cut_Reason.Width = 240;

            col_Cut_Desc.Text = "Mô tả / Ghi chú";
            col_Cut_Desc.Width = 280;

            col_Cut_Rate.Text = "Tỷ lệ áp dụng";
            col_Cut_Rate.TextAlign = HorizontalAlignment.Center;
            col_Cut_Rate.Width = 130;

            col_Cut_Amount.Text = "Số tiền";
            col_Cut_Amount.TextAlign = HorizontalAlignment.Right;
            col_Cut_Amount.Width = 180;

            // 
            // panel_Bottom
            // 
            panel_Bottom.BackColor = Color.FromArgb(249, 250, 251);
            panel_Bottom.Controls.Add(label_Summary);
            panel_Bottom.Dock = DockStyle.Bottom;
            panel_Bottom.Location = new Point(0, 820);
            panel_Bottom.Name = "panel_Bottom";
            panel_Bottom.Size = new Size(1800, 60);
            panel_Bottom.TabIndex = 2;

            label_Summary.AutoSize = true;
            label_Summary.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            label_Summary.ForeColor = Color.FromArgb(31, 41, 55);
            label_Summary.Location = new Point(15, 15);
            label_Summary.Name = "label_Summary";
            label_Summary.Size = new Size(450, 30);
            label_Summary.Text = "Tổng phụ cấp: 0 đ | Tổng khấu trừ: 0 đ";

            // 
            // PayrollAdjustmentControl
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(splitContainer_Main);
            Controls.Add(panel_Bottom);
            Controls.Add(panel_Top);
            Name = "PayrollAdjustmentControl";
            Size = new Size(1800, 880);

            panel_Top.ResumeLayout(false);
            panel_Top.PerformLayout();
            splitContainer_Main.Panel1.ResumeLayout(false);
            splitContainer_Main.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer_Main).EndInit();
            splitContainer_Main.ResumeLayout(false);
            groupBox_Allowances.ResumeLayout(false);
            groupBox_PayCuts.ResumeLayout(false);
            panel_Bottom.ResumeLayout(false);
            panel_Bottom.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel_Top;
        private Label label_Period;
        private ComboBox comboBox_Periods;
        private Label label_Employee;
        private TextBox textBox_EmployeeSearch;
        private Button button_SearchEmployee;
        private ComboBox comboBox_Employees;
        private Button button_AddAllowance;
        private Button button_AddPayCut;
        private Button button_ManageCatalog;
        private Button button_Reload;

        private SplitContainer splitContainer_Main;
        private GroupBox groupBox_Allowances;
        private ListView listView_Allowances;
        private ColumnHeader col_Allow_Type;
        private ColumnHeader col_Allow_Desc;
        private ColumnHeader col_Allow_Rate;
        private ColumnHeader col_Allow_Amount;

        private GroupBox groupBox_PayCuts;
        private ListView listView_PayCuts;
        private ColumnHeader col_Cut_Reason;
        private ColumnHeader col_Cut_Desc;
        private ColumnHeader col_Cut_Rate;
        private ColumnHeader col_Cut_Amount;

        private Panel panel_Bottom;
        private Label label_Summary;
    }
}

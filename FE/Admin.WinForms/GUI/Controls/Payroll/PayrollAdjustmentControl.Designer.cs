namespace Admin.WinForms.GUI.Controls.Payroll
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
            splitContainer_Main.SuspendLayout();
            groupBox_Allowances.SuspendLayout();
            groupBox_PayCuts.SuspendLayout();
            panel_Bottom.SuspendLayout();
            SuspendLayout();
            // 
            // panel_Top
            // 
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
            panel_Top.Margin = new Padding(4);
            panel_Top.Name = "panel_Top";
            panel_Top.Size = new Size(2340, 96);
            panel_Top.TabIndex = 0;
            // 
            // comboBox_Periods
            // 
            comboBox_Periods.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_Periods.Font = new Font("Segoe UI", 10F);
            comboBox_Periods.Location = new Point(20, 24);
            comboBox_Periods.Margin = new Padding(4);
            comboBox_Periods.Name = "comboBox_Periods";
            comboBox_Periods.Size = new Size(493, 45);
            comboBox_Periods.TabIndex = 0;
            // 
            // label_Employee
            // 
            label_Employee.AutoSize = true;
            label_Employee.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label_Employee.Location = new Point(524, 27);
            label_Employee.Margin = new Padding(4, 0, 4, 0);
            label_Employee.Name = "label_Employee";
            label_Employee.Size = new Size(153, 37);
            label_Employee.TabIndex = 1;
            label_Employee.Text = "Nhân viên:";
            // 
            // textBox_EmployeeSearch
            // 
            textBox_EmployeeSearch.Font = new Font("Segoe UI", 9F);
            textBox_EmployeeSearch.Location = new Point(680, 24);
            textBox_EmployeeSearch.Margin = new Padding(4);
            textBox_EmployeeSearch.Name = "textBox_EmployeeSearch";
            textBox_EmployeeSearch.PlaceholderText = "Tìm mã / tên / ID";
            textBox_EmployeeSearch.Size = new Size(220, 39);
            textBox_EmployeeSearch.TabIndex = 1;
            // 
            // button_SearchEmployee
            // 
            button_SearchEmployee.Font = new Font("Segoe UI", 9F);
            button_SearchEmployee.Location = new Point(908, 20);
            button_SearchEmployee.Margin = new Padding(4);
            button_SearchEmployee.Name = "button_SearchEmployee";
            button_SearchEmployee.Size = new Size(98, 54);
            button_SearchEmployee.TabIndex = 2;
            button_SearchEmployee.Text = "Tìm";
            button_SearchEmployee.UseVisualStyleBackColor = true;
            // 
            // comboBox_Employees
            // 
            comboBox_Employees.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_Employees.Font = new Font("Segoe UI", 10F);
            comboBox_Employees.Location = new Point(1018, 24);
            comboBox_Employees.Margin = new Padding(4);
            comboBox_Employees.Name = "comboBox_Employees";
            comboBox_Employees.Size = new Size(285, 45);
            comboBox_Employees.TabIndex = 3;
            // 
            // button_AddAllowance
            // 
            button_AddAllowance.BackColor = Color.FromArgb(16, 185, 129);
            button_AddAllowance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button_AddAllowance.ForeColor = Color.White;
            button_AddAllowance.Location = new Point(1311, 20);
            button_AddAllowance.Margin = new Padding(4);
            button_AddAllowance.Name = "button_AddAllowance";
            button_AddAllowance.Size = new Size(234, 54);
            button_AddAllowance.TabIndex = 4;
            button_AddAllowance.Text = "+ Thêm Phụ Cấp";
            button_AddAllowance.UseVisualStyleBackColor = false;
            // 
            // button_AddPayCut
            // 
            button_AddPayCut.BackColor = Color.FromArgb(239, 68, 68);
            button_AddPayCut.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button_AddPayCut.ForeColor = Color.White;
            button_AddPayCut.Location = new Point(1557, 20);
            button_AddPayCut.Margin = new Padding(4);
            button_AddPayCut.Name = "button_AddPayCut";
            button_AddPayCut.Size = new Size(260, 54);
            button_AddPayCut.TabIndex = 5;
            button_AddPayCut.Text = "- Thêm Khấu Trừ/Phạt";
            button_AddPayCut.UseVisualStyleBackColor = false;
            // 
            // button_ManageCatalog
            // 
            button_ManageCatalog.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button_ManageCatalog.Location = new Point(1825, 20);
            button_ManageCatalog.Margin = new Padding(4);
            button_ManageCatalog.Name = "button_ManageCatalog";
            button_ManageCatalog.Size = new Size(272, 54);
            button_ManageCatalog.TabIndex = 6;
            button_ManageCatalog.Text = "Quản lý danh mục";
            button_ManageCatalog.UseVisualStyleBackColor = true;
            // 
            // button_Reload
            // 
            button_Reload.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_Reload.Font = new Font("Segoe UI", 9F);
            button_Reload.Location = new Point(2193, 20);
            button_Reload.Margin = new Padding(4);
            button_Reload.Name = "button_Reload";
            button_Reload.Size = new Size(143, 54);
            button_Reload.TabIndex = 7;
            button_Reload.Text = "Reload";
            button_Reload.UseVisualStyleBackColor = true;
            // 
            // splitContainer_Main
            // 
            splitContainer_Main.Dock = DockStyle.Fill;
            splitContainer_Main.Location = new Point(0, 96);
            splitContainer_Main.Margin = new Padding(4);
            splitContainer_Main.Name = "splitContainer_Main";
            splitContainer_Main.Size = new Size(2340, 953);
            splitContainer_Main.SplitterDistance = 1170;
            splitContainer_Main.SplitterWidth = 5;
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
            // 
            // listView_Allowances
            // 
            listView_Allowances.BorderStyle = BorderStyle.FixedSingle;
            listView_Allowances.Columns.AddRange(new ColumnHeader[] { col_Allow_Type, col_Allow_Desc, col_Allow_Rate, col_Allow_Amount });
            listView_Allowances.Dock = DockStyle.Fill;
            listView_Allowances.Font = new Font("Segoe UI", 9.5F);
            listView_Allowances.ForeColor = Color.FromArgb(37, 48, 74);
            listView_Allowances.FullRowSelect = true;
            listView_Allowances.GridLines = true;
            listView_Allowances.Location = new Point(10, 46);
            listView_Allowances.Name = "listView_Allowances";
            listView_Allowances.Size = new Size(880, 689);
            listView_Allowances.TabIndex = 0;
            listView_Allowances.UseCompatibleStateImageBehavior = false;
            listView_Allowances.View = View.Details;
            // 
            // col_Allow_Type
            // 
            col_Allow_Type.Text = "Loại phụ cấp";
            col_Allow_Type.Width = 240;
            // 
            // col_Allow_Desc
            // 
            col_Allow_Desc.Text = "Mô tả / Ghi chú";
            col_Allow_Desc.Width = 280;
            // 
            // col_Allow_Rate
            // 
            col_Allow_Rate.Text = "Tỷ lệ áp dụng";
            col_Allow_Rate.TextAlign = HorizontalAlignment.Center;
            col_Allow_Rate.Width = 130;
            // 
            // col_Allow_Amount
            // 
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
            // 
            // listView_PayCuts
            // 
            listView_PayCuts.BorderStyle = BorderStyle.FixedSingle;
            listView_PayCuts.Columns.AddRange(new ColumnHeader[] { col_Cut_Reason, col_Cut_Desc, col_Cut_Rate, col_Cut_Amount });
            listView_PayCuts.Dock = DockStyle.Fill;
            listView_PayCuts.Font = new Font("Segoe UI", 9.5F);
            listView_PayCuts.ForeColor = Color.FromArgb(37, 48, 74);
            listView_PayCuts.FullRowSelect = true;
            listView_PayCuts.GridLines = true;
            listView_PayCuts.Location = new Point(10, 46);
            listView_PayCuts.Name = "listView_PayCuts";
            listView_PayCuts.Size = new Size(876, 689);
            listView_PayCuts.TabIndex = 0;
            listView_PayCuts.UseCompatibleStateImageBehavior = false;
            listView_PayCuts.View = View.Details;
            // 
            // col_Cut_Reason
            // 
            col_Cut_Reason.Text = "Lý do khấu trừ";
            col_Cut_Reason.Width = 240;
            // 
            // col_Cut_Desc
            // 
            col_Cut_Desc.Text = "Mô tả / Ghi chú";
            col_Cut_Desc.Width = 280;
            // 
            // col_Cut_Rate
            // 
            col_Cut_Rate.Text = "Tỷ lệ áp dụng";
            col_Cut_Rate.TextAlign = HorizontalAlignment.Center;
            col_Cut_Rate.Width = 130;
            // 
            // col_Cut_Amount
            // 
            col_Cut_Amount.Text = "Số tiền";
            col_Cut_Amount.TextAlign = HorizontalAlignment.Right;
            col_Cut_Amount.Width = 180;
            // 
            // panel_Bottom
            // 
            panel_Bottom.BackColor = Color.FromArgb(249, 250, 251);
            panel_Bottom.Controls.Add(label_Summary);
            panel_Bottom.Dock = DockStyle.Bottom;
            panel_Bottom.Location = new Point(0, 1049);
            panel_Bottom.Margin = new Padding(4);
            panel_Bottom.Name = "panel_Bottom";
            panel_Bottom.Size = new Size(2340, 77);
            panel_Bottom.TabIndex = 2;
            // 
            // label_Summary
            // 
            label_Summary.AutoSize = true;
            label_Summary.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            label_Summary.ForeColor = Color.FromArgb(31, 41, 55);
            label_Summary.Location = new Point(20, 19);
            label_Summary.Margin = new Padding(4, 0, 4, 0);
            label_Summary.Name = "label_Summary";
            label_Summary.Size = new Size(559, 41);
            label_Summary.TabIndex = 0;
            label_Summary.Text = "Tổng phụ cấp: 0 đ | Tổng khấu trừ: 0 đ";
            // 
            // PayrollAdjustmentControl
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(splitContainer_Main);
            Controls.Add(panel_Bottom);
            Controls.Add(panel_Top);
            Margin = new Padding(4);
            Name = "PayrollAdjustmentControl";
            Size = new Size(2340, 1126);
            panel_Top.ResumeLayout(false);
            panel_Top.PerformLayout();
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

namespace Admin.WinForms.Controls.Payroll
{
    partial class PayslipViewerControl
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
            comboBox_Employees = new ComboBox();
            button_ExportPayslip = new Button();
            button_Print = new Button();
            button_Reload = new Button();

            panel_CenterWrapper = new Panel();
            panel_SlipCard = new Panel();

            label_Company = new Label();
            label_SlipTitle = new Label();
            label_Divider = new Label();

            panel_EmpInfo = new Panel();
            label_EmpCodeVal = new Label();
            label_EmpNameVal = new Label();
            label_StatusVal = new Label();

            groupBox_SalaryWork = new GroupBox();
            label_BaseSalary = new Label();
            label_StandardDays = new Label();
            label_ActualDays = new Label();
            label_GrossSalary = new Label();
            label_ProjectAllowance = new Label();
            label_TotalAllowance = new Label();

            groupBox_Allowances = new GroupBox();
            listView_Allowances = new ListView();
            col_Allow_Name = new ColumnHeader();
            col_Allow_Amount = new ColumnHeader();

            groupBox_PayCuts = new GroupBox();
            listView_PayCuts = new ListView();
            col_Cut_Name = new ColumnHeader();
            col_Cut_Amount = new ColumnHeader();

            panel_NetSalaryBox = new Panel();
            label_NetTitle = new Label();
            label_NetValue = new Label();

            panel_Top.SuspendLayout();
            panel_CenterWrapper.SuspendLayout();
            panel_SlipCard.SuspendLayout();
            panel_EmpInfo.SuspendLayout();
            groupBox_SalaryWork.SuspendLayout();
            groupBox_Allowances.SuspendLayout();
            groupBox_PayCuts.SuspendLayout();
            panel_NetSalaryBox.SuspendLayout();
            SuspendLayout();

            // 
            // panel_Top
            // 
            panel_Top.Controls.Add(label_Period);
            panel_Top.Controls.Add(comboBox_Periods);
            panel_Top.Controls.Add(label_Employee);
            panel_Top.Controls.Add(comboBox_Employees);
            panel_Top.Controls.Add(button_ExportPayslip);
            panel_Top.Controls.Add(button_Print);
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

            comboBox_Employees.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_Employees.Font = new Font("Segoe UI", 10F);
            comboBox_Employees.Location = new Point(565, 19);
            comboBox_Employees.Name = "comboBox_Employees";
            comboBox_Employees.Size = new Size(350, 36);
            comboBox_Employees.TabIndex = 1;

            button_ExportPayslip.BackColor = Color.FromArgb(37, 99, 235);
            button_ExportPayslip.ForeColor = Color.White;
            button_ExportPayslip.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button_ExportPayslip.Location = new Point(935, 16);
            button_ExportPayslip.Name = "button_ExportPayslip";
            button_ExportPayslip.Size = new Size(170, 42);
            button_ExportPayslip.TabIndex = 2;
            button_ExportPayslip.Text = "Xuất Phiếu Lương";
            button_ExportPayslip.UseVisualStyleBackColor = false;

            button_Print.Font = new Font("Segoe UI", 9F);
            button_Print.Location = new Point(1115, 16);
            button_Print.Name = "button_Print";
            button_Print.Size = new Size(130, 42);
            button_Print.TabIndex = 3;
            button_Print.Text = "In Phiếu";
            button_Print.UseVisualStyleBackColor = true;

            button_Reload.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_Reload.Font = new Font("Segoe UI", 9F);
            button_Reload.Location = new Point(1670, 16);
            button_Reload.Name = "button_Reload";
            button_Reload.Size = new Size(110, 42);
            button_Reload.TabIndex = 4;
            button_Reload.Text = "Reload";
            button_Reload.UseVisualStyleBackColor = true;

            // 
            // panel_CenterWrapper
            // 
            panel_CenterWrapper.AutoScroll = true;
            panel_CenterWrapper.BackColor = Color.FromArgb(243, 244, 246);
            panel_CenterWrapper.Controls.Add(panel_SlipCard);
            panel_CenterWrapper.Dock = DockStyle.Fill;
            panel_CenterWrapper.Location = new Point(0, 75);
            panel_CenterWrapper.Name = "panel_CenterWrapper";
            panel_CenterWrapper.Padding = new Padding(20);
            panel_CenterWrapper.Size = new Size(1800, 805);
            panel_CenterWrapper.TabIndex = 1;

            // 
            // panel_SlipCard
            // 
            panel_SlipCard.BackColor = Color.White;
            panel_SlipCard.BorderStyle = BorderStyle.FixedSingle;
            panel_SlipCard.Controls.Add(panel_NetSalaryBox);
            panel_SlipCard.Controls.Add(groupBox_PayCuts);
            panel_SlipCard.Controls.Add(groupBox_Allowances);
            panel_SlipCard.Controls.Add(groupBox_SalaryWork);
            panel_SlipCard.Controls.Add(panel_EmpInfo);
            panel_SlipCard.Controls.Add(label_Divider);
            panel_SlipCard.Controls.Add(label_SlipTitle);
            panel_SlipCard.Controls.Add(label_Company);
            panel_SlipCard.Location = new Point(350, 15);
            panel_SlipCard.Name = "panel_SlipCard";
            panel_SlipCard.Padding = new Padding(30);
            panel_SlipCard.Size = new Size(1000, 750);
            panel_SlipCard.TabIndex = 0;

            // Header
            label_Company.AutoSize = true;
            label_Company.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label_Company.ForeColor = Color.FromArgb(107, 114, 128);
            label_Company.Location = new Point(30, 20);
            label_Company.Name = "label_Company";
            label_Company.Size = new Size(420, 28);
            label_Company.Text = "TẬP ĐOÀN CÔNG NGHỆ & NHÂN SỰ HRM";

            label_SlipTitle.AutoSize = true;
            label_SlipTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            label_SlipTitle.ForeColor = Color.FromArgb(30, 58, 138);
            label_SlipTitle.Location = new Point(30, 52);
            label_SlipTitle.Name = "label_SlipTitle";
            label_SlipTitle.Size = new Size(450, 45);
            label_SlipTitle.Text = "PHIẾU LƯƠNG NHÂN VIÊN";

            label_Divider.BackColor = Color.FromArgb(229, 231, 235);
            label_Divider.Location = new Point(30, 105);
            label_Divider.Name = "label_Divider";
            label_Divider.Size = new Size(940, 2);

            // Employee Info
            panel_EmpInfo.BackColor = Color.FromArgb(249, 250, 251);
            panel_EmpInfo.Controls.Add(label_EmpCodeVal);
            panel_EmpInfo.Controls.Add(label_EmpNameVal);
            panel_EmpInfo.Controls.Add(label_StatusVal);
            panel_EmpInfo.Location = new Point(30, 120);
            panel_EmpInfo.Name = "panel_EmpInfo";
            panel_EmpInfo.Size = new Size(940, 55);
            panel_EmpInfo.TabIndex = 0;

            label_EmpCodeVal.AutoSize = true;
            label_EmpCodeVal.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label_EmpCodeVal.Location = new Point(15, 12);
            label_EmpCodeVal.Name = "label_EmpCodeVal";
            label_EmpCodeVal.Size = new Size(130, 28);
            label_EmpCodeVal.Text = "Mã NV: --";

            label_EmpNameVal.AutoSize = true;
            label_EmpNameVal.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label_EmpNameVal.Location = new Point(250, 12);
            label_EmpNameVal.Name = "label_EmpNameVal";
            label_EmpNameVal.Size = new Size(160, 28);
            label_EmpNameVal.Text = "Họ và Tên: --";

            label_StatusVal.AutoSize = true;
            label_StatusVal.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label_StatusVal.ForeColor = Color.FromArgb(16, 149, 74);
            label_StatusVal.Location = new Point(650, 12);
            label_StatusVal.Name = "label_StatusVal";
            label_StatusVal.Size = new Size(150, 28);
            label_StatusVal.Text = "Trạng thái: --";

            // Salary & Work Days
            groupBox_SalaryWork.Controls.Add(label_BaseSalary);
            groupBox_SalaryWork.Controls.Add(label_StandardDays);
            groupBox_SalaryWork.Controls.Add(label_ActualDays);
            groupBox_SalaryWork.Controls.Add(label_GrossSalary);
            groupBox_SalaryWork.Controls.Add(label_ProjectAllowance);
            groupBox_SalaryWork.Controls.Add(label_TotalAllowance);
            groupBox_SalaryWork.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            groupBox_SalaryWork.ForeColor = Color.FromArgb(55, 65, 81);
            groupBox_SalaryWork.Location = new Point(30, 190);
            groupBox_SalaryWork.Name = "groupBox_SalaryWork";
            groupBox_SalaryWork.Size = new Size(940, 130);
            groupBox_SalaryWork.TabIndex = 1;
            groupBox_SalaryWork.TabStop = false;
            groupBox_SalaryWork.Text = "1. Lương & Ngày công";

            label_BaseSalary.AutoSize = true;
            label_BaseSalary.Font = new Font("Segoe UI", 9.5F);
            label_BaseSalary.Location = new Point(20, 35);
            label_BaseSalary.Name = "label_BaseSalary";
            label_BaseSalary.Size = new Size(160, 25);
            label_BaseSalary.Text = "Lương cơ bản: 0 đ";

            label_StandardDays.AutoSize = true;
            label_StandardDays.Font = new Font("Segoe UI", 9.5F);
            label_StandardDays.Location = new Point(20, 70);
            label_StandardDays.Name = "label_StandardDays";
            label_StandardDays.Size = new Size(150, 25);
            label_StandardDays.Text = "Công chuẩn: 0";

            label_ActualDays.AutoSize = true;
            label_ActualDays.Font = new Font("Segoe UI", 9.5F);
            label_ActualDays.Location = new Point(480, 35);
            label_ActualDays.Name = "label_ActualDays";
            label_ActualDays.Size = new Size(150, 25);
            label_ActualDays.Text = "Công thực tế: 0";

            label_GrossSalary.AutoSize = true;
            label_GrossSalary.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            label_GrossSalary.ForeColor = Color.FromArgb(37, 99, 235);
            label_GrossSalary.Location = new Point(480, 70);
            label_GrossSalary.Name = "label_GrossSalary";
            label_GrossSalary.Size = new Size(160, 25);
            label_GrossSalary.Text = "Lương Gross: 0 đ";

            label_ProjectAllowance.AutoSize = true;
            label_ProjectAllowance.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            label_ProjectAllowance.ForeColor = Color.FromArgb(16, 149, 74);
            label_ProjectAllowance.Location = new Point(20, 100);
            label_ProjectAllowance.Name = "label_ProjectAllowance";
            label_ProjectAllowance.Size = new Size(240, 25);
            label_ProjectAllowance.Text = "Tổng phụ cấp dự án: 0 đ";

            label_TotalAllowance.AutoSize = true;
            label_TotalAllowance.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            label_TotalAllowance.ForeColor = Color.FromArgb(16, 149, 74);
            label_TotalAllowance.Location = new Point(480, 100);
            label_TotalAllowance.Name = "label_TotalAllowance";
            label_TotalAllowance.Size = new Size(200, 25);
            label_TotalAllowance.Text = "Tổng phụ cấp: 0 đ";

            // Allowances
            groupBox_Allowances.Controls.Add(listView_Allowances);
            groupBox_Allowances.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            groupBox_Allowances.ForeColor = Color.FromArgb(16, 149, 74);
            groupBox_Allowances.Location = new Point(30, 330);
            groupBox_Allowances.Name = "groupBox_Allowances";
            groupBox_Allowances.Size = new Size(460, 270);
            groupBox_Allowances.TabIndex = 2;
            groupBox_Allowances.TabStop = false;
            groupBox_Allowances.Text = "2. Khoản cộng (Phụ cấp)";

            listView_Allowances.BorderStyle = BorderStyle.None;
            listView_Allowances.Columns.AddRange(new ColumnHeader[] { col_Allow_Name, col_Allow_Amount });
            listView_Allowances.Dock = DockStyle.Fill;
            listView_Allowances.Font = new Font("Segoe UI", 9F);
            listView_Allowances.FullRowSelect = true;
            listView_Allowances.GridLines = true;
            listView_Allowances.Location = new Point(3, 30);
            listView_Allowances.Name = "listView_Allowances";
            listView_Allowances.Size = new Size(454, 237);
            listView_Allowances.TabIndex = 0;
            listView_Allowances.UseCompatibleStateImageBehavior = false;
            listView_Allowances.View = View.Details;

            col_Allow_Name.Text = "Tên phụ cấp";
            col_Allow_Name.Width = 260;

            col_Allow_Amount.Text = "Số tiền";
            col_Allow_Amount.TextAlign = HorizontalAlignment.Right;
            col_Allow_Amount.Width = 160;

            // PayCuts
            groupBox_PayCuts.Controls.Add(listView_PayCuts);
            groupBox_PayCuts.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            groupBox_PayCuts.ForeColor = Color.FromArgb(220, 38, 38);
            groupBox_PayCuts.Location = new Point(510, 330);
            groupBox_PayCuts.Name = "groupBox_PayCuts";
            groupBox_PayCuts.Size = new Size(460, 270);
            groupBox_PayCuts.TabIndex = 3;
            groupBox_PayCuts.TabStop = false;
            groupBox_PayCuts.Text = "3. Khoản trừ (Bảo hiểm/Phạt)";

            listView_PayCuts.BorderStyle = BorderStyle.None;
            listView_PayCuts.Columns.AddRange(new ColumnHeader[] { col_Cut_Name, col_Cut_Amount });
            listView_PayCuts.Dock = DockStyle.Fill;
            listView_PayCuts.Font = new Font("Segoe UI", 9F);
            listView_PayCuts.FullRowSelect = true;
            listView_PayCuts.GridLines = true;
            listView_PayCuts.Location = new Point(3, 30);
            listView_PayCuts.Name = "listView_PayCuts";
            listView_PayCuts.Size = new Size(454, 237);
            listView_PayCuts.TabIndex = 0;
            listView_PayCuts.UseCompatibleStateImageBehavior = false;
            listView_PayCuts.View = View.Details;

            col_Cut_Name.Text = "Lý do khấu trừ";
            col_Cut_Name.Width = 260;

            col_Cut_Amount.Text = "Số tiền";
            col_Cut_Amount.TextAlign = HorizontalAlignment.Right;
            col_Cut_Amount.Width = 160;

            // Net Salary Box
            panel_NetSalaryBox.BackColor = Color.FromArgb(236, 249, 240);
            panel_NetSalaryBox.BorderStyle = BorderStyle.FixedSingle;
            panel_NetSalaryBox.Controls.Add(label_NetTitle);
            panel_NetSalaryBox.Controls.Add(label_NetValue);
            panel_NetSalaryBox.Location = new Point(30, 605);
            panel_NetSalaryBox.Name = "panel_NetSalaryBox";
            panel_NetSalaryBox.Size = new Size(940, 95);
            panel_NetSalaryBox.TabIndex = 4;

            label_NetTitle.AutoSize = true;
            label_NetTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label_NetTitle.ForeColor = Color.FromArgb(16, 149, 74);
            label_NetTitle.Location = new Point(25, 30);
            label_NetTitle.Name = "label_NetTitle";
            label_NetTitle.Size = new Size(330, 32);
            label_NetTitle.Text = "THỰC LĨNH (NET SALARY):";

            label_NetValue.AutoSize = true;
            label_NetValue.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            label_NetValue.ForeColor = Color.FromArgb(16, 149, 74);
            label_NetValue.Location = new Point(480, 20);
            label_NetValue.Name = "label_NetValue";
            label_NetValue.Size = new Size(80, 48);
            label_NetValue.Text = "0 đ";

            // 
            // PayslipViewerControl
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(panel_CenterWrapper);
            Controls.Add(panel_Top);
            Name = "PayslipViewerControl";
            Size = new Size(1800, 880);

            panel_Top.ResumeLayout(false);
            panel_Top.PerformLayout();
            panel_CenterWrapper.ResumeLayout(false);
            panel_SlipCard.ResumeLayout(false);
            panel_SlipCard.PerformLayout();
            panel_EmpInfo.ResumeLayout(false);
            panel_EmpInfo.PerformLayout();
            groupBox_SalaryWork.ResumeLayout(false);
            groupBox_SalaryWork.PerformLayout();
            groupBox_Allowances.ResumeLayout(false);
            groupBox_PayCuts.ResumeLayout(false);
            panel_NetSalaryBox.ResumeLayout(false);
            panel_NetSalaryBox.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel_Top;
        private Label label_Period;
        private ComboBox comboBox_Periods;
        private Label label_Employee;
        private ComboBox comboBox_Employees;
        private Button button_ExportPayslip;
        private Button button_Print;
        private Button button_Reload;

        private Panel panel_CenterWrapper;
        private Panel panel_SlipCard;
        private Label label_Company;
        private Label label_SlipTitle;
        private Label label_Divider;

        private Panel panel_EmpInfo;
        private Label label_EmpCodeVal;
        private Label label_EmpNameVal;
        private Label label_StatusVal;

        private GroupBox groupBox_SalaryWork;
        private Label label_BaseSalary;
        private Label label_StandardDays;
        private Label label_ActualDays;
        private Label label_GrossSalary;
        private Label label_ProjectAllowance;
        private Label label_TotalAllowance;

        private GroupBox groupBox_Allowances;
        private ListView listView_Allowances;
        private ColumnHeader col_Allow_Name;
        private ColumnHeader col_Allow_Amount;

        private GroupBox groupBox_PayCuts;
        private ListView listView_PayCuts;
        private ColumnHeader col_Cut_Name;
        private ColumnHeader col_Cut_Amount;

        private Panel panel_NetSalaryBox;
        private Label label_NetTitle;
        private Label label_NetValue;
    }
}

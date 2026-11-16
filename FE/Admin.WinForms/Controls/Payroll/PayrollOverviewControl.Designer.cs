namespace Admin.WinForms.Controls.Payroll
{
    partial class PayrollOverviewControl
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
            flowLayoutPanel_Nav = new FlowLayoutPanel();
            button_Table = new Button();
            button_Adjustments = new Button();
            button_Approval = new Button();
            button_Payslip = new Button();
            panel_Payroll = new Panel();
            flowLayoutPanel_Nav.SuspendLayout();
            SuspendLayout();

            // 
            // flowLayoutPanel_Nav
            // 
            flowLayoutPanel_Nav.Controls.Add(button_Table);
            flowLayoutPanel_Nav.Controls.Add(button_Adjustments);
            flowLayoutPanel_Nav.Controls.Add(button_Approval);
            flowLayoutPanel_Nav.Controls.Add(button_Payslip);
            flowLayoutPanel_Nav.Dock = DockStyle.Top;
            flowLayoutPanel_Nav.Location = new Point(0, 0);
            flowLayoutPanel_Nav.Name = "flowLayoutPanel_Nav";
            flowLayoutPanel_Nav.Padding = new Padding(10, 5, 0, 5);
            flowLayoutPanel_Nav.AutoSize = true;
            flowLayoutPanel_Nav.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flowLayoutPanel_Nav.WrapContents = true;
            flowLayoutPanel_Nav.Size = new Size(1815, 75);
            flowLayoutPanel_Nav.TabIndex = 0;

            // 
            // button_Table
            // 
            button_Table.BackColor = Color.FromArgb(35, 83, 166);
            button_Table.FlatAppearance.BorderSize = 0;
            button_Table.FlatStyle = FlatStyle.Flat;
            button_Table.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            button_Table.ForeColor = Color.White;
            button_Table.Location = new Point(13, 8);
            button_Table.Name = "button_Table";
            button_Table.Size = new Size(270, 58);
            button_Table.TabIndex = 0;
            button_Table.Text = "Bảng lương & Tính toán";
            button_Table.UseVisualStyleBackColor = false;
            button_Table.Click += button_Table_Click;

            // 
            // button_Adjustments
            // 
            button_Adjustments.BackColor = Color.Transparent;
            button_Adjustments.FlatAppearance.BorderSize = 0;
            button_Adjustments.FlatStyle = FlatStyle.Flat;
            button_Adjustments.Font = new Font("Segoe UI", 10F);
            button_Adjustments.ForeColor = Color.Black;
            button_Adjustments.Location = new Point(289, 8);
            button_Adjustments.Name = "button_Adjustments";
            button_Adjustments.Size = new Size(270, 58);
            button_Adjustments.TabIndex = 1;
            button_Adjustments.Text = "Phụ cấp & Khấu trừ";
            button_Adjustments.UseVisualStyleBackColor = false;
            button_Adjustments.Click += button_Adjustments_Click;

            // 
            // button_Approval
            // 
            button_Approval.BackColor = Color.Transparent;
            button_Approval.FlatAppearance.BorderSize = 0;
            button_Approval.FlatStyle = FlatStyle.Flat;
            button_Approval.Font = new Font("Segoe UI", 10F);
            button_Approval.ForeColor = Color.Black;
            button_Approval.Location = new Point(565, 8);
            button_Approval.Name = "button_Approval";
            button_Approval.Size = new Size(280, 58);
            button_Approval.TabIndex = 2;
            button_Approval.Text = "Phê duyệt & Quyết toán";
            button_Approval.UseVisualStyleBackColor = false;
            button_Approval.Click += button_Approval_Click;

            // 
            // button_Payslip
            // 
            button_Payslip.BackColor = Color.Transparent;
            button_Payslip.FlatAppearance.BorderSize = 0;
            button_Payslip.FlatStyle = FlatStyle.Flat;
            button_Payslip.Font = new Font("Segoe UI", 10F);
            button_Payslip.ForeColor = Color.Black;
            button_Payslip.Location = new Point(851, 8);
            button_Payslip.Name = "button_Payslip";
            button_Payslip.Size = new Size(270, 58);
            button_Payslip.TabIndex = 3;
            button_Payslip.Text = "Phiếu lương (Payslip)";
            button_Payslip.UseVisualStyleBackColor = false;
            button_Payslip.Click += button_Payslip_Click;

            // 
            // panel_Payroll
            // 
            panel_Payroll.Dock = DockStyle.Fill;
            panel_Payroll.Location = new Point(0, 75);
            panel_Payroll.Name = "panel_Payroll";
            panel_Payroll.Size = new Size(1815, 809);
            panel_Payroll.TabIndex = 1;

            // 
            // PayrollOverviewControl
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(panel_Payroll);
            Controls.Add(flowLayoutPanel_Nav);
            Name = "PayrollOverviewControl";
            Size = new Size(1815, 884);

            flowLayoutPanel_Nav.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private FlowLayoutPanel flowLayoutPanel_Nav;
        private Button button_Table;
        private Button button_Adjustments;
        private Button button_Approval;
        private Button button_Payslip;
        private Panel panel_Payroll;
    }
}

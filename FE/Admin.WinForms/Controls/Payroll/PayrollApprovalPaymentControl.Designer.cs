namespace Admin.WinForms.Controls.Payroll
{
    partial class PayrollApprovalPaymentControl
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
            panel_TopCards = new Panel();
            tableLayout_TopCards = new TableLayoutPanel();
            panel_Calculated = new Panel();
            label_CountCalculated = new Label();
            label_TitleCalculated = new Label();
            panel_Approved = new Panel();
            label_CountApproved = new Label();
            label_TitleApproved = new Label();
            panel_Paid = new Panel();
            label_CountPaid = new Label();
            label_TitlePaid = new Label();
            panel_Toolbar = new Panel();
            comboBox_Periods = new ComboBox();
            button_ApproveSelected = new Button();
            button_PaySelected = new Button();
            button_ApproveAll = new Button();
            button_PayAll = new Button();
            button_LockPeriod = new Button();
            button_Reload = new Button();
            listView_Approvals = new ListView();
            col_EmpCode = new ColumnHeader();
            col_EmpName = new ColumnHeader();
            col_Gross = new ColumnHeader();
            col_Allowance = new ColumnHeader();
            col_PayCut = new ColumnHeader();
            col_Net = new ColumnHeader();
            col_Status = new ColumnHeader();
            col_ApprovedAt = new ColumnHeader();
            col_PaidAt = new ColumnHeader();
            panel_TopCards.SuspendLayout();
            tableLayout_TopCards.SuspendLayout();
            panel_Calculated.SuspendLayout();
            panel_Approved.SuspendLayout();
            panel_Paid.SuspendLayout();
            panel_Toolbar.SuspendLayout();
            SuspendLayout();
            // 
            // panel_TopCards
            // 
            panel_TopCards.Controls.Add(tableLayout_TopCards);
            panel_TopCards.Dock = DockStyle.Top;
            panel_TopCards.Location = new Point(0, 0);
            panel_TopCards.Name = "panel_TopCards";
            panel_TopCards.Size = new Size(2340, 154);
            panel_TopCards.TabIndex = 0;
            // 
            // tableLayout_TopCards
            // 
            tableLayout_TopCards.ColumnCount = 3;
            tableLayout_TopCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
            tableLayout_TopCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
            tableLayout_TopCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
            tableLayout_TopCards.Controls.Add(panel_Calculated, 0, 0);
            tableLayout_TopCards.Controls.Add(panel_Approved, 1, 0);
            tableLayout_TopCards.Controls.Add(panel_Paid, 2, 0);
            tableLayout_TopCards.Dock = DockStyle.Fill;
            tableLayout_TopCards.Location = new Point(0, 0);
            tableLayout_TopCards.Name = "tableLayout_TopCards";
            tableLayout_TopCards.RowCount = 1;
            tableLayout_TopCards.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayout_TopCards.Size = new Size(2340, 154);
            tableLayout_TopCards.TabIndex = 0;
            // 
            // panel_Calculated
            // 
            panel_Calculated.BackColor = Color.FromArgb(254, 244, 234);
            panel_Calculated.Controls.Add(label_CountCalculated);
            panel_Calculated.Controls.Add(label_TitleCalculated);
            panel_Calculated.Dock = DockStyle.Fill;
            panel_Calculated.Location = new Point(10, 10);
            panel_Calculated.Margin = new Padding(10, 10, 10, 10);
            panel_Calculated.Name = "panel_Calculated";
            panel_Calculated.Size = new Size(759, 134);
            panel_Calculated.TabIndex = 0;
            // 
            // label_CountCalculated
            // 
            label_CountCalculated.AutoSize = true;
            label_CountCalculated.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            label_CountCalculated.ForeColor = Color.FromArgb(217, 119, 6);
            label_CountCalculated.Location = new Point(20, 13);
            label_CountCalculated.Name = "label_CountCalculated";
            label_CountCalculated.Size = new Size(50, 59);
            label_CountCalculated.TabIndex = 0;
            label_CountCalculated.Text = "0";
            // 
            // label_TitleCalculated
            // 
            label_TitleCalculated.AutoSize = true;
            label_TitleCalculated.Font = new Font("Segoe UI", 10F);
            label_TitleCalculated.ForeColor = Color.FromArgb(75, 85, 99);
            label_TitleCalculated.Location = new Point(20, 70);
            label_TitleCalculated.Name = "label_TitleCalculated";
            label_TitleCalculated.Size = new Size(343, 37);
            label_TitleCalculated.TabIndex = 1;
            label_TitleCalculated.Text = "Bảng Lương Chờ Phê Duyệt";
            // 
            // panel_Approved
            // 
            panel_Approved.BackColor = Color.FromArgb(229, 238, 253);
            panel_Approved.Controls.Add(label_CountApproved);
            panel_Approved.Controls.Add(label_TitleApproved);
            panel_Approved.Dock = DockStyle.Fill;
            panel_Approved.Location = new Point(789, 10);
            panel_Approved.Margin = new Padding(10, 10, 10, 10);
            panel_Approved.Name = "panel_Approved";
            panel_Approved.Size = new Size(759, 134);
            panel_Approved.TabIndex = 1;
            // 
            // label_CountApproved
            // 
            label_CountApproved.AutoSize = true;
            label_CountApproved.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            label_CountApproved.ForeColor = Color.FromArgb(37, 99, 235);
            label_CountApproved.Location = new Point(20, 13);
            label_CountApproved.Name = "label_CountApproved";
            label_CountApproved.Size = new Size(50, 59);
            label_CountApproved.TabIndex = 0;
            label_CountApproved.Text = "0";
            // 
            // label_TitleApproved
            // 
            label_TitleApproved.AutoSize = true;
            label_TitleApproved.Font = new Font("Segoe UI", 10F);
            label_TitleApproved.ForeColor = Color.FromArgb(75, 85, 99);
            label_TitleApproved.Location = new Point(20, 70);
            label_TitleApproved.Name = "label_TitleApproved";
            label_TitleApproved.Size = new Size(295, 37);
            label_TitleApproved.TabIndex = 1;
            label_TitleApproved.Text = "Đã Phê Duyệt (Chờ Chi)";
            // 
            // panel_Paid
            // 
            panel_Paid.BackColor = Color.FromArgb(236, 249, 240);
            panel_Paid.Controls.Add(label_CountPaid);
            panel_Paid.Controls.Add(label_TitlePaid);
            panel_Paid.Dock = DockStyle.Fill;
            panel_Paid.Location = new Point(1568, 10);
            panel_Paid.Margin = new Padding(10, 10, 10, 10);
            panel_Paid.Name = "panel_Paid";
            panel_Paid.Size = new Size(762, 134);
            panel_Paid.TabIndex = 2;
            // 
            // label_CountPaid
            // 
            label_CountPaid.AutoSize = true;
            label_CountPaid.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            label_CountPaid.ForeColor = Color.FromArgb(16, 149, 74);
            label_CountPaid.Location = new Point(20, 13);
            label_CountPaid.Name = "label_CountPaid";
            label_CountPaid.Size = new Size(50, 59);
            label_CountPaid.TabIndex = 0;
            label_CountPaid.Text = "0";
            // 
            // label_TitlePaid
            // 
            label_TitlePaid.AutoSize = true;
            label_TitlePaid.Font = new Font("Segoe UI", 10F);
            label_TitlePaid.ForeColor = Color.FromArgb(75, 85, 99);
            label_TitlePaid.Location = new Point(20, 70);
            label_TitlePaid.Name = "label_TitlePaid";
            label_TitlePaid.Size = new Size(336, 37);
            label_TitlePaid.TabIndex = 1;
            label_TitlePaid.Text = "Đã Hoàn Tất Chi Trả Lương";
            // 
            // panel_Toolbar
            // 
            panel_Toolbar.Controls.Add(comboBox_Periods);
            panel_Toolbar.Controls.Add(button_ApproveSelected);
            panel_Toolbar.Controls.Add(button_PaySelected);
            panel_Toolbar.Controls.Add(button_ApproveAll);
            panel_Toolbar.Controls.Add(button_PayAll);
            panel_Toolbar.Controls.Add(button_LockPeriod);
            panel_Toolbar.Controls.Add(button_Reload);
            panel_Toolbar.Dock = DockStyle.Top;
            panel_Toolbar.Location = new Point(0, 154);
            panel_Toolbar.Name = "panel_Toolbar";
            panel_Toolbar.Size = new Size(2340, 90);
            panel_Toolbar.TabIndex = 1;
            // 
            // comboBox_Periods
            // 
            comboBox_Periods.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_Periods.Font = new Font("Segoe UI", 10F);
            comboBox_Periods.Location = new Point(10, 21);
            comboBox_Periods.Name = "comboBox_Periods";
            comboBox_Periods.Size = new Size(541, 45);
            comboBox_Periods.TabIndex = 0;
            // 
            // button_ApproveSelected
            // 
            button_ApproveSelected.BackColor = Color.FromArgb(37, 99, 235);
            button_ApproveSelected.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button_ApproveSelected.ForeColor = Color.White;
            button_ApproveSelected.Location = new Point(578, 18);
            button_ApproveSelected.Name = "button_ApproveSelected";
            button_ApproveSelected.Size = new Size(221, 54);
            button_ApproveSelected.TabIndex = 1;
            button_ApproveSelected.Text = "✔ Duyệt Dòng Chọn";
            button_ApproveSelected.UseVisualStyleBackColor = false;
            // 
            // button_PaySelected
            // 
            button_PaySelected.BackColor = Color.FromArgb(16, 185, 129);
            button_PaySelected.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button_PaySelected.ForeColor = Color.White;
            button_PaySelected.Location = new Point(812, 18);
            button_PaySelected.Name = "button_PaySelected";
            button_PaySelected.Size = new Size(228, 54);
            button_PaySelected.TabIndex = 2;
            button_PaySelected.Text = "💵 Chi Trả Dòng Chọn";
            button_PaySelected.UseVisualStyleBackColor = false;
            // 
            // button_ApproveAll
            // 
            button_ApproveAll.BackColor = Color.FromArgb(59, 130, 246);
            button_ApproveAll.Font = new Font("Segoe UI", 9F);
            button_ApproveAll.ForeColor = Color.White;
            button_ApproveAll.Location = new Point(1053, 18);
            button_ApproveAll.Name = "button_ApproveAll";
            button_ApproveAll.Size = new Size(221, 54);
            button_ApproveAll.TabIndex = 3;
            button_ApproveAll.Text = "✔ Phê Duyệt Tất Cả";
            button_ApproveAll.UseVisualStyleBackColor = false;
            // 
            // button_PayAll
            // 
            button_PayAll.BackColor = Color.FromArgb(34, 197, 94);
            button_PayAll.Font = new Font("Segoe UI", 9F);
            button_PayAll.ForeColor = Color.White;
            button_PayAll.Location = new Point(1287, 18);
            button_PayAll.Name = "button_PayAll";
            button_PayAll.Size = new Size(234, 54);
            button_PayAll.TabIndex = 4;
            button_PayAll.Text = "💵 Quyết Toán Tất Cả";
            button_PayAll.UseVisualStyleBackColor = false;
            // 
            // button_LockPeriod
            // 
            button_LockPeriod.BackColor = Color.FromArgb(107, 114, 128);
            button_LockPeriod.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button_LockPeriod.ForeColor = Color.White;
            button_LockPeriod.Location = new Point(1534, 18);
            button_LockPeriod.Name = "button_LockPeriod";
            button_LockPeriod.Size = new Size(247, 54);
            button_LockPeriod.TabIndex = 5;
            button_LockPeriod.Text = "🔒 Chốt / Khóa Kỳ Lương";
            button_LockPeriod.UseVisualStyleBackColor = false;
            // 
            // button_Reload
            // 
            button_Reload.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_Reload.Font = new Font("Segoe UI", 9F);
            button_Reload.Location = new Point(2171, 18);
            button_Reload.Name = "button_Reload";
            button_Reload.Size = new Size(143, 54);
            button_Reload.TabIndex = 6;
            button_Reload.Text = "Reload";
            button_Reload.UseVisualStyleBackColor = true;
            // 
            // listView_Approvals
            // 
            listView_Approvals.BorderStyle = BorderStyle.FixedSingle;
            listView_Approvals.Columns.AddRange(new ColumnHeader[] { col_EmpCode, col_EmpName, col_Gross, col_Allowance, col_PayCut, col_Net, col_Status, col_ApprovedAt, col_PaidAt });
            listView_Approvals.Dock = DockStyle.Fill;
            listView_Approvals.Font = new Font("Segoe UI", 9.5F);
            listView_Approvals.FullRowSelect = true;
            listView_Approvals.GridLines = true;
            listView_Approvals.Location = new Point(0, 244);
            listView_Approvals.Name = "listView_Approvals";
            listView_Approvals.Size = new Size(2340, 882);
            listView_Approvals.TabIndex = 2;
            listView_Approvals.UseCompatibleStateImageBehavior = false;
            listView_Approvals.View = View.Details;
            listView_Approvals.SelectedIndexChanged += listView_Approvals_SelectedIndexChanged;
            // 
            // col_EmpCode
            // 
            col_EmpCode.Text = "Mã NV";
            col_EmpCode.Width = 120;
            // 
            // col_EmpName
            // 
            col_EmpName.Text = "Họ và Tên";
            col_EmpName.Width = 240;
            // 
            // col_Gross
            // 
            col_Gross.Text = "Lương Gross";
            col_Gross.TextAlign = HorizontalAlignment.Right;
            col_Gross.Width = 180;
            // 
            // col_Allowance
            // 
            col_Allowance.Text = "Tổng phụ cấp";
            col_Allowance.TextAlign = HorizontalAlignment.Right;
            col_Allowance.Width = 160;
            // 
            // col_PayCut
            // 
            col_PayCut.Text = "Khấu trừ/Phạt";
            col_PayCut.TextAlign = HorizontalAlignment.Right;
            col_PayCut.Width = 160;
            // 
            // col_Net
            // 
            col_Net.Text = "Thực Lĩnh (NET)";
            col_Net.TextAlign = HorizontalAlignment.Right;
            col_Net.Width = 200;
            // 
            // col_Status
            // 
            col_Status.Text = "Trạng thái";
            col_Status.TextAlign = HorizontalAlignment.Center;
            col_Status.Width = 150;
            // 
            // col_ApprovedAt
            // 
            col_ApprovedAt.Text = "Ngày phê duyệt";
            col_ApprovedAt.Width = 180;
            // 
            // col_PaidAt
            // 
            col_PaidAt.Text = "Ngày chi trả";
            col_PaidAt.Width = 180;
            // 
            // PayrollApprovalPaymentControl
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(listView_Approvals);
            Controls.Add(panel_Toolbar);
            Controls.Add(panel_TopCards);
            Name = "PayrollApprovalPaymentControl";
            Size = new Size(2340, 1126);
            panel_TopCards.ResumeLayout(false);
            tableLayout_TopCards.ResumeLayout(false);
            panel_Calculated.ResumeLayout(false);
            panel_Calculated.PerformLayout();
            panel_Approved.ResumeLayout(false);
            panel_Approved.PerformLayout();
            panel_Paid.ResumeLayout(false);
            panel_Paid.PerformLayout();
            panel_Toolbar.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panel_TopCards;
        private TableLayoutPanel tableLayout_TopCards;
        private Panel panel_Calculated;
        private Label label_CountCalculated;
        private Label label_TitleCalculated;
        private Panel panel_Approved;
        private Label label_CountApproved;
        private Label label_TitleApproved;
        private Panel panel_Paid;
        private Label label_CountPaid;
        private Label label_TitlePaid;

        private Panel panel_Toolbar;
        private ComboBox comboBox_Periods;
        private Button button_ApproveSelected;
        private Button button_PaySelected;
        private Button button_ApproveAll;
        private Button button_PayAll;
        private Button button_LockPeriod;
        private Button button_Reload;

        private ListView listView_Approvals;
        private ColumnHeader col_EmpCode;
        private ColumnHeader col_EmpName;
        private ColumnHeader col_Gross;
        private ColumnHeader col_Allowance;
        private ColumnHeader col_PayCut;
        private ColumnHeader col_Net;
        private ColumnHeader col_Status;
        private ColumnHeader col_ApprovedAt;
        private ColumnHeader col_PaidAt;
    }
}

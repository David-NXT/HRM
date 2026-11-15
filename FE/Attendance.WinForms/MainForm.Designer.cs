namespace Attendance.WinForms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _clockTimer?.Dispose();
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            panel_Top = new Panel();
            label_EmpBadge = new Label();
            comboBox_Employees = new ComboBox();
            label_SelectEmp = new Label();
            label_Clock = new Label();
            label_HeaderSubtitle = new Label();
            label_HeaderTitle = new Label();
            panel_NavBar = new FlowLayoutPanel();
            button_NavCheckIn = new Button();
            button_NavRequests = new Button();
            button_NavRules = new Button();
            panel_SubHeader = new Panel();
            label_TabDesc = new Label();
            label_TabTitle = new Label();
            panel_MainContent = new Panel();
            panel_Top.SuspendLayout();
            panel_NavBar.SuspendLayout();
            panel_SubHeader.SuspendLayout();
            SuspendLayout();
            // 
            // panel_Top
            // 
            panel_Top.BackColor = Color.FromArgb(15, 23, 42);
            panel_Top.Controls.Add(label_EmpBadge);
            panel_Top.Controls.Add(comboBox_Employees);
            panel_Top.Controls.Add(label_SelectEmp);
            panel_Top.Controls.Add(label_Clock);
            panel_Top.Controls.Add(label_HeaderSubtitle);
            panel_Top.Controls.Add(label_HeaderTitle);
            panel_Top.Dock = DockStyle.Top;
            panel_Top.Location = new Point(0, 0);
            panel_Top.Name = "panel_Top";
            panel_Top.Padding = new Padding(20, 15, 20, 15);
            panel_Top.Size = new Size(1400, 105);
            panel_Top.TabIndex = 0;
            // 
            // label_EmpBadge
            // 
            label_EmpBadge.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label_EmpBadge.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            label_EmpBadge.ForeColor = Color.FromArgb(147, 197, 253);
            label_EmpBadge.Location = new Point(650, 68);
            label_EmpBadge.Name = "label_EmpBadge";
            label_EmpBadge.Size = new Size(730, 26);
            label_EmpBadge.TabIndex = 5;
            label_EmpBadge.Text = "👤 Đang tải thông tin nhân viên...";
            label_EmpBadge.TextAlign = ContentAlignment.MiddleRight;
            // 
            // comboBox_Employees
            // 
            comboBox_Employees.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            comboBox_Employees.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_Employees.Font = new Font("Segoe UI", 10F);
            comboBox_Employees.FormattingEnabled = true;
            comboBox_Employees.Location = new Point(810, 24);
            comboBox_Employees.Name = "comboBox_Employees";
            comboBox_Employees.Size = new Size(570, 31);
            comboBox_Employees.TabIndex = 4;
            comboBox_Employees.SelectedIndexChanged += comboBox_Employees_SelectedIndexChanged;
            // 
            // label_SelectEmp
            // 
            label_SelectEmp.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label_SelectEmp.AutoSize = true;
            label_SelectEmp.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            label_SelectEmp.ForeColor = Color.FromArgb(226, 232, 240);
            label_SelectEmp.Location = new Point(680, 28);
            label_SelectEmp.Name = "label_SelectEmp";
            label_SelectEmp.Size = new Size(122, 21);
            label_SelectEmp.TabIndex = 3;
            label_SelectEmp.Text = "Tài khoản NV:";
            // 
            // label_Clock
            // 
            label_Clock.AutoSize = true;
            label_Clock.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            label_Clock.ForeColor = Color.FromArgb(251, 191, 36);
            label_Clock.Location = new Point(22, 68);
            label_Clock.Name = "label_Clock";
            label_Clock.Size = new Size(215, 25);
            label_Clock.TabIndex = 2;
            label_Clock.Text = "00:00:00 | 01/01/2026";
            // 
            // label_HeaderSubtitle
            // 
            label_HeaderSubtitle.AutoSize = true;
            label_HeaderSubtitle.Font = new Font("Segoe UI", 9.5F);
            label_HeaderSubtitle.ForeColor = Color.FromArgb(148, 163, 184);
            label_HeaderSubtitle.Location = new Point(22, 44);
            label_HeaderSubtitle.Name = "label_HeaderSubtitle";
            label_HeaderSubtitle.Size = new Size(415, 21);
            label_HeaderSubtitle.TabIndex = 1;
            label_HeaderSubtitle.Text = "Hệ thống tự phục vụ chấm công & nộp đơn trực tuyến nội bộ";
            // 
            // label_HeaderTitle
            // 
            label_HeaderTitle.AutoSize = true;
            label_HeaderTitle.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            label_HeaderTitle.ForeColor = Color.White;
            label_HeaderTitle.Location = new Point(20, 10);
            label_HeaderTitle.Name = "label_HeaderTitle";
            label_HeaderTitle.Size = new Size(545, 35);
            label_HeaderTitle.TabIndex = 0;
            label_HeaderTitle.Text = "CỔNG CHẤM CÔNG & DỊCH VỤ NHÂN VIÊN";
            // 
            // panel_NavBar
            // 
            panel_NavBar.BackColor = Color.FromArgb(30, 41, 59);
            panel_NavBar.Controls.Add(button_NavCheckIn);
            panel_NavBar.Controls.Add(button_NavRequests);
            panel_NavBar.Controls.Add(button_NavRules);
            panel_NavBar.Dock = DockStyle.Top;
            panel_NavBar.Location = new Point(0, 105);
            panel_NavBar.Name = "panel_NavBar";
            panel_NavBar.Padding = new Padding(15, 5, 15, 5);
            panel_NavBar.Size = new Size(1400, 56);
            panel_NavBar.TabIndex = 1;
            // 
            // button_NavCheckIn
            // 
            button_NavCheckIn.BackColor = Color.FromArgb(37, 99, 235);
            button_NavCheckIn.FlatAppearance.BorderSize = 0;
            button_NavCheckIn.FlatStyle = FlatStyle.Flat;
            button_NavCheckIn.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            button_NavCheckIn.ForeColor = Color.White;
            button_NavCheckIn.Location = new Point(18, 8);
            button_NavCheckIn.Name = "button_NavCheckIn";
            button_NavCheckIn.Size = new Size(330, 40);
            button_NavCheckIn.TabIndex = 0;
            button_NavCheckIn.Text = "⏱️ Chấm công hôm nay (Check-in/out)";
            button_NavCheckIn.UseVisualStyleBackColor = false;
            button_NavCheckIn.Click += btnNav_CheckIn_Click;
            // 
            // button_NavRequests
            // 
            button_NavRequests.BackColor = Color.FromArgb(30, 41, 59);
            button_NavRequests.FlatAppearance.BorderSize = 0;
            button_NavRequests.FlatStyle = FlatStyle.Flat;
            button_NavRequests.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            button_NavRequests.ForeColor = Color.FromArgb(203, 213, 225);
            button_NavRequests.Location = new Point(354, 8);
            button_NavRequests.Name = "button_NavRequests";
            button_NavRequests.Size = new Size(310, 40);
            button_NavRequests.TabIndex = 1;
            button_NavRequests.Text = "📝 Tạo đơn từ & Đề xuất (Requests)";
            button_NavRequests.UseVisualStyleBackColor = false;
            button_NavRequests.Click += btnNav_Requests_Click;
            // 
            // button_NavRules
            // 
            button_NavRules.BackColor = Color.FromArgb(30, 41, 59);
            button_NavRules.FlatAppearance.BorderSize = 0;
            button_NavRules.FlatStyle = FlatStyle.Flat;
            button_NavRules.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            button_NavRules.ForeColor = Color.FromArgb(203, 213, 225);
            button_NavRules.Location = new Point(670, 8);
            button_NavRules.Name = "button_NavRules";
            button_NavRules.Size = new Size(240, 40);
            button_NavRules.TabIndex = 2;
            button_NavRules.Text = "📋 Nội quy chấm công";
            button_NavRules.UseVisualStyleBackColor = false;
            button_NavRules.Click += btnNav_Rules_Click;
            // 
            // panel_SubHeader
            // 
            panel_SubHeader.BackColor = Color.FromArgb(241, 245, 249);
            panel_SubHeader.Controls.Add(label_TabDesc);
            panel_SubHeader.Controls.Add(label_TabTitle);
            panel_SubHeader.Dock = DockStyle.Top;
            panel_SubHeader.Location = new Point(0, 161);
            panel_SubHeader.Name = "panel_SubHeader";
            panel_SubHeader.Padding = new Padding(22, 12, 22, 10);
            panel_SubHeader.Size = new Size(1400, 62);
            panel_SubHeader.TabIndex = 2;
            // 
            // label_TabDesc
            // 
            label_TabDesc.AutoSize = true;
            label_TabDesc.Font = new Font("Segoe UI", 9.5F);
            label_TabDesc.ForeColor = Color.FromArgb(71, 85, 105);
            label_TabDesc.Location = new Point(22, 33);
            label_TabDesc.Name = "label_TabDesc";
            label_TabDesc.Size = new Size(491, 21);
            label_TabDesc.TabIndex = 1;
            label_TabDesc.Text = "Quẹt thẻ bắt đầu và kết thúc ca làm việc trực tuyến theo thời gian thực.";
            // 
            // label_TabTitle
            // 
            label_TabTitle.AutoSize = true;
            label_TabTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label_TabTitle.ForeColor = Color.FromArgb(30, 41, 59);
            label_TabTitle.Location = new Point(20, 8);
            label_TabTitle.Name = "label_TabTitle";
            label_TabTitle.Size = new Size(544, 28);
            label_TabTitle.TabIndex = 0;
            label_TabTitle.Text = "⏱️ CHẤM CÔNG HÀNG NGÀY (CHECK-IN / CHECK-OUT)";
            // 
            // panel_MainContent
            // 
            panel_MainContent.BackColor = Color.FromArgb(248, 250, 252);
            panel_MainContent.Dock = DockStyle.Fill;
            panel_MainContent.Location = new Point(0, 223);
            panel_MainContent.Name = "panel_MainContent";
            panel_MainContent.Size = new Size(1400, 627);
            panel_MainContent.TabIndex = 3;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(248, 250, 252);
            ClientSize = new Size(1400, 850);
            Controls.Add(panel_MainContent);
            Controls.Add(panel_SubHeader);
            Controls.Add(panel_NavBar);
            Controls.Add(panel_Top);
            Font = new Font("Segoe UI", 9F);
            MinimumSize = new Size(1100, 700);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "HRM - Cổng Chấm công & Dịch vụ Nhân viên";
            panel_Top.ResumeLayout(false);
            panel_Top.PerformLayout();
            panel_NavBar.ResumeLayout(false);
            panel_SubHeader.ResumeLayout(false);
            panel_SubHeader.PerformLayout();
            ResumeLayout(false);
        }

        private Panel panel_Top;
        private Label label_HeaderTitle;
        private Label label_HeaderSubtitle;
        private Label label_Clock;
        private Label label_SelectEmp;
        private ComboBox comboBox_Employees;
        private Label label_EmpBadge;
        private FlowLayoutPanel panel_NavBar;
        private Button button_NavCheckIn;
        private Button button_NavRequests;
        private Button button_NavRules;
        private Panel panel_SubHeader;
        private Label label_TabTitle;
        private Label label_TabDesc;
        private Panel panel_MainContent;
    }
}

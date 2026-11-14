namespace Admin.WinForms.Controls.Attendance
{
    partial class AttendanceOverviewControl
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
            flowLayoutPanel1 = new FlowLayoutPanel();
            button_Overview = new Button();
            button_CheckIn = new Button();
            button_Requests = new Button();
            button_Approvals = new Button();
            button_Reports = new Button();
            button_Config = new Button();
            panel_Attendance = new Panel();
            flowLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(button_Overview);
            flowLayoutPanel1.Controls.Add(button_CheckIn);
            flowLayoutPanel1.Controls.Add(button_Requests);
            flowLayoutPanel1.Controls.Add(button_Approvals);
            flowLayoutPanel1.Controls.Add(button_Reports);
            flowLayoutPanel1.Controls.Add(button_Config);
            flowLayoutPanel1.Location = new Point(15, 3);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(1797, 81);
            flowLayoutPanel1.TabIndex = 11;
            // 
            // button_Overview
            // 
            button_Overview.BackColor = Color.Transparent;
            button_Overview.FlatAppearance.BorderSize = 0;
            button_Overview.FlatStyle = FlatStyle.Flat;
            button_Overview.Location = new Point(3, 3);
            button_Overview.Name = "button_Overview";
            button_Overview.Size = new Size(220, 68);
            button_Overview.TabIndex = 0;
            button_Overview.Text = "Overview";
            button_Overview.UseVisualStyleBackColor = false;
            button_Overview.Click += btnOverview_Click;
            // 
            // button_CheckIn
            // 
            button_CheckIn.BackColor = Color.Transparent;
            button_CheckIn.FlatAppearance.BorderSize = 0;
            button_CheckIn.FlatStyle = FlatStyle.Flat;
            button_CheckIn.Location = new Point(229, 3);
            button_CheckIn.Name = "button_CheckIn";
            button_CheckIn.Size = new Size(240, 68);
            button_CheckIn.TabIndex = 1;
            button_CheckIn.Text = "Check-in / Out";
            button_CheckIn.UseVisualStyleBackColor = false;
            button_CheckIn.Click += btnCheckIn_Click;
            // 
            // button_Requests
            // 
            button_Requests.BackColor = Color.Transparent;
            button_Requests.FlatAppearance.BorderSize = 0;
            button_Requests.FlatStyle = FlatStyle.Flat;
            button_Requests.Location = new Point(475, 3);
            button_Requests.Name = "button_Requests";
            button_Requests.Size = new Size(240, 68);
            button_Requests.TabIndex = 2;
            button_Requests.Text = "Đơn từ (Requests)";
            button_Requests.UseVisualStyleBackColor = false;
            button_Requests.Click += btnRequests_Click;
            // 
            // button_Approvals
            // 
            button_Approvals.BackColor = Color.Transparent;
            button_Approvals.FlatAppearance.BorderSize = 0;
            button_Approvals.FlatStyle = FlatStyle.Flat;
            button_Approvals.Location = new Point(721, 3);
            button_Approvals.Name = "button_Approvals";
            button_Approvals.Size = new Size(220, 68);
            button_Approvals.TabIndex = 3;
            button_Approvals.Text = "Approvals";
            button_Approvals.UseVisualStyleBackColor = false;
            button_Approvals.Click += btnApprovals_Click;
            // 
            // button_Reports
            // 
            button_Reports.BackColor = Color.Transparent;
            button_Reports.FlatAppearance.BorderSize = 0;
            button_Reports.FlatStyle = FlatStyle.Flat;
            button_Reports.Location = new Point(947, 3);
            button_Reports.Name = "button_Reports";
            button_Reports.Size = new Size(220, 68);
            button_Reports.TabIndex = 4;
            button_Reports.Text = "Reports";
            button_Reports.UseVisualStyleBackColor = false;
            button_Reports.Click += btnReports_Click;
            // 
            // button_Config
            // 
            button_Config.BackColor = Color.Transparent;
            button_Config.FlatAppearance.BorderSize = 0;
            button_Config.FlatStyle = FlatStyle.Flat;
            button_Config.Location = new Point(1173, 3);
            button_Config.Name = "button_Config";
            button_Config.Size = new Size(240, 68);
            button_Config.TabIndex = 5;
            button_Config.Text = "Cấu hình (Rules)";
            button_Config.UseVisualStyleBackColor = false;
            button_Config.Click += btnConfig_Click;
            // 
            // panel_Attendance
            // 
            panel_Attendance.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel_Attendance.Location = new Point(15, 90);
            panel_Attendance.Name = "panel_Attendance";
            panel_Attendance.Size = new Size(1797, 786);
            panel_Attendance.TabIndex = 12;
            // 
            // AttendanceOverviewControl
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(panel_Attendance);
            Controls.Add(flowLayoutPanel1);
            Name = "AttendanceOverviewControl";
            Size = new Size(1815, 884);
            flowLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private FlowLayoutPanel flowLayoutPanel1;
        private Button button_Overview;
        private Button button_CheckIn;
        private Button button_Requests;
        private Button button_Approvals;
        private Button button_Reports;
        private Button button_Config;
        private Panel panel_Attendance;
    }
}

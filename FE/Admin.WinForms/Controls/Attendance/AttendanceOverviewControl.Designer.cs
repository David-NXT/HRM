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
            button_Approvals = new Button();
            button_Reports = new Button();
            panel_Attendance = new Panel();
            flowLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(button_Overview);
            flowLayoutPanel1.Controls.Add(button_Approvals);
            flowLayoutPanel1.Controls.Add(button_Reports);
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
            button_Overview.Size = new Size(328, 68);
            button_Overview.TabIndex = 0;
            button_Overview.Text = "Overview";
            button_Overview.UseVisualStyleBackColor = false;
            button_Overview.Click += btnOverview_Click;
            // 
            // button_Approvals
            // 
            button_Approvals.BackColor = Color.Transparent;
            button_Approvals.FlatAppearance.BorderSize = 0;
            button_Approvals.FlatStyle = FlatStyle.Flat;
            button_Approvals.Location = new Point(337, 3);
            button_Approvals.Name = "button_Approvals";
            button_Approvals.Size = new Size(328, 68);
            button_Approvals.TabIndex = 1;
            button_Approvals.Text = "Approvals";
            button_Approvals.UseVisualStyleBackColor = false;
            button_Approvals.Click += btnApprovals_Click;
            // 
            // button_Reports
            // 
            button_Reports.BackColor = Color.Transparent;
            button_Reports.FlatAppearance.BorderSize = 0;
            button_Reports.FlatStyle = FlatStyle.Flat;
            button_Reports.Location = new Point(671, 3);
            button_Reports.Name = "button_Reports";
            button_Reports.Size = new Size(328, 68);
            button_Reports.TabIndex = 2;
            button_Reports.Text = "Reports";
            button_Reports.UseVisualStyleBackColor = false;
            button_Reports.Click += btnReports_Click;
            // 
            // panel_Attendance
            // 
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
        private Button button_Approvals;
        private Button button_Reports;
        private Panel panel_Attendance;
    }
}

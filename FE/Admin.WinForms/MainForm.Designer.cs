namespace Admin.WinForms
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panel_Sidebar = new Panel();
            label_HRM = new Label();
            flowLayoutPanel2 = new FlowLayoutPanel();
            flowLayoutPanel1 = new FlowLayoutPanel();
            button_System = new Button();
            button_Project = new Button();
            button_Payroll = new Button();
            button_Attendance = new Button();
            button_Organization = new Button();
            button_Employee = new Button();
            button_Contract_Work = new Button();
            button_Dashboard = new Button();
            panel_Content = new Panel();
            panel_Sidebar.SuspendLayout();
            SuspendLayout();
            // 
            // panel_Sidebar
            // 
            panel_Sidebar.BackColor = Color.FromArgb(22, 38, 70);
            panel_Sidebar.Controls.Add(label_HRM);
            panel_Sidebar.Controls.Add(flowLayoutPanel2);
            panel_Sidebar.Controls.Add(flowLayoutPanel1);
            panel_Sidebar.Controls.Add(button_System);
            panel_Sidebar.Controls.Add(button_Project);
            panel_Sidebar.Controls.Add(button_Payroll);
            panel_Sidebar.Controls.Add(button_Attendance);
            panel_Sidebar.Controls.Add(button_Organization);
            panel_Sidebar.Controls.Add(button_Employee);
            panel_Sidebar.Controls.Add(button_Contract_Work);
            panel_Sidebar.Controls.Add(button_Dashboard);
            panel_Sidebar.Location = new Point(0, -1);
            panel_Sidebar.Name = "panel_Sidebar";
            panel_Sidebar.Size = new Size(300, 797);
            panel_Sidebar.TabIndex = 0;
            // 
            // label_HRM
            // 
            label_HRM.AutoSize = true;
            label_HRM.BackColor = Color.Transparent;
            label_HRM.FlatStyle = FlatStyle.Flat;
            label_HRM.Font = new Font("Segoe UI", 30F, FontStyle.Bold);
            label_HRM.ForeColor = Color.White;
            label_HRM.Location = new Point(31, 21);
            label_HRM.Name = "label_HRM";
            label_HRM.Size = new Size(235, 106);
            label_HRM.TabIndex = 8;
            label_HRM.Text = "HRM";
            // 
            // flowLayoutPanel2
            // 
            flowLayoutPanel2.Location = new Point(301, 2);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            flowLayoutPanel2.Size = new Size(1266, 795);
            flowLayoutPanel2.TabIndex = 1;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Location = new Point(303, 5);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(1265, 786);
            flowLayoutPanel1.TabIndex = 1;
            // 
            // button_System
            // 
            button_System.BackColor = Color.Transparent;
            button_System.FlatAppearance.BorderSize = 0;
            button_System.FlatAppearance.MouseDownBackColor = Color.FromArgb(49, 95, 159);
            button_System.FlatAppearance.MouseOverBackColor = Color.FromArgb(71, 117, 188);
            button_System.FlatStyle = FlatStyle.Flat;
            button_System.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            button_System.ForeColor = Color.White;
            button_System.Location = new Point(31, 510);
            button_System.Name = "button_System";
            button_System.Size = new Size(248, 46);
            button_System.TabIndex = 7;
            button_System.Text = "System";
            button_System.TextAlign = ContentAlignment.MiddleLeft;
            button_System.UseVisualStyleBackColor = false;
            button_System.Click += button_System_Click;
            // 
            // button_Project
            // 
            button_Project.BackColor = Color.Transparent;
            button_Project.FlatAppearance.BorderSize = 0;
            button_Project.FlatAppearance.MouseDownBackColor = Color.FromArgb(49, 95, 159);
            button_Project.FlatAppearance.MouseOverBackColor = Color.FromArgb(71, 117, 188);
            button_Project.FlatStyle = FlatStyle.Flat;
            button_Project.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            button_Project.ForeColor = Color.White;
            button_Project.Location = new Point(31, 458);
            button_Project.Name = "button_Project";
            button_Project.Size = new Size(248, 46);
            button_Project.TabIndex = 6;
            button_Project.Text = "Project";
            button_Project.TextAlign = ContentAlignment.MiddleLeft;
            button_Project.UseVisualStyleBackColor = false;
            button_Project.Click += button_Project_Click;
            // 
            // button_Payroll
            // 
            button_Payroll.BackColor = Color.Transparent;
            button_Payroll.FlatAppearance.BorderSize = 0;
            button_Payroll.FlatAppearance.MouseDownBackColor = Color.FromArgb(49, 95, 159);
            button_Payroll.FlatAppearance.MouseOverBackColor = Color.FromArgb(71, 117, 188);
            button_Payroll.FlatStyle = FlatStyle.Flat;
            button_Payroll.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            button_Payroll.ForeColor = Color.White;
            button_Payroll.Location = new Point(31, 406);
            button_Payroll.Name = "button_Payroll";
            button_Payroll.Size = new Size(248, 46);
            button_Payroll.TabIndex = 5;
            button_Payroll.Text = "Payroll";
            button_Payroll.TextAlign = ContentAlignment.MiddleLeft;
            button_Payroll.UseVisualStyleBackColor = false;
            button_Payroll.Click += button_Payroll_Click;
            // 
            // button_Attendance
            // 
            button_Attendance.BackColor = Color.Transparent;
            button_Attendance.FlatAppearance.BorderSize = 0;
            button_Attendance.FlatAppearance.MouseDownBackColor = Color.FromArgb(49, 95, 159);
            button_Attendance.FlatAppearance.MouseOverBackColor = Color.FromArgb(71, 117, 188);
            button_Attendance.FlatStyle = FlatStyle.Flat;
            button_Attendance.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            button_Attendance.ForeColor = Color.White;
            button_Attendance.Location = new Point(31, 354);
            button_Attendance.Name = "button_Attendance";
            button_Attendance.Size = new Size(248, 46);
            button_Attendance.TabIndex = 4;
            button_Attendance.Text = "Attendance";
            button_Attendance.TextAlign = ContentAlignment.MiddleLeft;
            button_Attendance.UseVisualStyleBackColor = false;
            button_Attendance.Click += button_Attendance_Click;
            // 
            // button_Organization
            // 
            button_Organization.BackColor = Color.Transparent;
            button_Organization.FlatAppearance.BorderSize = 0;
            button_Organization.FlatAppearance.MouseDownBackColor = Color.FromArgb(49, 95, 159);
            button_Organization.FlatAppearance.MouseOverBackColor = Color.FromArgb(71, 117, 188);
            button_Organization.FlatStyle = FlatStyle.Flat;
            button_Organization.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            button_Organization.ForeColor = Color.White;
            button_Organization.Location = new Point(31, 302);
            button_Organization.Name = "button_Organization";
            button_Organization.Size = new Size(248, 46);
            button_Organization.TabIndex = 3;
            button_Organization.Text = "Organization";
            button_Organization.TextAlign = ContentAlignment.MiddleLeft;
            button_Organization.UseVisualStyleBackColor = false;
            button_Organization.Click += button_Organization_Click;
            // 
            // button_Employee
            // 
            button_Employee.BackColor = Color.Transparent;
            button_Employee.FlatAppearance.BorderSize = 0;
            button_Employee.FlatAppearance.MouseDownBackColor = Color.FromArgb(49, 95, 159);
            button_Employee.FlatAppearance.MouseOverBackColor = Color.FromArgb(71, 117, 188);
            button_Employee.FlatStyle = FlatStyle.Flat;
            button_Employee.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            button_Employee.ForeColor = Color.White;
            button_Employee.Location = new Point(31, 198);
            button_Employee.Name = "button_Employee";
            button_Employee.Size = new Size(248, 46);
            button_Employee.TabIndex = 2;
            button_Employee.Text = "Employee";
            button_Employee.TextAlign = ContentAlignment.MiddleLeft;
            button_Employee.UseVisualStyleBackColor = false;
            button_Employee.Click += button_Employee_Click;
            // 
            // button_Contract_Work
            // 
            button_Contract_Work.BackColor = Color.Transparent;
            button_Contract_Work.FlatAppearance.BorderSize = 0;
            button_Contract_Work.FlatAppearance.MouseDownBackColor = Color.FromArgb(49, 95, 159);
            button_Contract_Work.FlatAppearance.MouseOverBackColor = Color.FromArgb(71, 117, 188);
            button_Contract_Work.FlatStyle = FlatStyle.Flat;
            button_Contract_Work.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            button_Contract_Work.ForeColor = Color.White;
            button_Contract_Work.Location = new Point(31, 250);
            button_Contract_Work.Name = "button_Contract_Work";
            button_Contract_Work.Size = new Size(248, 46);
            button_Contract_Work.TabIndex = 1;
            button_Contract_Work.Text = "Contracts and Work";
            button_Contract_Work.TextAlign = ContentAlignment.MiddleLeft;
            button_Contract_Work.UseVisualStyleBackColor = false;
            button_Contract_Work.Click += button_Contract_Work_Click;
            // 
            // button_Dashboard
            // 
            button_Dashboard.BackColor = Color.Transparent;
            button_Dashboard.FlatAppearance.BorderSize = 0;
            button_Dashboard.FlatAppearance.MouseDownBackColor = Color.FromArgb(49, 95, 159);
            button_Dashboard.FlatAppearance.MouseOverBackColor = Color.FromArgb(71, 117, 188);
            button_Dashboard.FlatStyle = FlatStyle.Flat;
            button_Dashboard.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            button_Dashboard.ForeColor = Color.White;
            button_Dashboard.ImageAlign = ContentAlignment.MiddleLeft;
            button_Dashboard.Location = new Point(31, 146);
            button_Dashboard.Name = "button_Dashboard";
            button_Dashboard.Size = new Size(248, 46);
            button_Dashboard.TabIndex = 0;
            button_Dashboard.Text = "Dashboard";
            button_Dashboard.TextAlign = ContentAlignment.MiddleLeft;
            button_Dashboard.UseVisualStyleBackColor = false;
            button_Dashboard.Click += button_Dashboard_Click;
            // 
            // panel_Content
            // 
            panel_Content.BackColor = Color.FromArgb(244, 246, 249);
            panel_Content.Location = new Point(304, 2);
            panel_Content.Name = "panel_Content";
            panel_Content.Size = new Size(1264, 794);
            panel_Content.TabIndex = 1;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(244, 246, 249);
            ClientSize = new Size(1567, 795);
            Controls.Add(panel_Content);
            Controls.Add(panel_Sidebar);
            Name = "MainForm";
            Text = "MainForm";
            Load += MainForm_Load;
            panel_Sidebar.ResumeLayout(false);
            panel_Sidebar.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel_Sidebar;
        private Button button_Dashboard;
        private Button button_Contract_Work;
        private Button button_Organization;
        private Button button_Employee;
        private Button button_System;
        private Button button_Project;
        private Button button_Payroll;
        private Button button_Attendance;
        private FlowLayoutPanel flowLayoutPanel1;
        private FlowLayoutPanel flowLayoutPanel2;
        private Panel panel_Content;
        private Label label_HRM;
    }
}
